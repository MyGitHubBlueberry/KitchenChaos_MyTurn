using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class PlateKitchenObject : KitchenObject
{
   public event EventHandler<OnIngridientAddedEventArgs> OnIngridientAdded;
   public class OnIngridientAddedEventArgs : EventArgs
   {
      public readonly KitchenObjectSO KitchenObjectSO;
      public OnIngridientAddedEventArgs(KitchenObjectSO kitchenObjectSO)
      {
         KitchenObjectSO = kitchenObjectSO;
      }
   }


   [SerializeField] private PlateValidIngridientListSO _validIngridientList;


   private List<KitchenObjectSO> _kitchenObjectSOList;


   protected override void Awake()
   {
      base.Awake();
      _kitchenObjectSOList = new List<KitchenObjectSO>();
   }

   public bool TryAddIngridient(KitchenObjectSO kitchenObjectSO)
   {
      if(_validIngridientList.GetValidKithenObjectSOList.Contains(kitchenObjectSO))
      {
         if(!_kitchenObjectSOList.Contains(kitchenObjectSO))
         {
            int kitchenObjectSOIndex = GameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObjectSO);

            AddIngridientServerRpc(kitchenObjectSOIndex);
            
            return true;
         }
      }
      return false;
   }

   [ServerRpc(RequireOwnership = false)]
   private void AddIngridientServerRpc(int kitchenObjectSOIndex)
   {
      if(!_kitchenObjectSOList.Contains(GameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex)))
         AddIngridientClientRpc(kitchenObjectSOIndex);
   }

   [ClientRpc]
   private void AddIngridientClientRpc(int kitchenObjectSOIndex)
   {
      KitchenObjectSO kitchenObjectSO = GameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);

      _kitchenObjectSOList.Add(kitchenObjectSO);

      OnIngridientAdded?.Invoke(this, new OnIngridientAddedEventArgs(kitchenObjectSO));
   }
   public List<KitchenObjectSO> GetKitchenObjectSOList()
   {
      return _kitchenObjectSOList;
   }
}
