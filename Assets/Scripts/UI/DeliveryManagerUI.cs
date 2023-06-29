using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour
{
   [SerializeField] private Transform _container;
   [SerializeField] private Transform _recipeTemplate;


   private void Awake()
   {
      _recipeTemplate.gameObject.SetActive(false);
   }

   private void Start()
   {
      DeliveryManager.Instance.OnRecipeCompleted +=  DeliveryManager_OnRecipeCompleted;
      DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;

      UpdateVisual();
   }

   private void DeliveryManager_OnRecipeSpawned(object sender, EventArgs e)
   {
      UpdateVisual();
   }

   private void DeliveryManager_OnRecipeCompleted(object sender, EventArgs e)
   {
      UpdateVisual();
   }

   private void UpdateVisual()
   {
      foreach (Transform child in _container)
         if(child != _recipeTemplate) Destroy(child.gameObject);

      foreach(RecipeWithCondiments waitingRecipe in DeliveryManager.Instance.GetWaitingRecipeWithCondimentsList())
      {
         Transform recipeTransform = Instantiate(_recipeTemplate, _container);

         recipeTransform.gameObject.SetActive(true);
         recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipe(waitingRecipe);
      }
   }
}
