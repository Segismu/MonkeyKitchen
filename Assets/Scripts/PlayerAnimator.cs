using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PlayerAnimator : NetworkBehaviour
{
    private const string isWalking = "IsWalking";

    private Animator animator;

    [SerializeField] PlayerController player;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!IsOwner) { return; }

        animator.SetBool(isWalking, player.IsWalking());
    }
}
