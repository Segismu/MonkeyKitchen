using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }

    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;

    private State state;
    private NetworkVariable <float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable <float> burningTimer = new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private void Start()
    {
        state = State.Idle;
    }

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
    }

    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float fryingTimerMax = fryingRecipeSO != null ? fryingRecipeSO.fryingTimerMax : 1f;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer.Value / fryingRecipeSO.fryingTimerMax
        });
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (HasKitchenObject())
        {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer.Value += Time.deltaTime;

                    

                    if (fryingTimer.Value > fryingRecipeSO.fryingTimerMax)
                    {
                        KitchenObj.DestroyKitchenObject(GetKitchenObj());

                    KitchenObj.SpawnKitchenObject(fryingRecipeSO.output, this);

                    state = State.Fried;
                    burningTimer = 0f;
                    burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObj().GetKitchenObjSO());

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });
                    }
                    break;
            case State.Fried:
                    burningTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = burningTimer / burningRecipeSO.burningTimerMax
                    });

                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {
                        KitchenObj.DestroyKitchenObject(GetKitchenObj());

                        KitchenObj.SpawnKitchenObject(burningRecipeSO.output, this);

                        state = State.Burned;


                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        }); ;
                    }
                    break;
            case State.Burned:
                break;
             }
        }
    }

    public override void Interact(PlayerController player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                if (HasRecipeWithInput(player.GetKitchenObj().GetKitchenObjSO()))
                { 
                    KitchenObj kitchenObj = player.GetKitchenObj();

                    kitchenObj.SetKitchenOnjectParent(this);

                    InteracrLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetKitchenObjectSOIndex(kitchenObj.GetKitchenObjSO()));
                }
            }
            else
            {

            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObj().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObj().GetKitchenObjSO()))
                    {
                        GetKitchenObj().DestroySelf();

                        state = State.Idle;


                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                        {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                        {
                            progressNormalized = 0f
                        }); ;
                    }
                }
            }
            else
            {
                GetKitchenObj().SetKitchenOnjectParent(player);

                state = State.Idle;


                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
                {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                }); ;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteracrLogicPlaceObjectOnCounterServerRpc(int kitchenObjectSOIndex)
    {
        fryingTimer.Value = 0f;

        InteracrLogicPlaceObjectOnCounterClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void InteracrLogicPlaceObjectOnCounterClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjSO kitchenObjSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjSO);

        state = State.Frying;

        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state
        });
    }

    private bool HasRecipeWithInput(KitchenObjSO inputKitchenObjectSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);
        return fryingRecipeSO != null;
    }

    private KitchenObjSO GetOutputForInput(KitchenObjSO inputKitchenObjectSO)
    {

        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputKitchenObjectSO);

        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }

    private FryingRecipeSO GetFryingRecipeSOWithInput(KitchenObjSO inputKitchenObjectSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputKitchenObjectSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }

    private BurningRecipeSO GetBurningRecipeSOWithInput(KitchenObjSO inputKitchenObjectSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputKitchenObjectSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }
}
