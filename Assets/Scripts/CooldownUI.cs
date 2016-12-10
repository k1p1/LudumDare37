using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField]
    private Image CooldownImage;
    private BumperControl player;
	// Use this for initialization
	void Awake ()
    {
       // BumperControl.PlayerSpawned += BumperSpawned;
	}

    private void BumperSpawned(BumperControl bumper)
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
