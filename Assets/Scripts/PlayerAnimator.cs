using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
   private const string IS_WALKING = "IsWalking";


   private Player _player;
   private Animator _animator;


   private void Awake()
   {
      _animator = GetComponent<Animator>();
      _player = GetComponentInParent<Player>();
   }

   private void Update()
   {
      if(!IsOwner) return;
      
      _animator.SetBool(IS_WALKING, _player.IsWalking);
   }
}
