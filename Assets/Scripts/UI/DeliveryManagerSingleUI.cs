using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeliveryManagerSingleUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI _recipeNameText;
   [SerializeField] private TextMeshProUGUI _reicpePointsText;
   [SerializeField] private Transform _iconContainer;
   [SerializeField] private Transform _iconTemplate;


   private void Awake()
   {
      _iconTemplate.gameObject.SetActive(false);
   }

   public void SetRecipe(RecipeWithCondiments recipeWithCondiments)
   {
      _recipeNameText.text = recipeWithCondiments.GetRecipeWithCondimentsName();
      _reicpePointsText.text = recipeWithCondiments.GetRecipeWithCondimentsTotalPoints().ToString();
      
      foreach(Transform child in _iconContainer)
         if(child != _iconTemplate) Destroy(child.gameObject);

      foreach(KitchenObjectSO recipeIngridient in recipeWithCondiments.GetRecipeWithCondimentsKitchenObjectSOArray())
      {
         Transform iconTrnasform = Instantiate(_iconTemplate, _iconContainer);
         iconTrnasform.gameObject.SetActive(true);

         iconTrnasform.GetComponent<Image>().sprite = recipeIngridient.GetIconSprite;
      }
   }
}
