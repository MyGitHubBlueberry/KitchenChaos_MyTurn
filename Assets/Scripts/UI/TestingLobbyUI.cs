using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestingLobbyUI : MonoBehaviour
{
   [SerializeField] private Button _createGameButton;
   [SerializeField] private Button _joinGameButton;


   private void Awake()
   {
      _createGameButton.onClick.AddListener(() =>
      {
         GameMultiplayer.Instance.StartHost();
         Loader.LoadNetwork(Loader.Scene.CharacterSelectScene);     
      });

      _joinGameButton.onClick.AddListener(() =>
      {
         GameMultiplayer.Instance.StartClient();
      });
   }
}
