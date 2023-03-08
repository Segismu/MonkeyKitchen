using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateKitchenObject : KitchenObj
{
    private List<KitchenObjSO> kitchenObjSOList;

    private void Awake()
    {
        kitchenObjSOList = new List<KitchenObjSO>();
    }

    public bool TryAddIngredient(KitchenObjSO kitchenObjSO)
    {
        if (kitchenObjSOList.Contains(kitchenObjSO))
        {
            return false;
        }
        else
        {
            kitchenObjSOList.Add(kitchenObjSO);
            return true;
        }
    }
}
