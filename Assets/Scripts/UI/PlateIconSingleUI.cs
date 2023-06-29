using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour
{
   [SerializeField] private Image _icon;

   
   public void SetIconSprite(KitchenObjectSO kitchenObjectSO)
   {
      _icon.sprite = kitchenObjectSO.GetIconSprite;
   }
}
