using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private PlayerController player;
    private float footstepsTimer;
    private float footstepsTimerMax = .1f;

    private void Awake()
    {
        player = GetComponent<PlayerController>();
    }

    private void Update()
    {
        footstepsTimer -= Time.deltaTime;
        if(footstepsTimer < 0f) {
            footstepsTimer = footstepsTimerMax;

            if(player.IsWalking())
            {
                float volume = 1f;
                SoundManager.Instance.PlayFootstepSound(player.transform.position, volume);
            }
        }
    }
}
