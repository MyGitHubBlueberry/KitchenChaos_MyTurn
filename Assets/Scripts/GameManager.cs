using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
   public static GameManager Instance {get; private set;}


   public event EventHandler OnStateChanged;
   public event EventHandler OnLocalGamePaused;
   public event EventHandler OnLocalGameUnpaused;
   public event EventHandler OnLocalPlayerReadyChanged;
   public event EventHandler OnMultiplayerGamePased;
   public event EventHandler OnMultiplayerGameUnpased;


   private enum State
   {
      WaitingToStart,
      CountdownToStart,
      GamePlaying,
      GameOver,
   }


   [SerializeField] private Transform _playerPrefab;


   private NetworkVariable<State> _state = new NetworkVariable<State>(State.WaitingToStart);
   private NetworkVariable<float> _countdownToStartTimer = new NetworkVariable<float>(3f);
   private NetworkVariable<float> _gamePlayingTimer = new NetworkVariable<float>(0f);
   private NetworkVariable<bool> _isGamePaused = new NetworkVariable<bool>(false);
   private Dictionary<ulong,bool> _playerReadyDictionary;
   private Dictionary<ulong,bool> _playerPauseDictionary;
   private bool _isLocalPlayerReady;
   private bool _isLocalGamePaused = false;
   private float _gamePlayingTimerMax = 300f;
   private bool autoTestGamePausedState;



   private void Awake()
   {
      Instance = this;

      _playerReadyDictionary = new Dictionary<ulong, bool>();
      _playerPauseDictionary = new Dictionary<ulong, bool>();
   }



   private void Start()
   {
      DeliveryManager.Instance.OnPointsAdded += DeliveryManager_OnPointsAdded;
      GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
      GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
   }
   private void GameInput_OnInteractAction(object sender, EventArgs e)
   {
      if(_state.Value == State.WaitingToStart)
      {
         _isLocalPlayerReady = true;

         OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

         SetPlayerReadyServerRpc();
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
   {
      _playerReadyDictionary.Add(serverRpcParams.Receive.SenderClientId, true);

      bool allClientsReady = true;
      foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
      {
         if(!_playerReadyDictionary.ContainsKey(clientId) || !_playerReadyDictionary[clientId])
         {
            allClientsReady = false;
            break;
         }
      }

      if(allClientsReady) _state.Value = State.CountdownToStart;
   }

   private void GameInput_OnPauseAction(object sender, EventArgs e)
   {
      TogglePauseGame();
   }

   public void TogglePauseGame()
   {
      _isLocalGamePaused = !_isLocalGamePaused;


      if(_isLocalGamePaused)
      {
         PauseGameServerRpc();
         OnLocalGamePaused.Invoke(this, EventArgs.Empty);
      }
      else
      {
         UnPauseGameServerRpc();
         OnLocalGameUnpaused?.Invoke(this, EventArgs.Empty);
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
   {
      _playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = true;

      TestGamePauseState();
   }

   [ServerRpc(RequireOwnership = false)]
   private void UnPauseGameServerRpc(ServerRpcParams serverRpcParams = default)
   {
      _playerPauseDictionary[serverRpcParams.Receive.SenderClientId] = false;

      TestGamePauseState();
   }

   private void TestGamePauseState()
   {
      foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
      {
         if(_playerPauseDictionary.ContainsKey(clientId) && _playerPauseDictionary[clientId])
         {
            _isGamePaused.Value = true;
            return;
         }
      }

      _isGamePaused.Value = false;
   }

   private void DeliveryManager_OnPointsAdded(object sender, DeliveryManager.OnPointsAddedEventArgs e)
   {
      AddTimeServerRpc(e.GetAddedPoints);
   }

   [ServerRpc(RequireOwnership = false)]
   private void AddTimeServerRpc(int addedPoints)
   {
      _gamePlayingTimer.Value += addedPoints / 10;
   }



   public override void OnNetworkSpawn()
   {
      _state.OnValueChanged  += State_OnValueChanged;
      _isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

      if(IsServer)
      {
         NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
         NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
      }
   }

   private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
   {
      foreach(ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
      {
         Transform playerTransform = Instantiate(_playerPrefab);
         playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
      }
   }

   private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
   {
      autoTestGamePausedState = true;
   }

   private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
   {
      if(!GameMultiplayer.PlayMultiplayer) return;
      
      if(_isGamePaused.Value)
      {
         Time.timeScale = 0f;

         OnMultiplayerGamePased?.Invoke(this, EventArgs.Empty);
      }
      else
      {
         Time.timeScale = 1f;

         OnMultiplayerGameUnpased?.Invoke(this, EventArgs.Empty);
      }
   }

   private void State_OnValueChanged(State previousValue, State newValue)
   {
      OnStateChanged?.Invoke(this, EventArgs.Empty);
   }

   

   private void Update()
   {
      if(!IsServer) return;

      switch(_state.Value)
      {
         case State.WaitingToStart:
            break;
         case State.CountdownToStart:
         _countdownToStartTimer.Value -= Time.deltaTime;

            if(_countdownToStartTimer.Value < 0)
            {
               _state.Value = State.GamePlaying;
               _gamePlayingTimer.Value = _gamePlayingTimerMax;

            }
            break;
         case State.GamePlaying:
            _gamePlayingTimer.Value -= Time.deltaTime;

            if(_gamePlayingTimer.Value < 0)
            {
               
               _state.Value = State.GameOver;
            }
            break;
         case State.GameOver:
            break;
      }
   }



   private void LateUpdate()
   {
      if(autoTestGamePausedState)
      {
         autoTestGamePausedState = false;
         TestGamePauseState();
      }
   }



   public bool IsGamePlaying()
   {
      return _state.Value == State.GamePlaying;
   }

   public bool IsCountdownToStartActive()
   {
      return _state.Value == State.CountdownToStart;
   }

   public float GetCountdownToStartTimer()
   {
      return _countdownToStartTimer.Value;
   }

   public bool IsGameOver()
   {
      return _state.Value == State.GameOver;
   }

   public float GetGamePlayingTimerNormalized()
   {
      return 1 - (_gamePlayingTimer.Value/_gamePlayingTimerMax);
   }

   public bool IsGamePaused()
   {
      return _isGamePaused.Value;
   }

   public bool IsLocalPlayerReady()
   {
      return _isLocalPlayerReady;
   }

   public bool IsWaitingToStart()
   {
      return _state.Value == State.WaitingToStart;
   }
}
