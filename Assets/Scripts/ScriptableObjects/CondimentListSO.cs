using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CondimentListSO : ScriptableObject
{
   [SerializeField] private List<KitchenObjectSO> _condimentKitchenObjectSOList;

   public List<KitchenObjectSO> GetCondimentKitchenObjectSOList { get => _condimentKitchenObjectSOList; }
}
