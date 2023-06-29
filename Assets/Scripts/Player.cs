using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class Player : NetworkBehaviour, IKitchenObjectParent
{
   public static event EventHandler OnAnyPlayerSpawned;
   public static event EventHandler OnAnyPickedSomething;

   public static void ResetStaticData()
   {
      OnAnyPlayerSpawned = null;
      OnAnyPickedSomething = null;
   }


   public static Player LocalInstance { get; private set; }


   public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;
   public class OnSelectedCounterChangedEventArgs : EventArgs
   {
      public readonly BaseCounter SelectedCounter;

      public OnSelectedCounterChangedEventArgs(BaseCounter selectedCounter)
      {
         SelectedCounter = selectedCounter;
      }
   }


   public bool IsWalking { get; private set; }


   [SerializeField] private LayerMask _countersLayerMask;
   [SerializeField] private LayerMask _collisionsLayerMask;
   [SerializeField] private Transform _kitchenObjectHoldPoint;
   [SerializeField] private List<Vector3> _spawnPositionList;
   [SerializeField] private PlayerVisual _playerVisual;



   private float _movementSpeed = 7f;
   private float _rotationSpeed = 5f;
   private BaseCounter _selectedCounter;
   private KitchenObject _kitchenObject;



   private void Start()
   {
      GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
      GameInput.Instance.OnIntearctAlternativeAction += GameInput_OnInteractAlternativeAction;

      PlayerData playerData = GameMultiplayer.Instance.GetPlayerDataFromClientId(OwnerClientId);
      _playerVisual.SetPlayerColor(GameMultiplayer.Instance.GetPlayerColor(playerData.colorId));
   }

   public override void OnNetworkSpawn()
   {
      if(IsOwner)
         LocalInstance = this;
      
      transform.position = _spawnPositionList[GameMultiplayer.Instance.GetPlayerDataIndexFromClientId(OwnerClientId)];

      OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);

      if(IsServer)
         NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
   }

   private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
   {
      if(clientId == OwnerClientId && HasKitchenObject())
      {
         KitchenObject.DestroyKitchenObject(GetKitchenObject());
      }
   }

   private void Update()
   {
      if(!IsOwner) return;

      HandleMovement();
      HandleInteractions();
   }

   

   
   private void GameInput_OnInteractAlternativeAction(object sender, EventArgs e)
   {
      if(!GameManager.Instance.IsGamePlaying() || GameManager.Instance.IsGamePaused()) return;

      if(_selectedCounter != null && _selectedCounter is IInteractAlternative)
      {
         IInteractAlternative interactAlternativeCounter = _selectedCounter as IInteractAlternative;
         
         interactAlternativeCounter.InteractAlternative(this);
      }

   }

   private void GameInput_OnInteractAction(object sender, EventArgs e)
   {
      if(!GameManager.Instance.IsGamePlaying() || GameManager.Instance.IsGamePaused()) return;

      if(_selectedCounter != null)
         _selectedCounter.Interact(this);
   }


   private void HandleMovement()
   {
      Vector3 movementDirection = new Vector3(GameInput.Instance.GetMovementVectorNormalized.x, 0f, GameInput.Instance.GetMovementVectorNormalized.y);

      float moveDistance = _movementSpeed * Time.deltaTime;

      if(!TryMove(movementDirection, moveDistance))
      {
         Vector3 moveDirectionX = (movementDirection.x * Vector3.right).normalized;

         if(moveDirectionX.x < -.5f || moveDirectionX.x > +.5f)
         {
            if(!TryMove(moveDirectionX, moveDistance))
            {
               Vector3 moveDirectionZ = (movementDirection.z * Vector3.forward).normalized;

               if(moveDirectionZ.z < -.5f || moveDirectionZ.z > +.5f)
                  TryMove(moveDirectionZ, moveDistance); 
            }
         }
      }
      
      IsWalking = movementDirection != Vector3.zero;

      if(IsWalking)
         transform.forward = Vector3.Slerp(transform.forward, movementDirection, _rotationSpeed);
   }

   private bool TryMove(Vector3 movementDirection, float moveDistance)
   {
      float playerRadius = .65f;
      
      bool canMove = !Physics.BoxCast(transform.position, Vector3.one * playerRadius, movementDirection, Quaternion.identity, moveDistance, _collisionsLayerMask);

      if(canMove) transform.position +=  movementDirection * moveDistance;

      return canMove;
   }

   private void HandleInteractions()
   {
      float interactDistance = 2f;

      if(Physics.Raycast(transform.position, transform.forward, out RaycastHit raycastHit, interactDistance, _countersLayerMask))
      {
         if(raycastHit.transform.TryGetComponent(out BaseCounter hitedCounter))
            SetSelectedCounter(hitedCounter);
         else
            SetSelectedCounter(null);
      }
      else 
      {
         SetSelectedCounter(null);
      }
   }
   
   private void SetSelectedCounter(BaseCounter baseCounter)
   {
      if(_selectedCounter != baseCounter)
      {
         _selectedCounter = baseCounter;
         OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs(_selectedCounter));
      } 
   }

   public bool HasKitchenObject()
   {
      return _kitchenObject != null;
   }

   public void ClearKitchenObject()
   {
      _kitchenObject = null;
   }

   public void SetKitchenObject(KitchenObject kitchenObject)
   {
      _kitchenObject = kitchenObject;

      if(_kitchenObject != null)
      {
         OnAnyPickedSomething?.Invoke(this, EventArgs.Empty);
      }
   }

   public Transform GetKitchenObjectTargetPosition()
   {
      return _kitchenObjectHoldPoint;
   }

   public KitchenObject GetKitchenObject()
   {
      return _kitchenObject;
   }

   public NetworkObject GetNetworkObject()
   {
      return NetworkObject;
   }

   public ulong ClientId()
   {
      return OwnerClientId;
   }
}
