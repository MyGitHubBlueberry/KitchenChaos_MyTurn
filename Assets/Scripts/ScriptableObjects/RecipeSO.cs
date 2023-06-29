using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject
{
   [SerializeField] private List<KitchenObjectSO> _recipeKitchenObjectSOList;
   [SerializeField] private string _recipeName;


   public List<KitchenObjectSO> GetRecipeKitchenObjectSOList { get => _recipeKitchenObjectSOList; }
   public string  GetRecipeName{ get => _recipeName; }
   public int GetRecipePoints
   {
      get
      {
         int totalPoints = 0;
         foreach(KitchenObjectSO kitchenObjectSO in _recipeKitchenObjectSOList)
            totalPoints += kitchenObjectSO.GetPoints;
         return totalPoints;
      }
   }
}
