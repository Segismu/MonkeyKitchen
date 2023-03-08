using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    [SerializeField] KitchenObjSO kitchenObjectSO;

    public override void Interact(PlayerController player)
    {
        if (!HasKitchenObject())
        {
            if (player.HasKitchenObject())
            {
                player.GetKitchenObj().SetKitchenOnjectParent(this);
            }
            else
            {

            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                if (player.GetKitchenObj() is PlateKitchenObject)
                {
                    PlateKitchenObject plateKitchenObject = player.GetKitchenObj() as PlateKitchenObject;
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObj().GetKitchenObjSO()))
                    {
                        GetKitchenObj().DestroySelf();
                    }
                }
            }
            else
            {
                GetKitchenObj().SetKitchenOnjectParent(player);
            }
        }
    }
}
