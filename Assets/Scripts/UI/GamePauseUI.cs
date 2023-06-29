using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour
{
   [SerializeField] private Button _pauseButton;
   [SerializeField] private Button _resumeButton;
   [SerializeField] private Button _optionsButton;
   [SerializeField] private Button _mainMenuButton;
   [SerializeField] private GameObject _pauseMenu;


   private void Awake()
   {
      _pauseButton.onClick.AddListener(() =>
      {
         GameManager.Instance.TogglePauseGame();
      });

      _resumeButton.onClick.AddListener(() =>
      {
         GameManager.Instance.TogglePauseGame();
      });

      _optionsButton.onClick.AddListener(() => 
      {
         OptionsUI.Instance.Show(ShowPauseMenu);
         HidePauseMenu();
      });

      _mainMenuButton.onClick.AddListener(() =>
      {
         NetworkManager.Singleton.Shutdown();
         Loader.Load(Loader.Scene.MainMenuScene);
      });
   }

   private void Start()
   {
      GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
      GameManager.Instance.OnLocalGameUnpaused += GameManager_OnLocalGameUnpaused;


      HidePauseMenu();
   }

   private void GameManager_OnLocalGamePaused(object sender, EventArgs e)
   {
      ShowPauseMenu();
   }

   private void GameManager_OnLocalGameUnpaused(object sender, EventArgs e)
   {
      HidePauseMenu();
      _pauseButton.gameObject.SetActive(true);
      _pauseButton.Select(); 
   }


   private void ShowPauseMenu()
   {
      _pauseMenu.SetActive(true);

      _resumeButton.Select();
      _pauseButton.gameObject.SetActive(false);
   }

   private void HidePauseMenu()
   {
      _pauseMenu.SetActive(false);
   }
}
