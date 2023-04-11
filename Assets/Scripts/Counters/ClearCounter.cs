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
                if (player.GetKitchenObj().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObj().GetKitchenObjSO()))
                    {
                        KitchenObj.DestroyKitchenObject(GetKitchenObj());
                    }
                }
                else
                {
                    if (GetKitchenObj().TryGetPlate(out plateKitchenObject))
                    {
                        if (plateKitchenObject.TryAddIngredient(player.GetKitchenObj().GetKitchenObjSO()))
                        {
                            KitchenObj.DestroyKitchenObject(player.GetKitchenObj());
                        }
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
