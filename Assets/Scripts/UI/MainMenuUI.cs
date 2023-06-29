using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
   [SerializeField] private Button _playMultiplayerButton;
   [SerializeField] private Button _playSingleplayerButton;
   [SerializeField] private Button _quitButton;

   
   private void Awake()
   {
      _playMultiplayerButton.onClick.AddListener(() =>
      {
         GameMultiplayer.PlayMultiplayer = true;
         Loader.Load(Loader.Scene.LobbyScene);
      });

      _playSingleplayerButton.onClick.AddListener(() =>
      {
         GameMultiplayer.PlayMultiplayer = false;
         Loader.Load(Loader.Scene.LobbyScene);
      });

      _quitButton.onClick.AddListener(() =>
      {
         Application.Quit();  
      });

      Time.timeScale = 1f;
   }

}
