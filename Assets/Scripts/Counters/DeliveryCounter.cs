using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    public override void Interact(PlayerController player)
    {
        if (player.HasKitchenObject())
        {
            if (player.GetKitchenObj().TryGetPlate(out PlateKitchenObject plateKitchenObject))
            {
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);

                KitchenObj.DestroyKitchenObject(player.GetKitchenObj());
            }
        }
    }
}
