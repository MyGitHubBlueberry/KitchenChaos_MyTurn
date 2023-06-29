using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using Unity.Netcode;

public class GameOverUI : MonoBehaviour
{
   public static GameOverUI Instance {get; private set;}



   public event EventHandler OnMultiplayerGameOver;



   [SerializeField] private Transform _recipesDeliveredSingleplayer;
   [SerializeField] private TextMeshProUGUI _totalRecipesAmountTextSingleplayer;
   [SerializeField] private TextMeshProUGUI _totalPointsTextSingleplayer;

   [SerializeField] private Transform _recipesDeliveredMultiplayer;
   [SerializeField] private Transform _playerGameStatsContainerMultiplayer;
   [SerializeField] private Transform _playerGameStatsTemplateMultiplayer;
   [SerializeField] private TextMeshProUGUI _totalRecipesAmountTextMultiplayer;
   [SerializeField] private TextMeshProUGUI _totalPointsTextMultiplayer;



   [SerializeField] private Button _playAgainButton;




   private void Awake()
   {
      Instance = this;



      _playAgainButton.onClick.AddListener(() =>
      {
         NetworkManager.Singleton.Shutdown();
         Loader.Load(Loader.Scene.MainMenuScene);
      });

      _playerGameStatsTemplateMultiplayer.gameObject.SetActive(false);
   }

   private void Start()
   {
      GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

      Hide();
   }


   private void GameManager_OnStateChanged(object sender, EventArgs e)
   {
      if(GameManager.Instance.IsGameOver())
      {
         ShowAndUpdate(GameMultiplayer.PlayMultiplayer);
      }
      else
      {
         Hide();
      }
   }
   

   private void UpdateVisualSingleplayer()
   {
      _totalPointsTextSingleplayer.text = DeliveryManager.Instance.GetTotalPoints().ToString();
      _totalRecipesAmountTextSingleplayer.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
   }

   private void SetGlogalGameStatsMultiplayer()
   {
      _totalPointsTextMultiplayer.text = DeliveryManager.Instance.GetTotalPoints().ToString();
      _totalRecipesAmountTextMultiplayer.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
   }

   public void UpdateVisualMultiplayer(PlayerData[] playerDataArray)
   {
      foreach(Transform child in _playerGameStatsContainerMultiplayer)
         if(child != _playerGameStatsTemplateMultiplayer) Destroy(child.gameObject);

      for(int playerIndex = 0; playerIndex < playerDataArray.Length; playerIndex++)
      {
         Transform playerGameStatsTransform = Instantiate(_playerGameStatsTemplateMultiplayer, _playerGameStatsContainerMultiplayer);
         playerGameStatsTransform.gameObject.SetActive(true);

         bool isYourPlayer = playerDataArray[playerIndex].Equals(GameMultiplayer.Instance.GetPlayerData());

         GameOverSingleUI gameOverSingleUI = playerGameStatsTransform.GetComponent<GameOverSingleUI>();
         gameOverSingleUI.SetPlayerGameStats(playerDataArray[playerIndex], isYourPlayer);
      }
   }


   private void ShowAndUpdate(bool isMultiplayer)
   {
      gameObject.SetActive(true);

      _recipesDeliveredSingleplayer.gameObject.SetActive(!isMultiplayer);
      _recipesDeliveredMultiplayer.gameObject.SetActive(isMultiplayer);

      if(isMultiplayer)
      {
         OnMultiplayerGameOver?.Invoke(this, EventArgs.Empty);
         SetGlogalGameStatsMultiplayer();
      }
      else
      {
         UpdateVisualSingleplayer();
      }

      _playAgainButton.Select();
   }

   private void Hide()
   {
      _recipesDeliveredSingleplayer.gameObject.SetActive(false);
      _recipesDeliveredMultiplayer.gameObject.SetActive(false);


      gameObject.SetActive(false);
   }
}
