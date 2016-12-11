using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Launcher))]
public class LauncherUI : MonoBehaviour 
{
	[SerializeField]
	Button playButton;

	[SerializeField]
	Text playButtonText;

	public bool PlayButtonInteractable
	{
		set { playButton.interactable = value; }
	}

	public string PlayButtonText
	{
		set { playButtonText.text = value; }
	}
}
