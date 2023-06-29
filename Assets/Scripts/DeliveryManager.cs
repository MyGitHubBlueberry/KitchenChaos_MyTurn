using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

public class DeliveryManager : NetworkBehaviour
{
   public static DeliveryManager Instance { get; private set; }


   public event EventHandler OnRecipeSpawned;
   public event EventHandler OnRecipeCompleted;
   public event EventHandler<OnPointsAddedEventArgs> OnPointsAdded;
   public class OnPointsAddedEventArgs : EventArgs
   {
      public readonly int GetAddedPoints;
      public readonly ulong GetPlayerClientId;

      public OnPointsAddedEventArgs(int pointsAdded, ulong playerClientId )
      {
         GetAddedPoints = pointsAdded;
         GetPlayerClientId = playerClientId;
      }
   }
   public event EventHandler OnRecipeSuccess;
   public event EventHandler OnRecipeFailed;


   [SerializeField] private RecipeListSO _recipeListSO;
   [SerializeField] private CondimentListSO _condimentListSO;
   

   private List<RecipeWithCondiments> _waitingRecipeWithCondimentsList;
   private NetworkVariable<int> _totalPoints = new NetworkVariable<int>(0);
   private int _successfulRecipeAmount;
   private float _recipeSpawnTimer;
   private float _recipeSpawnTimerMax = 4f;
   private float _recipeMaxCount = 4;



   private void Awake()
   {
      Instance = this;


      _waitingRecipeWithCondimentsList = new List<RecipeWithCondiments>();
   }

   private void Update()
   {
      if(!IsServer) return;

      if(_waitingRecipeWithCondimentsList.Count < _recipeMaxCount && GameManager.Instance.IsGamePlaying())
      {
         _recipeSpawnTimer += Time.deltaTime;

         if(_recipeSpawnTimer >= _recipeSpawnTimerMax)
         {
            _recipeSpawnTimer = 0f;

            GenereateStatsForNewRecipe(out int waitingRecipeIndex, out int amountOfCondimentsToGenerate, out int[] condimentIndexesArray);

            SpawnNewWaitingRecipeClientRpc(waitingRecipeIndex,amountOfCondimentsToGenerate, condimentIndexesArray);
         }
      }
   }

   [ClientRpc]
   private void SpawnNewWaitingRecipeClientRpc(int waitingRecipeIndex, int amountOfCondimentsToGenerate, params int[] condimentIndexesArray)
   {
      RecipeSO randomRecipe = _recipeListSO.GetRecipeSOList[waitingRecipeIndex];

      RecipeWithCondiments waitingRecipeSO = new RecipeWithCondiments(randomRecipe, _condimentListSO, amountOfCondimentsToGenerate, condimentIndexesArray);

      _waitingRecipeWithCondimentsList.Add(waitingRecipeSO);

      OnRecipeSpawned?.Invoke(this,EventArgs.Empty);
   }

   private void GenereateStatsForNewRecipe(out int waitingRecipeIndex, out int amountOfCondimentsToGenerate, out int[] condimentIndexesArray)
   {  
      waitingRecipeIndex = UnityEngine.Random.Range(0, _recipeListSO.GetRecipeSOList.Count);
      amountOfCondimentsToGenerate = UnityEngine.Random.Range(0, _condimentListSO.GetCondimentKitchenObjectSOList.Count + 1);
      List<int> possibleCondimentIndexes = Enumerable.Range(0, _condimentListSO.GetCondimentKitchenObjectSOList.Count).ToList();
      List<int> condimentIndexesList = new List<int>();

      for(int i = 0; i < amountOfCondimentsToGenerate; i ++)
      {
         int condimentIndex = UnityEngine.Random.Range(0, possibleCondimentIndexes.Count);
         condimentIndexesList.Add(possibleCondimentIndexes[condimentIndex]);
         possibleCondimentIndexes.RemoveAt(condimentIndex);
      }
      condimentIndexesArray = condimentIndexesList.ToArray();
   }

   public void DeliverRecipe(PlateKitchenObject plateKitchenObject, ulong playerClientId)
   {
      for(int i = 0; i < _waitingRecipeWithCondimentsList.Count; i++)
      {
         RecipeWithCondiments waitingRecipeSO = _waitingRecipeWithCondimentsList[i];
         
         if(waitingRecipeSO.GetRecipeWithCondimentsKitchenObjectSOArray().Length == plateKitchenObject.GetKitchenObjectSOList().Count)
         {
            bool platesContentMatches = true;
            foreach(KitchenObjectSO expextedIngridient in waitingRecipeSO.GetRecipeWithCondimentsKitchenObjectSOArray())
            {
               bool ingridientFound = false;
               foreach(KitchenObjectSO realIngridient in plateKitchenObject.GetKitchenObjectSOList())
               {
                  if(expextedIngridient == realIngridient)
                  {
                     ingridientFound = true;
                     break;
                  }
               }
               if(!ingridientFound)
               {
                  platesContentMatches = false;
               }
            }
            if(platesContentMatches)
            {
               DeliverCorrectRecipeServerRpc(i, playerClientId);
               return;
            }
         }
      }
      DeliverIncorrectRecipeServerRpc();
   }

   [ServerRpc(RequireOwnership = false)]
   private void DeliverIncorrectRecipeServerRpc()
   {
      DeliverIncorrectRecipeClientRpc();
   }

   [ClientRpc]
   private void  DeliverIncorrectRecipeClientRpc()
   {
      OnRecipeFailed?.Invoke(this, EventArgs.Empty);
   }

   [ServerRpc(RequireOwnership = false)]
   private void DeliverCorrectRecipeServerRpc(int waitingRecipeWithCondimentsListIndex, ulong playerClientId)
   {
      int addedPoints = _waitingRecipeWithCondimentsList[waitingRecipeWithCondimentsListIndex].GetRecipeWithCondimentsTotalPoints();
      _totalPoints.Value += addedPoints;

      OnPointsAdded.Invoke(this, new OnPointsAddedEventArgs(addedPoints, playerClientId));

      DeliverCorrectRecipeClientRpc(waitingRecipeWithCondimentsListIndex);
   }

   [ClientRpc]
   private void DeliverCorrectRecipeClientRpc(int waitingRecipeWithCondimentsListIndex)
   {
      _successfulRecipeAmount++;

      OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
      
      _waitingRecipeWithCondimentsList.RemoveAt(waitingRecipeWithCondimentsListIndex);
      
      OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
   }

   public List<RecipeWithCondiments> GetWaitingRecipeWithCondimentsList()
   {
      return _waitingRecipeWithCondimentsList;
   }

   public int GetSuccessfulRecipesAmount()
   {
      return _successfulRecipeAmount;
   }

   public int GetTotalPoints()
   {
      return _totalPoints.Value;
   }

   public bool ArrayContainsValue<T>(T[] array,T value)
   {
      foreach(T varriable in array)
         if(varriable.Equals(value)) return true;
      return false;
   }
}
