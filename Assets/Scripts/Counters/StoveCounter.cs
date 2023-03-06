using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter
{
    private enum State
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

                if (fryingTimer > fryingRecipeSO.fryingTimerMax)
                    {
                    GetKitchenObj().DestroySelf();

                    KitchenObj.SpawnKitchenObject(fryingRecipeSO.output, this);

                    Debug.Log("Object fried!");
                    state = State.Fried;
                    burningTimer = 0f;
                    burningRecipeSO = GetBurningRecipeSOWithInput(GetKitchenObj().GetKitchenObjSO());

                    }
                    break;
            case State.Fried:
                    burningTimer += Time.deltaTime;

                    if (burningTimer > burningRecipeSO.burningTimerMax)
                    {
                        GetKitchenObj().DestroySelf();

                        KitchenObj.SpawnKitchenObject(burningRecipeSO.output, this);

                        Debug.Log("Object burned!");

                        state = State.Burned;
                    }
                    break;
            case State.Burned:
                break;
        }

            Debug.Log(state);
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
                    player.GetKitchenObj().SetKitchenOnjectParent(this);

                    fryingRecipeSO = GetFryingRecipeSOWithInput(GetKitchenObj().GetKitchenObjSO());

                    state = State.Frying;
                    fryingTimer = 0f;
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

            }
            else
            {
                GetKitchenObj().SetKitchenOnjectParent(player);
            }
        }
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
