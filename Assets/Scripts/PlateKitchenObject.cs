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

    public void AddIngredient(KitchenObjSO kitchenObjSO)
    {
        kitchenObjSOList.Add(kitchenObjSO);
    }
}
