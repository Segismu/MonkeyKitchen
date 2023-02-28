using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ContainerCounter : BaseCounter
{
    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] KitchenObjSO kitchenObjectSO;

    public override void Interact(PlayerController player)
    {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);
        //kitchenObjectTransform.localPosition = Vector3.zero;
        kitchenObjectTransform.GetComponent<KitchenObj>().SetKitchenOnjectParent(player);

        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
    }
}
