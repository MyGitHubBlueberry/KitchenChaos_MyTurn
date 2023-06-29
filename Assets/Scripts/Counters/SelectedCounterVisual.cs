using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
   [SerializeField] private List<GameObject> _selectedCounterVisualArray;
   private BaseCounter _counter;


   private void Awake()
   {
      _counter = GetComponentInParent<BaseCounter>();
   }
   private void Start()
   {
      if(Player.LocalInstance != null)
         Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
      else
         Player.OnAnyPlayerSpawned +=Player_OnAnyPlayerSpawned;


   }

   private void Player_OnAnyPlayerSpawned(object sender, EventArgs e)
   {
      if(Player.LocalInstance != null)
      {
         Player.LocalInstance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
         Player.LocalInstance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
      }
   }

   private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
   {
      foreach(GameObject selectedCounterVisual in _selectedCounterVisualArray)
         selectedCounterVisual.SetActive(_counter == e.SelectedCounter);
   }
}
