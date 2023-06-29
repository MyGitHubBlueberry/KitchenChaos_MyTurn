using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMultiplayerUI : MonoBehaviour
{
   private void Start()
   {
      GameManager.Instance.OnMultiplayerGamePased+= GameManager_OnMultiplayerGamePased;
      GameManager.Instance.OnMultiplayerGameUnpased+= GameManager_OnMultiplayerGameUnpased;

      Hide();
   }

   private void GameManager_OnMultiplayerGamePased(object sender, EventArgs e)
   {
      Show();
   }

   private void GameManager_OnMultiplayerGameUnpased(object sender, EventArgs e)
   {
      Hide();
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
