using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class FryingRecipeSO : ScriptableObject
{
   [SerializeField] private KitchenObjectSO _inputKitchenObjectSO;
   [SerializeField] private KitchenObjectSO _outputKitchenObjectSO;
   [SerializeField] private float _fryingTimerMax;

   
   public KitchenObjectSO GetInputKitchenObjectSO { get => _inputKitchenObjectSO; }
   public KitchenObjectSO GetOutputKitchenObjectSO { get => _outputKitchenObjectSO; }
   public float GetFryingTimerMax { get => _fryingTimerMax; }
   public string GetFryingRecipeName { get => $"{_inputKitchenObjectSO.GetObjectName} - {_outputKitchenObjectSO.GetObjectName}"; }
}
