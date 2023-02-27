using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour
{
    [SerializeField] KitchenObjSO kitchenObjectSO;
    [SerializeField] Transform counterTopPoint;

    public void Interact()
    {
        Debug.Log("Opi!");
        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, counterTopPoint);
        kitchenObjectTransform.localPosition = Vector3.zero;

        Debug.Log(kitchenObjectTransform.GetComponent<KitchenObj>().GetKitchenObjSO().objectName);
    }
}
