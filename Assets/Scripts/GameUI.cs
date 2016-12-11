using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameManager))]
public class GameUI : MonoBehaviour
{
    [SerializeField]
	private Image cooldownImage;

	[SerializeField]
	private GameObject endGameWinGO;

	[SerializeField]
	private GameObject endGameLoseGO;

    // Update is called once per frame
    void Update ()
    {
        if (PlayerControl.LocalPlayerInstance != null)
        {
            cooldownImage.transform.localScale = new Vector3(PlayerControl.LocalPlayerInstance.DashPowerup, 1, 1);
        }
    }

	public void ShowEndGameImage(PlayerControl winner)
	{
		if (PlayerControl.LocalPlayerInstance == winner)
		{
			endGameWinGO.SetActive(true);
		}
		else
		{
			endGameLoseGO.SetActive(true);
		}
		cooldownImage.enabled = false;
	}

	public void OnNewRound()
	{
		endGameWinGO.SetActive(false);
		endGameLoseGO.SetActive(false);
		cooldownImage.enabled = true;
	}
}
