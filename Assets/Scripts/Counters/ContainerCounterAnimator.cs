using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterAnimator : MonoBehaviour
{
   private const string  OPEN_CLOSE = "OpenClose";


   private ContainerCounter _containerCounter;
   private Animator _animator;


   private void Awake()
   {
      _containerCounter = GetComponentInParent<ContainerCounter>();
      _animator = GetComponent<Animator>();
   }

   private void Start()
   {
      _containerCounter.OnPlayerTakeObject += ContainerCounter_OnPlayerTakeObject;
   }

   private void ContainerCounter_OnPlayerTakeObject(object sender, EventArgs e)
   {
      _animator.SetTrigger(OPEN_CLOSE);
   } 
}
