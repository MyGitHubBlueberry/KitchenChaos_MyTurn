using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour
{
   private const string POPUP = "Popup";


   [SerializeField] private Image _backgroundImage;
   [SerializeField] private Image _iconImage;
   [SerializeField] private TextMeshProUGUI _messageText;

   [SerializeField] private Color _successColor;
   [SerializeField] private Color _failColor;
   [SerializeField] private Sprite _successSprite;
   [SerializeField] private Sprite _failSprite;


   private Animator _animator;


   private void Awake()
   {
      _animator = GetComponent<Animator>();
   }

   private void Start()
   {
      DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
      DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
   

      gameObject.SetActive(false);
   }

   private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
   {
      gameObject.SetActive(true);
      
      _backgroundImage.color = _failColor;
      _iconImage.sprite = _failSprite;
      _messageText.text = "DELIVERY\nFAILED";

      _animator.SetTrigger(POPUP);
   }

   private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
   {
      gameObject.SetActive(true);

      _backgroundImage.color = _successColor;
      _iconImage.sprite = _successSprite;
      _messageText.text = "DELIVERY\nSUCCESS";

      _animator.SetTrigger(POPUP);
   }
}
