using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
   [Serializable]
   private struct KitchenObjectSO_GameObject
   {
      [SerializeField]private KitchenObjectSO _kitchenObjectSO;
      [SerializeField]private GameObject _gameObject;

      public KitchenObjectSO GetKitchenObjectSO { get => _kitchenObjectSO; }
      public GameObject GetGameObject { get => _gameObject; }
   }


   [SerializeField] private List<KitchenObjectSO_GameObject> _kitchenObjectSOGameObjectList;


   private PlateKitchenObject _plateKitchenObject;

   
   private void Awake()
   {
      _plateKitchenObject = GetComponentInParent<PlateKitchenObject>();

      foreach(KitchenObjectSO_GameObject kitchenObjectSOGameObject in _kitchenObjectSOGameObjectList)
         kitchenObjectSOGameObject.GetGameObject.SetActive(false);
   }

   private void Start()
   {
      _plateKitchenObject.OnIngridientAdded += PlateKitchenObject_OnIngridientAdded;
   }

   private void PlateKitchenObject_OnIngridientAdded(object sender, PlateKitchenObject.OnIngridientAddedEventArgs e)
   {
      foreach(KitchenObjectSO_GameObject kitchenObjectSOGameObject in _kitchenObjectSOGameObjectList)
         if(kitchenObjectSOGameObject.GetKitchenObjectSO == e.KitchenObjectSO)
            kitchenObjectSOGameObject.GetGameObject.SetActive(true);
   }
}
