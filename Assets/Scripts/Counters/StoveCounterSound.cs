using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour
{
   [SerializeField] private StoveCounter _stoveCounter;

   private AudioSource _audioSource;
   private float _waringSoundTimer; 
   private bool _playWarningSound;


   private void Awake()
   {
      _audioSource = GetComponent<AudioSource>();
   }

   private void Start()
   {
      _stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
      _stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;
   }

   private void Update()
   {
      if(_playWarningSound)
      {
         _waringSoundTimer -= Time.deltaTime;

         if(_waringSoundTimer <= 0f)
         {
            float waringSoundTimerMax = .2f;
            _waringSoundTimer = waringSoundTimerMax;
            
            SoundManager.Instance.PlayWarningSound(_stoveCounter.transform.position);
         }
      }
   }

   private void StoveCounter_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
   {
      float burnShowProgressAmount = .5f;
      _playWarningSound = _stoveCounter.IsFryed() && e.ProgressNormalized >= burnShowProgressAmount;
   }

   private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
   {
      bool playSound = e.state == StoveCounter.State.Frying || e.state == StoveCounter.State.Fryed;

      if(playSound)
         _audioSource.Play();
      else
         _audioSource.Pause();
   }
}
