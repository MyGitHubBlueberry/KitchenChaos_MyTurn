using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CondimentCounterAnimator : MonoBehaviour
{
   private const string IsUsingCondiment = "IsUsingCondiment";

   
   private Animator _animator;
   private CondimentCounter _condimentCounter;


   private void Awake()
   {
      _animator = GetComponent<Animator>();
      _condimentCounter = GetComponentInParent<CondimentCounter>();
   }

   private void Start()
   {
      _condimentCounter.OnStateChanged += CondimentCounter_OnStateChanged;
   }

   private void CondimentCounter_OnStateChanged(object sender, CondimentCounter.OnStateChangedEventArgs stateHandler)
   {
      bool isUsingCondiment = (stateHandler.GetState == CondimentCounter.State.UsingCondiment);
      _animator.SetBool(IsUsingCondiment, isUsingCondiment);
   }
}
