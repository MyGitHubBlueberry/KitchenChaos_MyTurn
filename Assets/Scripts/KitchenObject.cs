using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenObject : NetworkBehaviour
{
   [SerializeField] private KitchenObjectSO _kitchenObjectSO;


   private IKitchenObjectParent _kitchenObjectParent;
   private FollowTransform _followTransform;


   protected virtual void Awake()
   {
      _followTransform = GetComponent<FollowTransform>();
   }

   public void SetKitchenObjectParent(IKitchenObjectParent kitchenObjectParent)
   {
      SetKitchenObjectParentServerRpc(kitchenObjectParent.GetNetworkObject());
   }

   [ServerRpc(RequireOwnership = false)]
   private void SetKitchenObjectParentServerRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
   {  
      kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
      IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

      if(!kitchenObjectParent.HasKitchenObject())
         SetKitchenObjectParentClientRpc(kitchenObjectParentNetworkObjectReference);
   }

   [ClientRpc]
   private void SetKitchenObjectParentClientRpc(NetworkObjectReference kitchenObjectParentNetworkObjectReference)
   {
      kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
      IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

      if(_kitchenObjectParent != null) _kitchenObjectParent.ClearKitchenObject();

      _kitchenObjectParent = kitchenObjectParent;

      _kitchenObjectParent.SetKitchenObject(this);
      
      _followTransform.SetTargetTransform(_kitchenObjectParent.GetKitchenObjectTargetPosition());
   }

   public KitchenObjectSO GetKitchenObjectSO()
   {
      return _kitchenObjectSO;
   }

   public void DestroyItSelf()
   {
      Destroy(gameObject);
   }

   public void ClearKitchenObjectOnParent()
   {
      _kitchenObjectParent.ClearKitchenObject();
   }
   
   public bool TryGetPlate(out PlateKitchenObject plateKitchenObject )
   {
      if(this is PlateKitchenObject)
      {
         plateKitchenObject = this as PlateKitchenObject;
         return true;
      }
      plateKitchenObject = null;
      return false;
   }




   public static void SpawnKitchenObject(KitchenObjectSO kitchenObjectSO, IKitchenObjectParent iKitchenObjectParent)
   {
      GameMultiplayer.Instance.SpawnKitchenObject(kitchenObjectSO, iKitchenObjectParent);
   }

   public static void DestroyKitchenObject(KitchenObject kitchenObject)
   {
      GameMultiplayer.Instance.DestroyKitchenObject(kitchenObject);
   }
}
