using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
   [SerializeField] private Transform _iconTemplate;
   private PlateKitchenObject _plateKitchenObject;


   private void Awake()
   {
      _plateKitchenObject = GetComponentInParent<PlateKitchenObject>();

      _iconTemplate.gameObject.SetActive(false);
   }  

   private void Start()
   {
      _plateKitchenObject.OnIngridientAdded += PlateKitchenObject_OnIngridientAdded;
   }

   private void PlateKitchenObject_OnIngridientAdded(object sender, PlateKitchenObject.OnIngridientAddedEventArgs e)
   {
      UpdateVisual();  
   }

   private void UpdateVisual()
   {
      foreach(Transform child in transform)
         if(child != _iconTemplate) Destroy(child.gameObject);


      foreach(KitchenObjectSO kitchenObjectSO in _plateKitchenObject.GetKitchenObjectSOList())
      {
         Transform iconTransform = Instantiate(_iconTemplate, transform);
         iconTransform.gameObject.SetActive(true);
         iconTransform.GetComponent<PlateIconSingleUI>().SetIconSprite(kitchenObjectSO);
      }
   }
}
