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

	[SerializeField]
	GameObject inputPanel;

	[SerializeField]
	Text inputPanelText;

	private Launcher launcher;

	public bool PlayButtonInteractable
	{
		set { playButton.interactable = value; }
	}

	public string PlayButtonText
	{
		set { playButtonText.text = value; }
	}

	void Start()
	{
		launcher = GetComponent<Launcher>();
		inputPanel.SetActive("default string" == PlayerPrefs.GetString(GameManager.NameKey, "default string"));
	}

	public void OnInputButtonPressed()
	{
		launcher.SetPlayerName(inputPanelText.text);
	}
}
