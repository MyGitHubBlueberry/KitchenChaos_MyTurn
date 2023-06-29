using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager : MonoBehaviour
{
   public static SoundManager Instance {get; private set;}


   private const string PLAYER_PREFS_SOUND_EFFECTS_VOLUME = "SoundEffectsVolume";
   [SerializeField] private AudioClipRefsSO _audioClipRefsSO;


   private float _volume = 1f;


   private void Awake()
   {
      Instance  = this;

      float defaultVolume = 1f;
      _volume = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, defaultVolume);
   }

   private void Start()
   {
      DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
      DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
      CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
      Player.OnAnyPickedSomething += Player_OnPickedSomething;
      BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
      TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
      CondimentCounter.OnAnyCondimentIsUsing += CondimentCounter_OnAnyCondimentIsUsing;
   }

   private void CondimentCounter_OnAnyCondimentIsUsing(object sender, EventArgs e)
   {
      CondimentCounter condimentCounter = sender as CondimentCounter;
      PlaySound(_audioClipRefsSO.GetUsingCoundiment, condimentCounter.transform.position);
   }

   private void TrashCounter_OnAnyObjectTrashed(object sender, EventArgs e)
   {
      TrashCounter trashCounter = sender as TrashCounter;
      PlaySound(_audioClipRefsSO.GetTrash, trashCounter.transform.position);
   }

   private void BaseCounter_OnAnyObjectPlacedHere(object sender, EventArgs e)
   {
      BaseCounter baseCounter = sender as BaseCounter;
      PlaySound(_audioClipRefsSO.GetObjectDrop, baseCounter.transform.position);
   }

   private void Player_OnPickedSomething(object sender, EventArgs e)
   {
      Player player = sender as Player;
      PlaySound(_audioClipRefsSO.GetObjectPickup, player.transform.position);
   }

   private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
   {
      CuttingCounter cuttingCounter = sender as CuttingCounter;
      PlaySound(_audioClipRefsSO.GetChop, cuttingCounter.transform.position);
   }

   private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
   {
      DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
      PlaySound(_audioClipRefsSO.GetDeliverySuccess, deliveryCounter.transform.position);
   }

   private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
   {
      DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
      PlaySound(_audioClipRefsSO.GetDeliveryFail, deliveryCounter.transform.position);
   }


   private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f)
   {
      AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * _volume);
   }

   private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
   {
      PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume);
   }

   public void PlayFootstepsSound(Vector3 position, float volume = 1f)
   {
      PlaySound(_audioClipRefsSO.GetFootstep, position, volume);
   }

   public void PlayWarningSound(Vector3 position)
   {
      PlaySound(_audioClipRefsSO.GetWarning, position, _volume);
   }

   public void PlayCountdownSound()
   {
      PlaySound(_audioClipRefsSO.GetWarning, Vector3.zero, _volume);
   }

   public void ChangeVolume()
   {
      _volume += .1f;
      
      if(_volume > 1f) _volume = 0f;

      PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECTS_VOLUME, _volume);
      PlayerPrefs.Save();
   }

   public float GetVolume()
   {
      return _volume;
   }
}  