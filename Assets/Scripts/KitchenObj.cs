using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenObj : MonoBehaviour
{
    [SerializeField] private KitchenObjSO kitchenObjSO;

    public KitchenObjSO GetKitchenObjSO()
    {
        return kitchenObjSO;
    }
}
