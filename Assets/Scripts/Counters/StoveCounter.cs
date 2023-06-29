using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
   public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
   public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
   public class OnStateChangedEventArgs : EventArgs
   {
      public readonly State state;

      public OnStateChangedEventArgs(State state)
      {
         this.state = state;
      }
   }

   public enum State
   {
      Idle,
      Frying,
      Fryed,
      Burned
   }


   [SerializeField] private FryingRecipeSO[] _fryingRecipeSOArray;
   [SerializeField] private BurningRecipeSO[] _burningRecipeSOArray;


   private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
   private FryingRecipeSO _fryingRecipeSO;
   private BurningRecipeSO _burningRecipeSO;
   private NetworkVariable<float> _fryingTimer = new NetworkVariable<float>(0f);
   private  NetworkVariable<float> _burningTimer = new NetworkVariable<float>(0f);


   public override void OnNetworkSpawn()
   {
      _fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
      _burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
      _state.OnValueChanged += State_OnValueChanged;
   }

   private void FryingTimer_OnValueChanged(float previousValue, float newValue)
   {
      float fryingTimerMax = _fryingRecipeSO != null ? _fryingRecipeSO.GetFryingTimerMax : 1f;

      OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
      (
         progressNormalized: _fryingTimer.Value/fryingTimerMax
      ));
   }

   private void BurningTimer_OnValueChanged(float previousValue, float newValue)
   {
      float burningTimerMax = _burningRecipeSO != null ? _burningRecipeSO.GetBurningTimerMax : 1f;

      OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
      (
         progressNormalized: _burningTimer.Value/burningTimerMax
      ));
   }

   private void State_OnValueChanged(State previousValue, State newValue)
   {
      OnStateChanged?.Invoke(this, new OnStateChangedEventArgs(_state.Value));

      if(_state.Value == State.Burned || _state.Value == State.Idle)
      {
         OnProgressChanged.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
         (
            progressNormalized: 0f
         ));
      }
   }


   private void Update()
   {
      if(!IsServer) return;

      if(HasKitchenObject())
      {
         switch(_state.Value)
         {
            case State.Idle:
               break;
            case State.Frying:
               _fryingTimer.Value += Time.deltaTime;

               if(_fryingTimer.Value >= _fryingRecipeSO.GetFryingTimerMax)
               {
                  KitchenObject.DestroyKitchenObject(GetKitchenObject());
      
                  KitchenObject.SpawnKitchenObject(_fryingRecipeSO.GetOutputKitchenObjectSO, this);

                  _state.Value = State.Fryed;
                  _burningTimer.Value = 0f;
                  
                  SetBurningRecipeSOClientRpc
                  (
                     GameMultiplayer.Instance.GetKitchenObjectSOIndex(GetKitchenObject().GetKitchenObjectSO())
                  );

                  _burningRecipeSO = GetBurningRecipeWithInput(_fryingRecipeSO.GetOutputKitchenObjectSO);
               }     
               break;
            case State.Fryed:
               _burningTimer.Value += Time.deltaTime;

               if(_burningTimer.Value >= _burningRecipeSO.GetBurningTimerMax)
               {
                  KitchenObject.DestroyKitchenObject(GetKitchenObject());

                  KitchenObject.SpawnKitchenObject(_burningRecipeSO.GetOutputKitchenObjectSO, this);

                  _state.Value = State.Burned;
               }
               break;
            case State.Burned:
               break;
         }
             
      }
   }

   public override void Interact(Player player)
   {
      if(!HasKitchenObject())
      {
         if(player.HasKitchenObject())
         {
            if(HasFryingRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO()))
            {
               KitchenObject kitchenObject = player.GetKitchenObject();
               kitchenObject.SetKitchenObjectParent(this);

               InteractLogicPlaceObjectOnCounterServerRpc(
                  GameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObject.GetKitchenObjectSO()
               ));
            }
         }  
      }
      else
      {
         if(!player.HasKitchenObject())
         {
            GetKitchenObject().SetKitchenObjectParent(player);
            
            SetStateIdleServerRpc();
         }  
         else if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
         {
            if(plateKitchenObject.TryAddIngridient(GetKitchenObject().GetKitchenObjectSO()))
            {
               KitchenObject.DestroyKitchenObject(GetKitchenObject());

               SetStateIdleServerRpc();
            }
         }  
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void SetStateIdleServerRpc()
   {
      _state.Value = State.Idle;
   }

   [ServerRpc(RequireOwnership = false)]
   private void InteractLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
   {
      _fryingTimer.Value = 0f;
      _state.Value = State.Frying;

      SetFryingRecipeSOClientRpc(kitchenObjectSOIndex);
   }

   [ClientRpc]
   private void SetFryingRecipeSOClientRpc(int kitchenObjectSOIndex)
   {
      _fryingRecipeSO = GetFryingRecipeWithInput(GameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex));
   }

   [ClientRpc]
   private void SetBurningRecipeSOClientRpc(int kitchenObjectSOIndex)
   {
      _burningRecipeSO = GetBurningRecipeWithInput(GameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex));
   }

   private bool HasFryingRecipeWithInput(KitchenObjectSO kitchenObjectSO)
   {
      return GetFryingRecipeWithInput(kitchenObjectSO) != null;
   }

   private FryingRecipeSO GetFryingRecipeWithInput(KitchenObjectSO kitchenObjectSO)
   {
      foreach(FryingRecipeSO fryingRecipeSO in _fryingRecipeSOArray)
         if(fryingRecipeSO.GetInputKitchenObjectSO == kitchenObjectSO)
            return fryingRecipeSO;
      return null;
   }

   private BurningRecipeSO GetBurningRecipeWithInput(KitchenObjectSO kitchenObjectSO)
   {
      foreach(BurningRecipeSO burningRecipeSO in _burningRecipeSOArray)
         if(burningRecipeSO.GetInputKitchenObjectSO == kitchenObjectSO)
            return burningRecipeSO;
      return null;
   }

   public bool IsFryed()
   {
      return _state.Value == State.Fryed;
   }
}
