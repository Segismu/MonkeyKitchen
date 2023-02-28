using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKitchenObjectParent
{
    public Transform GetKitchenObjectFollowTransform();

    public void SetKitchenOnject(KitchenObj kitchenObj);

    public KitchenObj GetKitchenObj();

    public void ClearKitchenObject();

    public bool HasKitchenObject();
}
