using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterAnimator : MonoBehaviour
{
   private const string CUT = "Cut";


   private Animator _animator;
   private CuttingCounter _cuttingCounter;


   private void Awake()
   {
      _animator = GetComponent<Animator>();
      _cuttingCounter = GetComponentInParent<CuttingCounter>();
   }

   private void Start()
   {
      _cuttingCounter.OnCut += CuttingCounter_OnCut;
   }

   private void CuttingCounter_OnCut(object sender, EventArgs e)
   {
      _animator.SetTrigger(CUT);
   }
}
