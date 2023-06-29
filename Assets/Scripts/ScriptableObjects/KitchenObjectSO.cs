using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject
{
   [SerializeField] private Transform _prefab;
   [SerializeField] private Sprite _iconSprite;
   [SerializeField] private string _objectName;
   [SerializeField] private int _points;

   public Transform GetPrefab { get => _prefab; }
   public Sprite GetIconSprite { get => _iconSprite; }
   public string GetObjectName { get => _objectName; }
   public int GetPoints { get => _points; }
}
