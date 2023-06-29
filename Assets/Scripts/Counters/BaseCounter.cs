using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class BaseCounter : NetworkBehaviour, IKitchenObjectParent
{
   public static event EventHandler OnAnyObjectPlacedHere;
    
   public static void ResetStaticData()
   {
      OnAnyObjectPlacedHere = null;
   }


   [SerializeField] protected Transform _counterTopPoint;


   private KitchenObject _kitchenObject;


   public abstract void Interact(Player player);

   public bool HasKitchenObject()
   {
      return _kitchenObject != null;
   }

   public virtual void ClearKitchenObject()
   {
      _kitchenObject = null;
   }

   public virtual void SetKitchenObject(KitchenObject kitchenObject)
   {
      _kitchenObject = kitchenObject;

      if(_kitchenObject!= null) OnAnyObjectPlacedHere?.Invoke(this, EventArgs.Empty);
   }

   public virtual Transform GetKitchenObjectTargetPosition()
   {
      return _counterTopPoint;
   }

   public virtual KitchenObject GetKitchenObject()
   {
      return _kitchenObject;
   }

   public NetworkObject GetNetworkObject()
   {
      return NetworkObject;
   }
}
