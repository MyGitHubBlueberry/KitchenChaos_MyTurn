using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class CuttingCounter : BaseCounter, IInteractAlternative, IHasProgress
{
   public static event EventHandler OnAnyCut;

   new public static void ResetStaticData()
   {
      OnAnyCut = null;
   }
   

   public event EventHandler OnCut;
   public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

   [SerializeField] private CuttingRecipeSO[] _cuttingRecipeSOArray;


   private int _currentCuttingProgress;



   public override void Interact(Player player)
   {
      if(HasKitchenObject())
      {
         if(!player.HasKitchenObject())
         {
            GetKitchenObject().SetKitchenObjectParent(player);
            InteractLogicPlaceObjectOnCounterServerRpc();
         }
         else if(player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
         {
            if(plateKitchenObject.TryAddIngridient(GetKitchenObject().GetKitchenObjectSO()))
            {
               KitchenObject.DestroyKitchenObject(GetKitchenObject());
            }
         }
      }
      else
      {
         if(player.HasKitchenObject())
         {
            if(TryGetCuttingRecipeSO(player.GetKitchenObject().GetKitchenObjectSO(), out CuttingRecipeSO currentRecepeSO))
            {
               player.GetKitchenObject().SetKitchenObjectParent(this);

               InteractLogicPlaceObjectOnCounterServerRpc();
            }
         }
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void InteractLogicPlaceObjectOnCounterServerRpc()
   {
      InteractLogicPlaceObjectOnCounterClientRpc();
   }

   [ClientRpc]
   private void InteractLogicPlaceObjectOnCounterClientRpc()
   {
      _currentCuttingProgress = 0;

      OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
      (
         progressNormalized: (float)_currentCuttingProgress
      ));
   }

   public void InteractAlternative(Player player)
   {
      if(HasKitchenObject() && CanBeCuted(GetKitchenObject().GetKitchenObjectSO()))
      {
         CutObjectServerRpc();
         TestCuttingProgressDoneServerRpc();
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void CutObjectServerRpc()
   {
      if(HasKitchenObject() && CanBeCuted(GetKitchenObject().GetKitchenObjectSO()))
      {
         CutObjectClientRpc();
      }
   }

   [ClientRpc]
   private void CutObjectClientRpc()
   {
      _currentCuttingProgress++;

      OnCut?.Invoke(this, EventArgs.Empty);
      OnAnyCut?.Invoke(this, EventArgs.Empty);

      TryGetCuttingRecipeSO(GetKitchenObject().GetKitchenObjectSO(), out CuttingRecipeSO currentRecepeSO);

      OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
      (
         progressNormalized: (float)_currentCuttingProgress / currentRecepeSO.GetCutsMaxAmount
      ));
   }

   [ServerRpc(RequireOwnership = false)]
   private void TestCuttingProgressDoneServerRpc()
   {
      if(HasKitchenObject() && TryGetCuttingRecipeSO(GetKitchenObject().GetKitchenObjectSO(), out CuttingRecipeSO currentRecepeSO))
      {
         bool cuted = _currentCuttingProgress >= currentRecepeSO.GetCutsMaxAmount;

         if(cuted)
         {
            KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());

            KitchenObject.DestroyKitchenObject(GetKitchenObject());

            KitchenObject.SpawnKitchenObject(outputKitchenObjectSO,this);
         }
      }
   }

   private bool TryGetCuttingRecipeSO(KitchenObjectSO kitchenObjectSO, out CuttingRecipeSO  currentCuttingRecipeSO)
   {
      foreach(CuttingRecipeSO cuttingRecipeSO in _cuttingRecipeSOArray)
      {
         if(cuttingRecipeSO.GetInputKitchenObjectSO == kitchenObjectSO)
         {
            currentCuttingRecipeSO = cuttingRecipeSO;
            return true;
         }
      }

      currentCuttingRecipeSO = null;
      return false;
   }

   private bool CanBeCuted(KitchenObjectSO kitchenObjectSO)
   {
      return TryGetCuttingRecipeSO(kitchenObjectSO, out CuttingRecipeSO cuttingRecipeSO);
   }

   private KitchenObjectSO GetOutputForInput(KitchenObjectSO kitchenObjectSO)
   {
      TryGetCuttingRecipeSO(kitchenObjectSO, out CuttingRecipeSO cuttingRecipeSO);

      return cuttingRecipeSO.GetOutputKitchenObjectSO;
   }
}
