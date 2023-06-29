using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Lobbies.Models;
using System;

public class LobbyUI : MonoBehaviour
{
   [SerializeField] private Button _mainMenuButton;
   [SerializeField] private Button _createLobbyButton;
   [SerializeField] private Button _quickJoinButton;
   [SerializeField] private Button _joinByCodeButton;
   [SerializeField] private TMP_InputField _lobbyCodeInputField;
   [SerializeField] private TMP_InputField _playerNameInputField;
   [SerializeField] private LobbyCreateUI _lobbyCreateUI;
   [SerializeField] private Transform _lobbyContainer;
   [SerializeField] private Transform _lobbyTemplate;



   private void Awake()
   {
      _mainMenuButton.onClick.AddListener(() => 
      {
         GameLobby.Instance.LeaveLobby();
         Loader.Load(Loader.Scene.MainMenuScene);
      });

      _createLobbyButton.onClick.AddListener(() =>
      {
         _lobbyCreateUI.Show();
      });

      _quickJoinButton.onClick.AddListener(() =>
      {
         GameLobby.Instance.QuickJoin();
      });

      _joinByCodeButton.onClick.AddListener(() =>
      {
         GameLobby.Instance.JoinByCode(_lobbyCodeInputField.text);
      });

      _lobbyTemplate.gameObject.SetActive(false);
   }



   private void Start()
   {
      _playerNameInputField.text = GameMultiplayer.Instance.GetPlayerName();
      _playerNameInputField.onValueChanged.AddListener((string newPlayerName) =>
      {
         GameMultiplayer.Instance.SetPlayerName(newPlayerName);
      });

      GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
      UpdateLobbyList(new List<Lobby>());
   }

   private void GameLobby_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
   {
      UpdateLobbyList(e.GetLobbyList);
   }

   private void UpdateLobbyList(List<Lobby> lobbyList)
   {
      foreach(Transform child in _lobbyContainer)
         if(child != _lobbyTemplate) Destroy(child.gameObject);

      foreach(Lobby lobby in lobbyList)
      {
         Transform lobbyTransform = Instantiate(_lobbyTemplate, _lobbyContainer);
         lobbyTransform.gameObject.SetActive(true);

         lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
      }
   }


   private void OnDestroy()
   {
      GameLobby.Instance.OnLobbyListChanged -= GameLobby_OnLobbyListChanged;
   }
}
