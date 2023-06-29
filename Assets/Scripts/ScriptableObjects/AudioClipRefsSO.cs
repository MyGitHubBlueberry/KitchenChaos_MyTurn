using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject
{
   [SerializeField] private AudioClip[] _chop;
   [SerializeField] private AudioClip[] _deliveryFail;
   [SerializeField] private AudioClip[] _deliverySuccess;
   [SerializeField] private AudioClip[] _footstep;
   [SerializeField] private AudioClip[] _objectDrop;
   [SerializeField] private AudioClip[] _objectPickup;
   [SerializeField] private AudioClip _stoveSizzle;
   [SerializeField] private AudioClip[] _trash;
   [SerializeField] private AudioClip[] _warning;
   [SerializeField] private AudioClip[] _usingCondiment;

   public AudioClip[] GetChop { get => _chop; }
   public AudioClip[] GetDeliveryFail { get => _deliveryFail; }
   public AudioClip[] GetDeliverySuccess { get => _deliverySuccess; }
   public AudioClip[] GetFootstep { get => _footstep; }
   public AudioClip[] GetObjectDrop { get => _objectDrop; }
   public AudioClip[] GetObjectPickup { get => _objectPickup; }
   public AudioClip GetStoveSizzle { get => _stoveSizzle; }
   public AudioClip[] GetTrash { get => _trash; }
   public AudioClip[] GetWarning { get => _warning; }
   public AudioClip[] GetUsingCoundiment { get => _usingCondiment; }
}
