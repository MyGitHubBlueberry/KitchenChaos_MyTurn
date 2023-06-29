using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ContainerCounter : BaseCounter
{
   public event EventHandler OnPlayerTakeObject;


   [SerializeField] private KitchenObjectSO _kitchenObjectSO;
   [SerializeField] private GameObject _iconSpriteGameObject;


   private void Awake()
   {
      SetCorrectIcon();
   }

   public override void Interact(Player player)
   {
      if(!player.HasKitchenObject())
      {
         KitchenObject.SpawnKitchenObject(_kitchenObjectSO, player);
         
         InteractLogicServerRpc();
      }
      else if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
      {
         if(plateKitchenObject.TryAddIngridient(_kitchenObjectSO))
         {
            InteractLogicServerRpc();
         }
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void InteractLogicServerRpc()
   {
      InteractLogicClientRpc();
   }

   [ClientRpc]
   private void InteractLogicClientRpc()
   {
      OnPlayerTakeObject?.Invoke(this,EventArgs.Empty);
   }

   private void SetCorrectIcon()
   {
      _iconSpriteGameObject.GetComponent<SpriteRenderer>().sprite = _kitchenObjectSO.GetIconSprite;
   }
}
