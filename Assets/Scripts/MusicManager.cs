using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
   private const string PLYAER_PREFS_MUSIC_VOLUME = "MusicVolume";
   
   
   public static MusicManager Instance {get; private set;}


   private AudioSource _audioSource;
   private float _volume = .3f;


   private void Awake()
   {
      Instance = this;

      _audioSource = GetComponent<AudioSource>();

      float defaultVolume = .3f;
      _volume = PlayerPrefs.GetFloat(PLYAER_PREFS_MUSIC_VOLUME, defaultVolume);

      _audioSource.volume = _volume;
   }

   public void ChangeVolume()
   {
      _volume += .1f;
      
      if(_volume > 1f) _volume = 0f;

      _audioSource.volume = _volume;

      PlayerPrefs.SetFloat(PLYAER_PREFS_MUSIC_VOLUME, _volume);
      PlayerPrefs.Save();
   }

   public float GetVolume()
   {
      return _volume;
   }
}
