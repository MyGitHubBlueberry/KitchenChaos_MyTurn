using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BurningRecipeSO : ScriptableObject
{
   [SerializeField] private KitchenObjectSO _inputKitchenObjectSO;
   [SerializeField] private KitchenObjectSO _outputKitchenObjectSO;
   [SerializeField] private float _burningTimerMax;

   
   public KitchenObjectSO GetInputKitchenObjectSO { get => _inputKitchenObjectSO; }
   public KitchenObjectSO GetOutputKitchenObjectSO { get => _outputKitchenObjectSO; }
   public float GetBurningTimerMax { get => _burningTimerMax; }
   public string GetBurningRecipeName { get => $"{_inputKitchenObjectSO.GetObjectName} - {_outputKitchenObjectSO.GetObjectName}"; }
}
