using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI _moveKeyUpText;
   [SerializeField] private TextMeshProUGUI _moveKeyLeftText;
   [SerializeField] private TextMeshProUGUI _moveKeyDownText;
   [SerializeField] private TextMeshProUGUI _moveKeyRightText;
   [SerializeField] private TextMeshProUGUI _interactKeyText;
   [SerializeField] private TextMeshProUGUI _interactAltKeyText;
   [SerializeField] private TextMeshProUGUI _pauseKeyText;

   [SerializeField] private TextMeshProUGUI _gamepadInteractKeyText;
   [SerializeField] private TextMeshProUGUI _gamepadInteractAltKeyText;
   [SerializeField] private TextMeshProUGUI _gamepadPauseKeyText;


   private void Start()
   {
      GameInput.Instance.OnBindingRebind += GameInput_OnBindingRebind;
      GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerReadyChanged;
      
      UpdateVisual();

      Show();
   }

   private void GameManager_OnLocalPlayerReadyChanged(object sender, EventArgs e)
   {
      if(GameManager.Instance.IsLocalPlayerReady())
         Hide();
   }

   private void GameInput_OnBindingRebind(object sender, EventArgs e)
   {
      UpdateVisual();
   }

   private void UpdateVisual()
   {
      _moveKeyUpText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
      _moveKeyDownText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
      _moveKeyLeftText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
      _moveKeyRightText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
      _interactKeyText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
      _interactAltKeyText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternative);
      _pauseKeyText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.Pause);
      _gamepadInteractKeyText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
      _gamepadInteractAltKeyText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternative);
      _gamepadPauseKeyText.text =  GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
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
