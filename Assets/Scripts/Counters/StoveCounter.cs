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
    private float fryingTimer;
    private float burningTimer;
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    private void Start()
    {
        state = State.Idle;
    }

    private void Update()
    {
        if (HasKitchenObject())
        {
        switch (state)
        {
            case State.Idle:
                break;
            case State.Frying:
                fryingTimer += Time.deltaTime;

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
                    });

                    if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                    GetKitchenObj().DestroySelf();

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
                        GetKitchenObj().DestroySelf();

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
        InteracrLogicPlaceObjectOnCounterClientRpc(kitchenObjectSOIndex);
    }

    [ClientRpc]
    private void InteracrLogicPlaceObjectOnCounterClientRpc(int kitchenObjectSOIndex)
    {
        KitchenObjSO kitchenObjSO = KitchenGameMultiplayer.Instance.GetKitchenObjectSOFromIndex(kitchenObjectSOIndex);
        fryingRecipeSO = GetFryingRecipeSOWithInput(kitchenObjSO);

        state = State.Frying;
        fryingTimer = 0f;


        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state
        });

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
        {
            progressNormalized = fryingTimer / fryingRecipeSO.fryingTimerMax
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
