using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField]
    private Image CooldownImage;
    private PlayerControl player;
	// Use this for initialization
	void Awake ()
    {
       // BumperControl.PlayerSpawned += BumperSpawned;
	}

    private void PlayerSpawned(PlayerControl bumper)
    {
        player = bumper;
    }

    // Update is called once per frame
    void Update ()
    {
        if (player != null)
        {
            CooldownImage.transform.localScale = new Vector3(player.DashPowerup, 1, 1);
        }
    }
}
