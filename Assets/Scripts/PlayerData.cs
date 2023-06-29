using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public struct PlayerData : IEquatable<PlayerData>, INetworkSerializable
{
   public ulong clientId;
   public int colorId;
   public int playerPoints;
   public int playerSuccessRecipesAmount;
   public FixedString64Bytes playerName;
   public FixedString64Bytes playerId;



   public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
   {
      serializer.SerializeValue(ref clientId);
      serializer.SerializeValue(ref colorId);
      serializer.SerializeValue(ref playerName);
      serializer.SerializeValue(ref playerId);
      serializer.SerializeValue(ref playerPoints);
      serializer.SerializeValue(ref playerSuccessRecipesAmount);
   }

   bool IEquatable<PlayerData>.Equals(PlayerData other)
   {
      return 
         clientId == other.clientId && 
         colorId == other.colorId &&
         playerName == other.playerName &&
         playerId == other.playerId &&
         playerPoints == other.playerPoints &&
         playerSuccessRecipesAmount == other.playerSuccessRecipesAmount;
   }
}
