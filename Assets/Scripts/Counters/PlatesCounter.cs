using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlatesCounter : BaseCounter
{
   public event EventHandler OnPlateSpawned;
   public event EventHandler OnPlateRemoved;

   [SerializeField] private KitchenObjectSO _plateKitchenObjectSO;


   private float _spawnPlateTimer;
   private float _spawnPlateTimerMax = 4f;
   private int _spawnedPlatesAmount;
   private int _spawnedPlatesAmountMax = 4;


   private void Update()
   {
      if(!IsServer) return;

      if(_spawnedPlatesAmountMax > _spawnedPlatesAmount && GameManager.Instance.IsGamePlaying())
      {
         _spawnPlateTimer += Time.deltaTime;
         if(_spawnPlateTimer >= _spawnPlateTimerMax)
         {
            _spawnPlateTimer = 0f;

            SpawnPlateServerRpc();
         }
      }
   }

   [ServerRpc]
   private void SpawnPlateServerRpc()
   {
      SpawnPlateClientRpc();
   }

   [ClientRpc]
   private void SpawnPlateClientRpc()
   {
      _spawnedPlatesAmount++;

      OnPlateSpawned?.Invoke(this, EventArgs.Empty);
   }

   public override void Interact(Player player)
   {
      if(_spawnedPlatesAmount > 0 && !player.HasKitchenObject())
      {
         KitchenObject.SpawnKitchenObject(_plateKitchenObjectSO, player);

         InteractLogicServerRpc(player.NetworkObject);
      }
   }

   [ServerRpc(RequireOwnership = false)]
   private void InteractLogicServerRpc(NetworkObjectReference playerNetworkObjectReference)
   {
      playerNetworkObjectReference.TryGet(out NetworkObject playerNetworkObject);
      Player player = playerNetworkObject.GetComponent<Player>();

      if(_spawnedPlatesAmount > 0 && !player.HasKitchenObject())
         InteractLogicClientRpc();
   }

   [ClientRpc]
   private void InteractLogicClientRpc()
   {
      _spawnedPlatesAmount--;

      OnPlateRemoved?.Invoke(this, EventArgs.Empty);
   }

}

