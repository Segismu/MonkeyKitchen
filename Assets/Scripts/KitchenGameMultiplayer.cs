using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private KitchenObjListSO kitchenObjectListSO;

    private void Awake()
    {
        Instance = this;
    }


    public void SpawnKitchenObject(KitchenObjSO kitchenObjSO, IKitchenObjectParent kitchenObjectParent)
    {
        SpawnKitchenObjectServerRpc(GetKitchenObjectSOIndex(kitchenObjSO), kitchenObjectParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnKitchenObjectServerRpc(int kitchenObjSOIndex, NetworkObjectReference kitchenObjectParentNetworkObjectReference)
    {
        KitchenObjSO kitchenObjectSO = GetKitchenObjectSOFromIndex(kitchenObjSOIndex);

        Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab);

        NetworkObject kitchenObjectNetworkObject = kitchenObjectTransform.GetComponent<NetworkObject>();
        kitchenObjectNetworkObject.Spawn(true);
        KitchenObj kitchenObject = kitchenObjectTransform.GetComponent<KitchenObj>();

        kitchenObjectParentNetworkObjectReference.TryGet(out NetworkObject kitchenObjectParentNetworkObject);
        IKitchenObjectParent kitchenObjectParent = kitchenObjectParentNetworkObject.GetComponent<IKitchenObjectParent>();

        kitchenObject.SetKitchenOnjectParent(kitchenObjectParent);
    }

    private int GetKitchenObjectSOIndex(KitchenObjSO kitchenObjectSO)
    {
        return kitchenObjectListSO.kitchenObjSOList.IndexOf(kitchenObjectSO);
    }

    private KitchenObjSO GetKitchenObjectSOFromIndex(int kitchenObjSOIndex)
    {
        return kitchenObjectListSO.kitchenObjSOList[kitchenObjSOIndex];
    }

    public void DestroyKitchenObject(KitchenObj kitchenObj)
    {
        DestroyKitchenObjectServerRpc(kitchenObj.NetworkObject);
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyKitchenObjectServerRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kichenObjectNetworkObject);
        KitchenObj kitchenObject = kichenObjectNetworkObject.GetComponent<KitchenObj>();

        ClearKitchenObjectClientRpc(kitchenObjectNetworkObjectReference);
        kitchenObject.DestroySelf();
    }

    [ClientRpc]
    private void ClearKitchenObjectClientRpc(NetworkObjectReference kitchenObjectNetworkObjectReference)
    {
        kitchenObjectNetworkObjectReference.TryGet(out NetworkObject kichenObjectNetworkObject);
        KitchenObj kitchenObject = kichenObjectNetworkObject.GetComponent<KitchenObj>();

        kitchenObject.ClearKitchenObjectOnParent();
    }
}
