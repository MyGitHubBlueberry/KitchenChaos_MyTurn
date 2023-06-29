using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
   private Player _player;
   private float _footStepTimer;
   private float _footStepTimerMax = .1f;


   private void Awake()
   {
      _player = GetComponent<Player>();
   }

   private void Update()
   {
      if(_player.IsWalking)
      {
         _footStepTimer += Time.deltaTime;

         if(_footStepTimerMax <= _footStepTimer)
         {
            _footStepTimer = 0f;


            SoundManager.Instance.PlayFootstepsSound(_player.transform.position);
         }
      }
   }
}
