using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IKitchenObjectParent
{
   public bool HasKitchenObject();
   public void ClearKitchenObject();
   public void SetKitchenObject(KitchenObject kitchenObject);
   public Transform GetKitchenObjectTargetPosition();
   public KitchenObject GetKitchenObject();
   public NetworkObject GetNetworkObject();
}