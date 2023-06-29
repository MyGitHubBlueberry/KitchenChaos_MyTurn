using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class TestingNetcodeUI : MonoBehaviour
{
   [SerializeField] private Button _startHostButton;
   [SerializeField] private Button _startClientButton;


   private void Awake()
   {
      _startHostButton.onClick.AddListener(() =>
      {
         Debug.Log("HOST");
         GameMultiplayer.Instance.StartHost();
         Hide();
      });

      _startClientButton.onClick.AddListener(() =>
      {
         Debug.Log("CLIENT");
         GameMultiplayer.Instance.StartClient();
         Hide();
      });
   }

   private void Hide()
   {
      gameObject.SetActive(false);
   }
}
