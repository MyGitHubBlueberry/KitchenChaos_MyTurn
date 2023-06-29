using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ClearCounter : BaseCounter
{
   public override void Interact(Player player)
   {
      if(!HasKitchenObject())
      {
         if(player.HasKitchenObject())
         {
            player.GetKitchenObject().SetKitchenObjectParent(this);
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
         }
      }
   }
}
