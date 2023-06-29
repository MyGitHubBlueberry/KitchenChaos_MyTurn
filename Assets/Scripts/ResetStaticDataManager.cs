using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStaticDataManager : MonoBehaviour
{
   private void Awake()
   {
      BaseCounter.ResetStaticData();
      CondimentCounter.ResetStaticData();
      CuttingCounter.ResetStaticData();
      TrashCounter.ResetStaticData();
      Player.ResetStaticData();
   }
}
