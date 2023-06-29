using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameStartCountdownUI : MonoBehaviour
{
   private const string NUMBER_POPUP = "NumberPopup";


   [SerializeField] private TextMeshProUGUI _countdownText;


   private Animator _animator;
   private int _previousCountdownNumber;


   private void Awake()
   {
      _animator = GetComponent<Animator>();
   }

   private void Start()
   {
      GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

      Hide();
   }
   
   private void Update()
   {
      int countdownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
      _countdownText.text = countdownNumber.ToString();

      if(countdownNumber != _previousCountdownNumber)
      {
         _previousCountdownNumber = countdownNumber;

         _animator.SetTrigger(NUMBER_POPUP);
         
         SoundManager.Instance.PlayCountdownSound();
      }
   }

   private void GameManager_OnStateChanged(object sender, EventArgs e)
   {
      if(GameManager.Instance.IsCountdownToStartActive())
      {
         Show();
      }
      else
      {
         Hide();
      }
   }

   private void Show()
   {
      gameObject.SetActive(true);
   }

   private void Hide()
   {
      gameObject.SetActive(false);
   }

}
