using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeListSO : ScriptableObject
{
   [SerializeField] private List<RecipeSO> _recipeSOList;

   public List<RecipeSO> GetRecipeSOList { get => _recipeSOList; }
}
