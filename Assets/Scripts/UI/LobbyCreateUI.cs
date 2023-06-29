using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyCreateUI : MonoBehaviour
{
   [SerializeField] private Button _closeButton;
   [SerializeField] private Button _createPublicButton;
   [SerializeField] private Button _createPrivateButton;
   [SerializeField] private TMP_InputField _lobbyNameInputField;



   private void Awake()
   {
      _createPublicButton.onClick.AddListener(() =>
      {
         GameLobby.Instance.CreateLobby(_lobbyNameInputField.text, false);
      });

      _createPrivateButton.onClick.AddListener(() =>
      {
         GameLobby.Instance.CreateLobby(_lobbyNameInputField.text, true);
      });

      _closeButton.onClick.AddListener(() =>
      {
         Hide();
      });
   }

   
   private void Start()
   {
      Hide();
   }



   public void Show()
   {
      gameObject.SetActive(true);

      _createPublicButton.Select();
   }

   private void Hide()
   {
      gameObject.SetActive(false);
   }
}
