using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using System;
using UnityEngine.SceneManagement;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;

public class GameLobby : MonoBehaviour
{
   private const string KEY_RELAY_JOIN_CODE = "RelayJoinCode";


   public static GameLobby Instance { get; private set; }


   public event EventHandler OnCreateLobbyStarted;
   public event EventHandler OnCreateLobbyFailed;
   public event EventHandler OnCreateLobbyFailedNullArgument;

   public event EventHandler OnJoinStarted;
   public event EventHandler OnQuickJoinFailed;
   public event EventHandler OnJoinFailed;
   public event EventHandler OnJoinFailedNullArgument;

   public event EventHandler<OnLobbyListChangedEventArgs> OnLobbyListChanged;
   public class OnLobbyListChangedEventArgs : EventArgs
   {
      public readonly List<Lobby> GetLobbyList;
      public OnLobbyListChangedEventArgs(List<Lobby> lobbyList)
      {
         GetLobbyList = lobbyList;
      }
   }



   private Lobby _joinedLobby;
   private float _heartbeatTimer;
   private float _leastLobbiesTimer;



   private void Awake()
   {
      Instance = this;
      

      DontDestroyOnLoad(gameObject);

      InitializeUnityAuthentification();
   }

   private async void InitializeUnityAuthentification()
   {
      if(UnityServices.State != ServicesInitializationState.Initialized)
      {  
         InitializationOptions initializationOptions = new InitializationOptions();
         //initializationOptions.SetProfile(UnityEngine.Random.Range(0, 10000).ToString());

         await UnityServices.InitializeAsync(initializationOptions);      

         await AuthenticationService.Instance.SignInAnonymouslyAsync();      
      }
   }



   private void Update()
   {
      HandleHeartbeat();
      HandlePeriodicListLobbies();
   }

   private void HandlePeriodicListLobbies()
   {
      if(_joinedLobby == null && 
      AuthenticationService.Instance.IsSignedIn && 
      SceneManager.GetActiveScene().name == Loader.Scene.LobbyScene.ToString())
      {
         _leastLobbiesTimer -= Time.deltaTime;

         if(_leastLobbiesTimer <= 0f)
         {
            float _leastLobbiesTimerMax = 3f;
            _leastLobbiesTimer = _leastLobbiesTimerMax;

            ListLobbies();
         }
      }
   }

   private void HandleHeartbeat()
   {
      if(!IsLobbyHost()) return;

      _heartbeatTimer -= Time.deltaTime;

      if(_heartbeatTimer <= 0f)
      {
         float _heartbeatTimerMax = 15f;
         _heartbeatTimer = _heartbeatTimerMax;

         LobbyService.Instance.SendHeartbeatPingAsync(_joinedLobby.Id);
      }
   }



   private bool IsLobbyHost()
   {
      return _joinedLobby != null && _joinedLobby.HostId == AuthenticationService.Instance.PlayerId;
   }

   private async void ListLobbies()
   {
      try
      {
         QueryLobbiesOptions queryLobbiesOptions = new QueryLobbiesOptions
         {
            Filters = new List<QueryFilter>
            {
               new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT)
            }
         };

         QueryResponse queryResponse = await LobbyService.Instance.QueryLobbiesAsync(queryLobbiesOptions);

         OnLobbyListChanged?.Invoke(this, new OnLobbyListChangedEventArgs(queryResponse.Results));
      }
      catch(LobbyServiceException exc)
      {
         Debug.Log(exc);
      }
   } 

   private async Task<Allocation> AllocateRelay()
   {  
      int playersAmountExcludingTheHost = GameMultiplayer.MAX_PLAYER_AMOUNT - 1;
      
      try
      {
         Allocation allocation = await RelayService.Instance.CreateAllocationAsync(playersAmountExcludingTheHost);
         return allocation;
      }
      catch(RelayServiceException exc)
      {
         Debug.Log(exc);

         return default;
      }
   }

   private async Task<string> GetRelayJoinCode(Allocation allocation)
   {
      try
      {
         string relayJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

         return relayJoinCode;
      }
      catch(RelayServiceException exc)
      {
         Debug.Log(exc);

         return default;
      }
   }

   private async Task<JoinAllocation> JoinRelayByCode(string relayJoinCode)
   {
      try
      {
         JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);

         return joinAllocation;
      }
      catch(RelayServiceException exc)
      {
         Debug.Log(exc);

         return default;
      }
   }

   public async void CreateLobby(string lobbyName, bool isPrivate)
   {
      OnCreateLobbyStarted?.Invoke(this, EventArgs.Empty);

      try
      {
         _joinedLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, GameMultiplayer.MAX_PLAYER_AMOUNT, new CreateLobbyOptions
         {
            IsPrivate = isPrivate
         });

         Allocation allocation = await AllocateRelay();

         string relayJoinCode = await GetRelayJoinCode(allocation);

         await LobbyService.Instance.UpdateLobbyAsync(_joinedLobby.Id, new UpdateLobbyOptions
         {
            Data = new Dictionary<string, DataObject>
            {
               {KEY_RELAY_JOIN_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayJoinCode)},
            }
         });
         
         NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));

         GameMultiplayer.Instance.StartHost();     
         Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);
      }
      catch(ArgumentNullException exc)
      {
         Debug.Log(exc);
         OnCreateLobbyFailedNullArgument?.Invoke(this, EventArgs.Empty);
      }
      catch(LobbyServiceException exc)
      {
         Debug.Log(exc);
         OnCreateLobbyFailed?.Invoke(this, EventArgs.Empty);
      }
   }

   public async void QuickJoin()
   {
      OnJoinStarted?.Invoke(this, EventArgs.Empty);

      try
      {
         _joinedLobby = await LobbyService.Instance.QuickJoinLobbyAsync();

         string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

         JoinAllocation joinAllocation = await JoinRelayByCode(relayJoinCode);

         NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

         GameMultiplayer.Instance.StartClient();
      }
      catch(LobbyServiceException exc)
      {
         Debug.Log(exc);
         OnQuickJoinFailed?.Invoke(this, EventArgs.Empty);
      }
   }

   public async void JoinByCode(string lobbyCode)
   {
      OnJoinStarted?.Invoke(this, EventArgs.Empty);

      try
      {
         _joinedLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode);

         string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

         JoinAllocation joinAllocation = await JoinRelayByCode(relayJoinCode);

         NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

         GameMultiplayer.Instance.StartClient();
      }
      catch(ArgumentNullException exc)
      {
         Debug.Log(exc);
         OnJoinFailedNullArgument?.Invoke(this, EventArgs.Empty);
      }
      catch(LobbyServiceException exc)
      {
         Debug.Log(exc);
         OnJoinFailed?.Invoke(this, EventArgs.Empty);
      }
   }

   public async void JoinById(string lobbyId)
   {
      OnJoinStarted?.Invoke(this, EventArgs.Empty);

      try
      {
         _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobbyId);

         string relayJoinCode = _joinedLobby.Data[KEY_RELAY_JOIN_CODE].Value;

         JoinAllocation joinAllocation = await JoinRelayByCode(relayJoinCode);

         NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));

         GameMultiplayer.Instance.StartClient();
      }
      catch(ArgumentNullException exc)
      {
         Debug.Log(exc);
         OnJoinFailedNullArgument?.Invoke(this, EventArgs.Empty);
      }
      catch(LobbyServiceException exc)
      {
         Debug.Log(exc);
         OnJoinFailed?.Invoke(this, EventArgs.Empty);
      }
   }

   public async void DeleteLobby()
   {
      if(_joinedLobby == null) return;
      
      try
      {
         await LobbyService.Instance.DeleteLobbyAsync(_joinedLobby.Id);

         _joinedLobby = null;
      }
      catch(LobbyServiceException exc)
      {
         Debug.Log(exc);
      }
   }

   public async void LeaveLobby()
   {
      if(_joinedLobby == null) return;

      try
      {
         await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id,AuthenticationService.Instance.PlayerId);
         
         _joinedLobby = null;
      }
      catch(LobbyServiceException exc)
      {
         Debug.Log(exc);
      }
   }

   public async void KickPlayer(string playerId)
   {
      if(!IsLobbyHost()) return;

      try
      {
         await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, playerId);
      }
      catch(LobbyServiceException exc)
      {
         Debug.Log(exc);
      }
   }


   public Lobby GetLobby()
   {
      return _joinedLobby;
   }
}
