using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject
{
   [SerializeField] private KitchenObjectSO _inputKitchenObjectSO;
   [SerializeField] private KitchenObjectSO _outputKitchenObjectSO;
   [SerializeField] private int _cutsMaxAmount;

   
   public KitchenObjectSO GetInputKitchenObjectSO { get => _inputKitchenObjectSO; }
   public KitchenObjectSO GetOutputKitchenObjectSO { get => _outputKitchenObjectSO; }
   public int GetCutsMaxAmount { get => _cutsMaxAmount; }
   public string GetCuttingRecipeName { get => $"{_inputKitchenObjectSO.GetObjectName} - {_outputKitchenObjectSO.GetObjectName}"; }
}
