using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu()]
public class KithcenObjectListSO : ScriptableObject
{
   [SerializeField] private List<KitchenObjectSO> _kitchenObjectSOList;

   public List<KitchenObjectSO> GetKitchenObjectSOList {get => _kitchenObjectSOList;}
}
