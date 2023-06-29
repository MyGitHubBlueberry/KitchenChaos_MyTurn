using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;


public class GameOverSingleUI : MonoBehaviour
{
   [SerializeField] private TextMeshProUGUI _playerNameText;
   [SerializeField] private TextMeshProUGUI _playerPointsText;
   [SerializeField] private TextMeshProUGUI _playerRecipesAmountText;

   [SerializeField] private Transform _backgroundTranfrom;
   [SerializeField] private List<Transform> _secondaryTranfromList;

   [SerializeField] private Color _backgroundColor;
   [SerializeField] private Color _secondaryColor;


   public void SetPlayerGameStats(PlayerData playerData, bool isYourPlayer)
   {
      _playerNameText.text = playerData.playerName.ToString();
      _playerPointsText.text = playerData.playerPoints.ToString();
      _playerRecipesAmountText.text = playerData.playerSuccessRecipesAmount.ToString();
      
      if(isYourPlayer)
      {
         _backgroundTranfrom.GetComponent<Image>().color = _backgroundColor;
         foreach(Transform secondaryTranfrom in _secondaryTranfromList)
         {
            secondaryTranfrom.GetComponent<Image>().color = _secondaryColor;
         }         
      }
   }

   // public void SetPlayerGameStats(Player player, bool isYourPlayer)
   // {
   //    _playerNameText.text = player.GetPlayerName();
   //    _playerPointsText.text = player.GetPlayerPoints().ToString();
   //    _playerRecipesAmountText.text = player.GetPlayerSuccessRecipeAmount().ToString();
      
   //    if(isYourPlayer)
   //    {
   //       _backgroundTranfrom.GetComponent<Image>().color = _backgroundColor;
   //       foreach(Transform secondaryTranfrom in _secondaryTranfromList)
   //       {
   //          secondaryTranfrom.GetComponent<Image>().color = _secondaryColor;
   //       }         
   //    }
   // }
}
