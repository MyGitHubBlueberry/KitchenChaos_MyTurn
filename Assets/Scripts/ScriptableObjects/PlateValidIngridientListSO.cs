using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu()]
public class PlateValidIngridientListSO : ScriptableObject
{
   [SerializeField] private List<KitchenObjectSO> _validKithenObjectSOList;

    public List<KitchenObjectSO> GetValidKithenObjectSOList { get => _validKithenObjectSOList; }
}
