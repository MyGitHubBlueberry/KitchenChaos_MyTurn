using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyListSingleUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI _lobbyNameText;


   private Lobby _lobby;



   private void Awake()
   {
      GetComponent<Button>().onClick.AddListener(() =>
      {
         GameLobby.Instance.JoinById(_lobby.Id);
      });
   }

   public void SetLobby(Lobby lobby)
   {
      _lobby = lobby;
      _lobbyNameText.text = _lobby.Name;
   }
}
