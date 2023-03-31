using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{

    public static event EventHandler OnAnyObjectTrashed;

    new public static void ResetStaticData()
    {
        OnAnyObjectTrashed = null;
    }

    public override void Interact(PlayerController player)
    {
        if (player.HasKitchenObject())
        {
            KitchenObj.DestroyKitchenObject(player.GetKitchenObj());

            InteractLogicServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }

    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnAnyObjectTrashed?.Invoke(this, EventArgs.Empty);
    }
}
