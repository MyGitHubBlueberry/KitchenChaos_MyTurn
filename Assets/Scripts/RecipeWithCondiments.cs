using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeWithCondiments
{
   private RecipeSO _recipeSO;
   private KitchenObjectSO[] _condimentsArray;
   private KitchenObjectSO[] _recipeWithCondimentsKitchenObjectSOArray;
   private int _recipeWithCondimentsTotalPoints;


   public RecipeWithCondiments(RecipeSO recipeSO, CondimentListSO allPossibleCondiments, int amountOfCondimentsToGenerate, int[] condimentNumbers)
   {
      _recipeSO = recipeSO;
      _condimentsArray = InitializeCondiments(allPossibleCondiments, amountOfCondimentsToGenerate, condimentNumbers);
      _recipeWithCondimentsTotalPoints = _recipeSO.GetRecipePoints + GetCondimentsTotalPoints();

      _recipeWithCondimentsKitchenObjectSOArray = new KitchenObjectSO[recipeSO.GetRecipeKitchenObjectSOList.Count + _condimentsArray.Length];
      recipeSO.GetRecipeKitchenObjectSOList.CopyTo(_recipeWithCondimentsKitchenObjectSOArray);
      _condimentsArray.CopyTo(_recipeWithCondimentsKitchenObjectSOArray, recipeSO.GetRecipeKitchenObjectSOList.Count);
   }
   
   public KitchenObjectSO[] GetRecipeWithCondimentsKitchenObjectSOArray()
   {
      return _recipeWithCondimentsKitchenObjectSOArray;
   }

   public string GetRecipeWithCondimentsName()
   {
      return _recipeSO.GetRecipeName;
   }

   public int GetRecipeWithCondimentsTotalPoints()
   {
      return _recipeWithCondimentsTotalPoints;
   }

   private int GetCondimentsTotalPoints()
   {
      int toatlCondimentPoints = 0;
      foreach(KitchenObjectSO condiment in _condimentsArray)
         toatlCondimentPoints += condiment.GetPoints;
      return toatlCondimentPoints;
   }

   private KitchenObjectSO[] InitializeCondiments(CondimentListSO condimentListSO, int amountOfCondimentsToGenerate, int[] condimentNumbers)
   {
      if(amountOfCondimentsToGenerate > 0)
      {
         KitchenObjectSO[] condiments = new KitchenObjectSO[amountOfCondimentsToGenerate];
         
         if(amountOfCondimentsToGenerate == condimentListSO.GetCondimentKitchenObjectSOList.Count)
         {
            condiments = condimentListSO.GetCondimentKitchenObjectSOList.ToArray();

            return condiments;
         }

         KitchenObjectSO[] allCondimentsArray = new KitchenObjectSO[condimentListSO.GetCondimentKitchenObjectSOList.Count];
         condimentListSO.GetCondimentKitchenObjectSOList.CopyTo(allCondimentsArray);

         for(int i = 0; i < amountOfCondimentsToGenerate; i ++)
            condiments[i] = allCondimentsArray[condimentNumbers[i]];

         return condiments;
      }
      else
      {
         return new KitchenObjectSO[0];
      }
   }
}