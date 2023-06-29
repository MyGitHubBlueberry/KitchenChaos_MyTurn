using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
   private const string PLYAER_PREFS_BINDINGS = "InputBindings";


   public static GameInput Instance {get; private set;}


   public event EventHandler OnInteractAction;
   public event EventHandler OnIntearctAlternativeAction;
   public event EventHandler OnPauseAction;
   public event EventHandler OnBindingRebind;


   public enum Binding
   {
      Move_Up,
      Move_Down,
      Move_Left,
      Move_Right,
      Interact,
      InteractAlternative,
      Pause,
      Gamepad_Interact,
      Gamepad_InteractAlternative,
      Gamepad_Pause,
   }


   public Vector2 GetMovementVectorNormalized { get => _playerInputActions.Player.Move.ReadValue<Vector2>().normalized; }

   
   private PlayerInputActions _playerInputActions;


   private void Awake()
   {
      Instance = this;

      _playerInputActions = new PlayerInputActions();

      if(PlayerPrefs.HasKey(PLYAER_PREFS_BINDINGS))
      {
         _playerInputActions.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLYAER_PREFS_BINDINGS));
      }
      
      _playerInputActions.Player.Enable();
      
      _playerInputActions.Player.Interact.performed += Interact_performed;
      _playerInputActions.Player.InteractAlternative.performed += InteractAlternative_performed; 
      _playerInputActions.Player.Pause.performed += Pause_performed;
   }

   private void OnDestroy()
   {
      _playerInputActions.Player.Interact.performed -= Interact_performed;
      _playerInputActions.Player.InteractAlternative.performed -= InteractAlternative_performed; 
      _playerInputActions.Player.Pause.performed -= Pause_performed;

      _playerInputActions.Dispose();
   }

   private void Pause_performed(InputAction.CallbackContext obj)
   {
      OnPauseAction?.Invoke(this, EventArgs.Empty);
   }

   private void InteractAlternative_performed(InputAction.CallbackContext obj)
   {
      OnIntearctAlternativeAction?.Invoke(this, EventArgs.Empty);
   }

   private void Interact_performed(InputAction.CallbackContext obj)
   {
      OnInteractAction?.Invoke(this, EventArgs.Empty);
   }

   public bool IsInteractAlternativeInProgress()
   {
      return _playerInputActions.Player.InteractAlternative.IsInProgress();
   }

   public string GetBindingText(Binding binding)
   {
      switch(binding)
      {  
         default:
         case Binding.Interact:
            return _playerInputActions.Player.Interact.bindings[0].ToDisplayString();
         case Binding.InteractAlternative:
            return _playerInputActions.Player.InteractAlternative.bindings[0].ToDisplayString();
         case Binding.Pause:
            return _playerInputActions.Player.Pause.bindings[0].ToDisplayString();
         case Binding.Move_Up:
            return _playerInputActions.Player.Move.bindings[1].ToDisplayString();
         case Binding.Move_Down:
            return _playerInputActions.Player.Move.bindings[2].ToDisplayString();
         case Binding.Move_Left:
            return _playerInputActions.Player.Move.bindings[3].ToDisplayString();
         case Binding.Move_Right:
            return _playerInputActions.Player.Move.bindings[4].ToDisplayString();
         case Binding.Gamepad_Interact:
            return _playerInputActions.Player.Interact.bindings[1].ToDisplayString();
         case Binding.Gamepad_InteractAlternative:
            return _playerInputActions.Player.InteractAlternative.bindings[1].ToDisplayString();
         case Binding.Gamepad_Pause:
            return _playerInputActions.Player.Pause.bindings[1].ToDisplayString();
      }
   }

   public void RebindBinding(Binding binding,  Action onActionRebound)
   {
      _playerInputActions.Player.Disable();

      InputAction inputAction;
      int bindingIndex;

      switch(binding)
      {
         default:
         case Binding.Move_Up:
            inputAction = _playerInputActions.Player.Move;
            bindingIndex = 1;
            break;
         case Binding.Move_Down:
            inputAction = _playerInputActions.Player.Move;
            bindingIndex = 2;
            break;
         case Binding.Move_Left:
            inputAction = _playerInputActions.Player.Move;
            bindingIndex = 3;
            break;
         case Binding.Move_Right:
            inputAction = _playerInputActions.Player.Move;
            bindingIndex = 4;
            break;
         case Binding.Interact:
            inputAction = _playerInputActions.Player.Interact;
            bindingIndex = 0;
            break;
         case Binding.InteractAlternative:
            inputAction = _playerInputActions.Player.InteractAlternative;
            bindingIndex = 0;
            break;
         case Binding.Pause:
            inputAction = _playerInputActions.Player.Pause;
            bindingIndex = 0;
            break;
         case Binding.Gamepad_Interact:
            inputAction = _playerInputActions.Player.Interact;
            bindingIndex = 1;
            break;
         case Binding.Gamepad_InteractAlternative:
            inputAction = _playerInputActions.Player.InteractAlternative;
            bindingIndex = 1;
            break;
         case Binding.Gamepad_Pause:
            inputAction = _playerInputActions.Player.Pause;
            bindingIndex = 1;
            break;
      }

      inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback => 
      {
         callback.Dispose(); 
         _playerInputActions.Player.Enable();
         onActionRebound();

         PlayerPrefs.SetString(PLYAER_PREFS_BINDINGS, _playerInputActions.SaveBindingOverridesAsJson());
         PlayerPrefs.Save();

         OnBindingRebind?.Invoke(this, EventArgs.Empty);
      }).Start();
   }
}
