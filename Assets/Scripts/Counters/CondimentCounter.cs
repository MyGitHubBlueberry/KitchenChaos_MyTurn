using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CondimentCounter : BaseCounter, IInteractAlternative, IHasProgress
{
   public static event EventHandler OnAnyCondimentIsUsing;

   new public static void ResetStaticData()
   {
      OnAnyCondimentIsUsing = null;
   }

   public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
   public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
   public class OnStateChangedEventArgs : EventArgs
   {
      public readonly State GetState;
      public OnStateChangedEventArgs(State state)
      {
         GetState = state;
      }
   }


   [SerializeField] private KitchenObjectSO _condimentKitchenObjectSO;
   [SerializeField] private float _condimentUseTimerMax;
   

   public enum State
   {
      Idle,
      UsingCondiment,
      CondimentIsUsed,
   }


   private NetworkVariable<float> _condimentUseTimer = new NetworkVariable<float>(0f);
   private NetworkVariable<float> _condimentUsingSoundTimer = new NetworkVariable<float>(0f);
   private float _condimentUsingSoundLength = 1f;

   private NetworkVariable<State> _state = new NetworkVariable<State>(State.Idle);
   private PlateKitchenObject _plateKitchenObject;


   public override void OnNetworkSpawn()
   {
      _condimentUseTimer.OnValueChanged += CondimentUseTimer_OnValueChanged;
      _state.OnValueChanged += State_OnValueChanged;
   }

   private void State_OnValueChanged(State previousValue, State newValue)
   {
      OnStateChanged.Invoke(this, new OnStateChangedEventArgs(_state.Value));

      if(_state.Value == State.Idle || _state.Value == State.CondimentIsUsed)
      {
         OnProgressChanged.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
         (
            progressNormalized: 0f
         ));
      }
   }

   private void CondimentUseTimer_OnValueChanged(float previousValue, float newValue)
   {
      OnProgressChanged.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
      (
         progressNormalized: _condimentUseTimer.Value/_condimentUseTimerMax
      ));
   }

   private void Update()
   {
      if(!IsServer) return;

      switch(_state.Value)
      {
         case State.Idle:
            break;
         case State.UsingCondiment:
               _condimentUseTimer.Value += Time.deltaTime;
               _condimentUsingSoundTimer.Value += Time.deltaTime;

               if(_condimentUsingSoundTimer.Value >= _condimentUsingSoundLength)
               {
                  _condimentUsingSoundTimer.Value = 0f;
                  OnAnyCondimentIsUsing?.Invoke(this, EventArgs.Empty);
               }


               if(_condimentUseTimer.Value >= _condimentUseTimerMax)
               {
                  _condimentUseTimer.Value = 0f;

                  _plateKitchenObject.TryAddIngridient(_condimentKitchenObjectSO);

                  _state.Value = State.CondimentIsUsed;
               }
            break;
         case State.CondimentIsUsed:
            break;
      }
   }

   public override void Interact(Player player)
   {
      if(!HasKitchenObject())
      {
         if(player.HasKitchenObject())
         {
            player.GetKitchenObject().SetKitchenObjectParent(this);

            InteractLogicPlaceObjectOnCounterServerRpc();
         }
      }
      else
      {
         if(player.HasKitchenObject())
         {
            if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
               if(plateKitchenObject.TryAddIngridient(GetKitchenObject().GetKitchenObjectSO()))
               {
                  KitchenObject.DestroyKitchenObject(GetKitchenObject());
               }
            }
            else if(GetKitchenObject().TryGetPlate(out plateKitchenObject))
            {
               if(plateKitchenObject.TryAddIngridient(player.GetKitchenObject().GetKitchenObjectSO()))
               {
                  KitchenObject.DestroyKitchenObject(player.GetKitchenObject());
               }
            }
         }  
         else
         {
            GetKitchenObject().SetKitchenObjectParent(player);

            SetStateIdleServerRpc();
         }
      }
   }


   [ServerRpc(RequireOwnership = false)]
   private void InteractLogicPlaceObjectOnCounterServerRpc()
   {
      if(!HasKitchenObject()) return;

      _condimentUseTimer.Value = 0f;
      _condimentUsingSoundTimer.Value = 0f;
   }

   [ServerRpc(RequireOwnership = false)]
   private void SetStateIdleServerRpc()
   {
      _state.Value = State.Idle;
   }

   public void InteractAlternative(Player player)
   {
      if(HasKitchenObject() && !player.HasKitchenObject() && _state.Value != State.UsingCondiment)
      {
         if(GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
         {
            if(!plateKitchenObject.GetKitchenObjectSOList().Contains(_condimentKitchenObjectSO))
            {
               InteractAlternativeLogicServerRpc(plateKitchenObject.NetworkObject);
            }
         }
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void InteractAlternativeLogicServerRpc(NetworkObjectReference plateKitchenObjectNetworkObjectReference)
   {
      _state.Value = State.UsingCondiment;
      
      SetPlateKitchenObjectClientRpc(plateKitchenObjectNetworkObjectReference);
   }

   [ClientRpc]
   private void SetPlateKitchenObjectClientRpc(NetworkObjectReference plateKitchenObjectNetworkObjectReference)
   {
      plateKitchenObjectNetworkObjectReference.TryGet(out NetworkObject plateKitchenObjectNetworkObject);
      PlateKitchenObject plateKitchenObject = plateKitchenObjectNetworkObject.GetComponent<PlateKitchenObject>();

      _plateKitchenObject = plateKitchenObject;
   }
}
