using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameOverNetworkObject : NetworkBehaviour
{
   private void Awake()
   {
      GameOverUI.Instance.OnMultiplayerGameOver += GameOverUI_OnMultiplayerGameOver;
   }



   private void GameOverUI_OnMultiplayerGameOver(object sender, EventArgs e)
   {
      UpdateVisualMultiplayerServerRpc();
   }

   [ServerRpc(RequireOwnership = false)]
   private void UpdateVisualMultiplayerServerRpc()
   {
      PlayerData[] playerDataArray = new PlayerData[NetworkManager.Singleton.ConnectedClients.Count];
      
      for(int playerIndex = 0; playerIndex < playerDataArray.Length; playerIndex++)
      {
         playerDataArray[playerIndex] = GameMultiplayer.Instance.GetPlayerDataFromPlayerIndex(playerIndex);
      }

      UpdateVisualMultiplayerClientRpc(playerDataArray);
      
   }

   [ClientRpc]
   private void UpdateVisualMultiplayerClientRpc(PlayerData[] playerDataArray)
   {
      GameOverUI.Instance.UpdateVisualMultiplayer(playerDataArray);
   }
}
