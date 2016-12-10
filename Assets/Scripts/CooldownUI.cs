using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField]
    private Image CooldownImage;

    // Update is called once per frame
    void Update ()
    {
        if (PlayerControl.LocalPlayerInstance != null)
        {
            CooldownImage.transform.localScale = new Vector3(PlayerControl.LocalPlayerInstance.DashPowerup, 1, 1);
        }
    }
}
