using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounterVisual : MonoBehaviour
{
   [SerializeField] private Transform _plateVisual;

   
   private PlatesCounter _platesCounter;
   private Stack<GameObject> _spawnedPlatesVisualPositions;


   private void Awake()
   {
      _platesCounter = GetComponentInParent<PlatesCounter>();
      _spawnedPlatesVisualPositions = new Stack<GameObject>();
   }

   private void Start()
   {
      _platesCounter.OnPlateSpawned += PlatesCounter_OnPlateSpawned;
      _platesCounter.OnPlateRemoved += PlatesCounter_OnPlateRemoved;
   }

   private void PlatesCounter_OnPlateSpawned(object sender, EventArgs e)
   {
      Transform platePosition = Instantiate(_plateVisual, _platesCounter.GetKitchenObjectTargetPosition());   

      float platesOffstepY = .1f;
      platePosition.localPosition = new Vector3(0, _spawnedPlatesVisualPositions.Count * platesOffstepY,0);

      _spawnedPlatesVisualPositions.Push(platePosition.gameObject);
   }

   private void PlatesCounter_OnPlateRemoved(object sender, EventArgs e)
   {
      Destroy(_spawnedPlatesVisualPositions.Peek());

      _spawnedPlatesVisualPositions.Pop();
   }

}
