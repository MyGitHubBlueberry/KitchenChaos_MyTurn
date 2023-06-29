using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectPlayer : MonoBehaviour
{
   [SerializeField] private int _playerIndex;
   [SerializeField] private GameObject _readyGameObject;
   [SerializeField] private PlayerVisual _playerVisual;
   [SerializeField] private Button _kickButton;
   [SerializeField] private TextMeshPro _playerNameText;



   private void Awake()
   {
      _kickButton.onClick.AddListener( () =>
      {
         PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);

         GameLobby.Instance.KickPlayer(playerData.playerId.ToString());
         GameMultiplayer.Instance.KickPlayer(playerData.clientId);
      });   
   }

   private void Start()
   {
      GameMultiplayer.Instance.OnPlayerDataNetworkListChanged += GameMultiplayer_OnPlayerDataNetworkListChanged;
      CharacterSelectReady.Instance.OnReadyChanged += CharacterSelectReady_OnReadyChanged;

      _kickButton.gameObject.SetActive(NetworkManager.Singleton.IsServer && _playerIndex != 0);

      UpdatePlayer();
   }

   private void CharacterSelectReady_OnReadyChanged(object sender, EventArgs e)
   {
      UpdatePlayer();
   }

   private void GameMultiplayer_OnPlayerDataNetworkListChanged(object sender, EventArgs e)
   {
      UpdatePlayer();
   }



   private void UpdatePlayer()
   {
      if(GameMultiplayer.Instance.IsPlayerIndexConnected(_playerIndex))
      {
         Show();
         
         PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(_playerIndex);

         _readyGameObject.SetActive(CharacterSelectReady.Instance.IsPlayerReady(playerData.clientId));

         _playerVisual.SetPlayerColor(GameMultiplayer.Instance.GetPlayerColor(playerData.colorId));

         _playerNameText.text = playerData.playerName.ToString();
      }
      else
      {
         Hide();
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

   

   private void OnDestroy()
   {
      GameMultiplayer.Instance.OnPlayerDataNetworkListChanged -= GameMultiplayer_OnPlayerDataNetworkListChanged;
   }
}
