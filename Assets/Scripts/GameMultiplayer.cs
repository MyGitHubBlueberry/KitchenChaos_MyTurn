using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMultiplayer : NetworkBehaviour
{
   public const int MAX_PLAYER_AMOUNT = 4;
   public const string PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER = "PlayerNameMultiplayer";


   public static GameMultiplayer Instance {get; private set;}


   public static bool PlayMultiplayer;


   public event EventHandler OnTryingToJoinGame;
   public event EventHandler OnFaleToJoinGame;
   public event EventHandler OnPlayerDataNetworkListChanged;
   


   [SerializeField] private KithcenObjectListSO _kithcenObjectListSO;
   [SerializeField] private List<Color> _playerColorList;


   private NetworkList<PlayerData> _playerDataNetworkList;
   private string _playerName;


   private void Awake()
   {
      Instance = this;

      DontDestroyOnLoad(gameObject);

      _playerName = PlayerPrefs.GetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, "Player " + UnityEngine.Random.Range(100,1000));


      _playerDataNetworkList = new NetworkList<PlayerData>();
      _playerDataNetworkList.OnListChanged +=  PlayerDataNetworkList_OnListChanged;
   }

   private void PlayerDataNetworkList_OnListChanged(NetworkListEvent<PlayerData> changeEvent)
   {
      OnPlayerDataNetworkListChanged?.Invoke(this, EventArgs.Empty);
   }

   public string GetPlayerName()
   {
      return _playerName;
   }

   public void SetPlayerName(string newPlayerName)
   {
      _playerName = newPlayerName;

      PlayerPrefs.SetString(PLAYER_PREFS_PLAYER_NAME_MULTIPLAYER, _playerName);
   }


   private void Start()
   {
      if(!PlayMultiplayer)
      {
         StartHost();
         Loader.LoadNetwork(Loader.Scene.GameScene);
      }

      SceneManager.activeSceneChanged += SceneManager_ActiveSceneChanged;

   }

   private void SceneManager_ActiveSceneChanged(Scene previousScene, Scene newScene)
   {
      if(newScene.name == Loader.Scene.GameScene.ToString())
         DeliveryManager.Instance.OnPointsAdded += DeliveryManager_OnPointsAdded;

      if(previousScene.name == Loader.Scene.GameScene.ToString())
         DeliveryManager.Instance.OnPointsAdded -= DeliveryManager_OnPointsAdded;
      
   }
   
   private void DeliveryManager_OnPointsAdded(object sender, DeliveryManager.OnPointsAddedEventArgs deliveryInfo)
   {
      AddPlayerPointsAndSuccessRecipeServerRpc(deliveryInfo.GetAddedPoints, deliveryInfo.GetPlayerClientId);
   }

   [ServerRpc(RequireOwnership = false)]
   private void AddPlayerPointsAndSuccessRecipeServerRpc(int playerPoints, ulong playerClientId)
   {
      int playerDataIndex = GameMultiplayer.Instance.GetPlayerDataIndexFromClientId(playerClientId);
      
      PlayerData playerData = _playerDataNetworkList[playerDataIndex];

      playerData.playerPoints += playerPoints;
      playerData.playerSuccessRecipesAmount++;

      _playerDataNetworkList[playerDataIndex] = playerData;
   }




   public void StartHost()
   {
      NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
      NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Server_OnClientConnectedCallback;
      NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Server_OnClientDisconnectCallback;
      NetworkManager.Singleton.StartHost();
   }
   

   private void NetworkManager_Server_OnClientDisconnectCallback(ulong clientId)
   {
      for (int i = 0; i < _playerDataNetworkList.Count; i++)
      {
         PlayerData playerData  = _playerDataNetworkList[i];

         if(playerData.clientId == clientId)
         {
            _playerDataNetworkList.RemoveAt(i);
         }
      }

   }

   private void NetworkManager_Server_OnClientConnectedCallback(ulong clientId)
   {
      _playerDataNetworkList.Add(new PlayerData
      {
         clientId = clientId,
         colorId = GetFirstUnusedColorId(),
         playerPoints = 0,
         playerSuccessRecipesAmount = 0
      });

      SetPlayerNameServerRpc(GetPlayerName());
      SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
   }

   private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest connectionApprovalRequest, NetworkManager.ConnectionApprovalResponse connectionApprovalResponse)
   {
      if(SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
      {
         connectionApprovalResponse.Approved = false;
         connectionApprovalResponse.Reason = "Game has already started";
         
         return;
      }

      if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT)
      {
         connectionApprovalResponse.Approved = false;
         connectionApprovalResponse.Reason = "Game is full";

         return;
      }

      connectionApprovalResponse.Approved = true;
   }



   public void StartClient()
   {
      OnTryingToJoinGame?.Invoke(this, EventArgs.Empty);

      NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_Client_OnClientDisconnectCallback;
      NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_Client_OnClientConnectedCallback;
      NetworkManager.Singleton.StartClient();
   }

   private void NetworkManager_Client_OnClientConnectedCallback(ulong clientId)
   {
      SetPlayerNameServerRpc(GetPlayerName());
      SetPlayerIdServerRpc(AuthenticationService.Instance.PlayerId);
   }

   [ServerRpc(RequireOwnership = false)]
   private void SetPlayerNameServerRpc(string playerName, ServerRpcParams serverRpcParams = default)
   {
      int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
      
      PlayerData playerData = _playerDataNetworkList[playerDataIndex];

      playerData.playerName = playerName;

      _playerDataNetworkList[playerDataIndex] = playerData;
   }

   [ServerRpc(RequireOwnership = false)]
   private void SetPlayerIdServerRpc(string playerId, ServerRpcParams serverRpcParams = default)
   {
      int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
      
      PlayerData playerData = _playerDataNetworkList[playerDataIndex];

      playerData.playerId = playerId;

      _playerDataNetworkList[playerDataIndex] = playerData;
   }

   private void NetworkManager_Client_OnClientDisconnectCallback(ulong clientId)
   {
      OnFaleToJoinGame?.Invoke(this, EventArgs.Empty);
   }



   public void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent kitchenObjectParent)
   {
      SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjectSO), kitchenObjectParent.GetNetworkObject());
   }
   
   [ServerRpc(RequireOwnership = false)]
   private void SpawnKitchenObjectServerRpc(int kitchenObjectSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
   {
      kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
      IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

      if(kitchenObjectParent.HasKitchenObject()) return;

      Transform kitchenObjectTransform = Instantiate(GetKitchenObjectSOFromIndex(kitchenObjectSOIndex).GetPrefab);

      NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
      kitchenObjectNetworkObject.Spawn(true);

      KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
      
      kitchenObject.SetKitchenObjectParent(kitchenObjectParent);
   }

   public int GetKitchenObjectSOIndex(KitchenObjectSO kitchenObjectSO)
   {
      return _kithcenObjectListSO.GetKitchenObjectSOList.IndexOf(kitchenObjectSO);
   }

   public KitchenObjectSO GetKitchenObjectSOFromIndex(int kitchenObjectSOIndex)
   {
      return _kithcenObjectListSO.GetKitchenObjectSOList[kitchenObjectSOIndex];
   }



   public void DestroyKitchenObject(KitchenObject kitchenObject)
   {
      DestroyKitchenObjectServerRpc(kitchenObject.NetworkObject);
   }

   [ServerRpc(RequireOwnership = false)]
   private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
   {
      kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);

      if(kitchenObjectNetworkObject == null) return;

      KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();
      
      ClearKitchenObjectOnParentClientRpc(kitchenObjectNetworkObjectReference);

      kitchenObject.DestroyItSelf();
      kitchenObjectNetworkObject.Despawn();
   }

   [ClientRpc]
   private void ClearKitchenObjectOnParentClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
   {
      kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kitchenObjectNetworkObject);
      KitchenObject kitchenObject = kitchenObjectNetworkObject.GetComponent<KitchenObject>();

      kitchenObject.ClearKitchenObjectOnParent();
   }

   public void ChangePlayerColor(int colorId)
   {
      ChangePlayerColorServerRpc(colorId);
   }

   [ServerRpc(RequireOwnership = false)]
   private void ChangePlayerColorServerRpc(int colorId, ServerRpcParams serverRpcParams = default)
   {
      if(!IsColorAvailible(colorId)) return;

      int playerDataIndex = GetPlayerDataIndexFromClientId(serverRpcParams.Receive.SenderClientId);
      
      PlayerData playerData = _playerDataNetworkList[playerDataIndex];

      playerData.colorId = colorId;

      _playerDataNetworkList[playerDataIndex] = playerData;
   } 

   private bool IsColorAvailible(int colorId)
   {
      foreach(PlayerData playerData in _playerDataNetworkList)
         if(playerData.colorId == colorId) 
            return false;
      
      return true;
   }

   private int GetFirstUnusedColorId()
   {
      for(int i = 0; i < _playerColorList.Count; i++)
         if(IsColorAvailible(i))
            return i;

      return -1;
   }




   public bool IsPlayerIndexConnected(int playerIndex)
   {
      return playerIndex < _playerDataNetworkList.Count;
   }

   public int GetPlayerDataIndexFromClientId(ulong clientId)
   {
      for(int i = 0; i < _playerDataNetworkList.Count; i++)
         if(_playerDataNetworkList[i].clientId == clientId)
            return i;

      return -1;
   }

   public PlayerData GetPlayerDataFromClientId(ulong clientId)
   {
      foreach(PlayerData playerData in _playerDataNetworkList)
         if(playerData.clientId == clientId)
            return playerData;

      return default;
   }

   public PlayerData GetPlayerData()
   {
      return GetPlayerDataFromClientId(NetworkManager.Singleton.LocalClientId);
   }

   public PlayerData GetPlayerDataFromPlayerIndex(int playerIndex)
   {
      return _playerDataNetworkList[playerIndex];
   }

   public Color GetPlayerColor(int colorId)
   {
      return _playerColorList[colorId];
   }
   public void KickPlayer(ulong clientId)
   {
      NetworkManager.Singleton.DisconnectClient(clientId);
      NetworkManager_Server_OnClientDisconnectCallback(clientId);
   }
}
