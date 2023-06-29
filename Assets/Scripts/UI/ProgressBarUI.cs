using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
   [SerializeField] private Image _barImage;
   [SerializeField]private GameObject _hasProgressGameObject;


   private IHasProgress _hasProgress;


   private void Start()
   {
      if(!_hasProgressGameObject.TryGetComponent(out _hasProgress))
      {
         Debug.LogError("GameObject " + _hasProgressGameObject + "does not implement IHasProgress interface");
      }
      

      _hasProgress.OnProgressChanged += HasProgress_OnProgressChanged;

      _barImage.fillAmount = 0f;

      Hide();
   }

   private void HasProgress_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArgs e)
   {
      _barImage.fillAmount = e.ProgressNormalized;
      
      if(e.ProgressNormalized == 0f || e.ProgressNormalized == 1f && gameObject.activeSelf)
         Hide();
      else if(!gameObject.activeSelf)
         Show();
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
