using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour
{
   [SerializeField]private GameObject _stoveOnGameObject;
   [SerializeField]private GameObject _particlesGameObject;


   private StoveCounter _stoveCounter;


   private void Awake()
   {
      _stoveCounter = GetComponentInParent<StoveCounter>();

      _particlesGameObject.SetActive(false);
      _stoveOnGameObject.SetActive(false);
   }

   private void Start()
   {
      _stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
   }

   private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
   {
      bool showVisual = e.state == StoveCounter.State.Frying|| e.state == StoveCounter.State.Fryed;

      _particlesGameObject.SetActive(showVisual);
      _stoveOnGameObject.SetActive(showVisual);
   }
}
