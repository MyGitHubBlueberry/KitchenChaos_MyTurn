using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectingUI : MonoBehaviour
{

   private void Start()
   {
      GameMultiplayer.Instance.OnTryingToJoinGame += GameMultiplayer_OnTryingToJoinGame;
      GameMultiplayer.Instance.OnFaleToJoinGame += GameMultiplayer_OnFaleToJoinGame;

      Hide();
   }

   private void OnDestroy()
   {
      GameMultiplayer.Instance.OnTryingToJoinGame -= GameMultiplayer_OnTryingToJoinGame;
      GameMultiplayer.Instance.OnFaleToJoinGame -= GameMultiplayer_OnFaleToJoinGame;
   }

   private void GameMultiplayer_OnFaleToJoinGame(object sender, EventArgs e)
   {
      Hide();
   }

   private void GameMultiplayer_OnTryingToJoinGame(object sender, EventArgs e)
   {
      Show();
   }



   private void Show()
   {
      gameObject.SetActive(true);
   }

   private void Hide()
   {
      gameObject.SetActive(false);
   }
}
