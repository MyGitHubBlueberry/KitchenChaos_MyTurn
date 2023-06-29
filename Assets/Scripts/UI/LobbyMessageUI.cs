using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using Unity.Netcode;

public class LobbyMessageUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI _messageText;
   [SerializeField] private Button  _closeButton;


   private void Awake()
   {
      _closeButton.onClick.AddListener(Hide);
   }



   private void Start()
   {
      GameMultiplayer.Instance.OnFaleToJoinGame += GameMultiplayer_OnFaleToJoinGame;
      GameLobby.Instance.OnCreateLobbyStarted += GameLobby_OnCreateLobbyStarted;
      GameLobby.Instance.OnCreateLobbyFailed += GameLobby_OnCreateLobbyFailed;
      GameLobby.Instance.OnCreateLobbyFailedNullArgument += GameLobby_OnCreateLobbyFailedNullArgument;
      GameLobby.Instance.OnJoinStarted += GameLobby_OnJoinStarted;
      GameLobby.Instance.OnJoinFailed += GameLobby_OnJoinFailed;
      GameLobby.Instance.OnJoinFailedNullArgument += GameLobby_OnJoinFailedNullArgument;
      GameLobby.Instance.OnQuickJoinFailed += GameLobby_OnQuickJoinFailed;
      

      Hide();
   }

   private void GameLobby_OnJoinFailedNullArgument(object sender, EventArgs e)
   {
      ShowMessage("Failed to join Lobby!\nEnter Lobby code before joining!");
   }

   private void GameLobby_OnCreateLobbyFailedNullArgument(object sender, EventArgs e)
   {
      ShowMessage("Failed to create Lobby!\nEnter Lobby name before creating!");
   }

   private void GameLobby_OnJoinStarted(object sender, EventArgs e)
   {
      ShowMessage("Joining Lobby...");
   }

   private void GameLobby_OnJoinFailed(object sender, EventArgs e)
   {
      ShowMessage("Failed to join Lobby!");
   }

   private void GameLobby_OnQuickJoinFailed(object sender, EventArgs e)
   {
      ShowMessage("No Lobbies available for Quick Join found");
   }

   private void GameLobby_OnCreateLobbyStarted(object sender, EventArgs e)
   {
      ShowMessage("Creating Lobby...");
   }

   private void GameLobby_OnCreateLobbyFailed(object sender, EventArgs e)
   {
      ShowMessage("Failed to create Lobby!");
   }

   private void GameMultiplayer_OnFaleToJoinGame(object sender, EventArgs e)
   {
      if(NetworkManager.Singleton.DisconnectReason == "")
      {
         ShowMessage("Failed to connect");
      }
      else
      {
         ShowMessage(NetworkManager.Singleton.DisconnectReason);
      }
   }



   private void Show()
   {
      gameObject.SetActive(true);
   }

   private void Hide()
   {
      gameObject.SetActive(false);
   }

   private void ShowMessage(string messageText)
   {
      Show();

      _messageText.text = messageText;
   }



   private void OnDestroy()
   {
      GameMultiplayer.Instance.OnFaleToJoinGame -= GameMultiplayer_OnFaleToJoinGame;
      GameLobby.Instance.OnCreateLobbyStarted -= GameLobby_OnCreateLobbyStarted;
      GameLobby.Instance.OnCreateLobbyFailed -= GameLobby_OnCreateLobbyFailed;
      GameLobby.Instance.OnCreateLobbyFailedNullArgument -= GameLobby_OnCreateLobbyFailedNullArgument;
      GameLobby.Instance.OnJoinStarted -= GameLobby_OnJoinStarted;
      GameLobby.Instance.OnJoinFailed -= GameLobby_OnJoinFailed;
      GameLobby.Instance.OnJoinFailedNullArgument -= GameLobby_OnJoinFailedNullArgument;
      GameLobby.Instance.OnQuickJoinFailed -= GameLobby_OnQuickJoinFailed;
   }
}