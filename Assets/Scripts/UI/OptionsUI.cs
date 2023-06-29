using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class OptionsUI : MonoBehaviour
{
   public static OptionsUI Instance {get; private set;}


   [SerializeField] private Button _soundEffectsButton;
   [SerializeField] private Button _musicButton;
   [SerializeField] private Button _closeButton;
   [SerializeField] private Button _moveUpBindingButton;
   [SerializeField] private Button _moveDownBindingButton;
   [SerializeField] private Button _moveLeftBindingButton;
   [SerializeField] private Button _moveRightBindingButton;
   [SerializeField] private Button _interactBindingButton;
   [SerializeField] private Button _interactAltBindingButton;
   [SerializeField] private Button _pauseBindingButton;
   [SerializeField] private Button _gamepadInteractBindingButton;
   [SerializeField] private Button _gamepadInteractAltBindingButton;
   [SerializeField] private Button _gamepadPauseBindingButton;



   [SerializeField] private TextMeshProUGUI _soundEffectButtonText;
   [SerializeField] private TextMeshProUGUI _musicButtonText;
   [SerializeField] private TextMeshProUGUI _moveUpBindingButtonText;
   [SerializeField] private TextMeshProUGUI _moveDownBindingButtonText;
   [SerializeField] private TextMeshProUGUI _moveLeftBindingButtonText;
   [SerializeField] private TextMeshProUGUI _moveRightBindingButtonText;
   [SerializeField] private TextMeshProUGUI _interactBindingButtonText;
   [SerializeField] private TextMeshProUGUI _interactAltBindingButtonText;
   [SerializeField] private TextMeshProUGUI _pauseBindingButtonText;
   [SerializeField] private TextMeshProUGUI _gamepadInteractBindingButtonText;
   [SerializeField] private TextMeshProUGUI _gamepadInteractAltBindingButtonText;
   [SerializeField] private TextMeshProUGUI _gamepadPauseBindingButtonText;

   [SerializeField] private GameObject _pressToRebindKeyGameObject;


   private Action _onCloseButtonAction;


   private void Awake()
   {
      Instance = this;

      _soundEffectsButton.onClick.AddListener(() => 
      {
         SoundManager.Instance.ChangeVolume();
         UpdateVisual();
      });

      _musicButton.onClick.AddListener(() =>
      {
         MusicManager.Instance.ChangeVolume();
         UpdateVisual();
      });

      _closeButton.onClick.AddListener(() =>
      {
         Hide();
         _onCloseButtonAction();
      });

      _moveUpBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Up));
      _moveDownBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Down));
      _moveLeftBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Left));
      _moveRightBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Move_Right));
      _interactBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Interact));
      _interactAltBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.InteractAlternative));
      _pauseBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Pause));

      _gamepadInteractBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_Interact));
      _gamepadInteractAltBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_InteractAlternative));
      _gamepadPauseBindingButton.onClick.AddListener(() => RebindBinding(GameInput.Binding.Gamepad_Pause));
   } 

   private void Start()
   {
      GameManager.Instance.OnLocalGameUnpaused += GameManager_OnLocalGameUnpaused;

      UpdateVisual();

      HidePressToRebindKey();
      Hide();
   }

   private void GameManager_OnLocalGameUnpaused(object sender, EventArgs e)
   {
      Hide();
   }

   private void UpdateVisual()
   {
      _soundEffectButtonText.text = "Sound Effects: " + Mathf.Round(SoundManager.Instance.GetVolume() * 10f);
      _musicButtonText.text = "Music: " + Mathf.Round(MusicManager.Instance.GetVolume() * 10f);

      _moveUpBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Up);
      _moveDownBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Down);
      _moveLeftBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Left);
      _moveRightBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Move_Right);
      _interactBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Interact);
      _interactAltBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.InteractAlternative);
      _pauseBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Pause);

      _gamepadInteractBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Interact);
      _gamepadInteractAltBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_InteractAlternative);
      _gamepadPauseBindingButtonText.text = GameInput.Instance.GetBindingText(GameInput.Binding.Gamepad_Pause);
   }

   public void Show(Action onCloseButtonAction)
   {
      _onCloseButtonAction = onCloseButtonAction;

      gameObject.SetActive(true);
   
      _soundEffectsButton.Select();
   }

   private void Hide()
   {
      gameObject.SetActive(false);
   }

   private void ShowPressToRebindKey()
   {
      _pressToRebindKeyGameObject.SetActive(true);
   }

   private void HidePressToRebindKey()
   {
      _pressToRebindKeyGameObject.SetActive(false);
   }

   private void RebindBinding(GameInput.Binding binding)
   {
      ShowPressToRebindKey();
      GameInput.Instance.RebindBinding(binding, () => 
      {
         HidePressToRebindKey();
         UpdateVisual();
      });
   }
}
 