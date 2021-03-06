﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Launcher : Photon.PunBehaviour
{
	private bool isConnectedToMaster = false;

	[SerializeField]
	byte maxPlayers = 20;

	LauncherUI ui;

	void Start ()
	{
		ui = GetComponent<LauncherUI>();

        PhotonNetwork.autoJoinLobby = false;
		PhotonNetwork.automaticallySyncScene = true;

		if (!PhotonNetwork.ConnectUsingSettings("v4.2"))
		{
			return;
		}
	}

	public void SetPlayerName(string name)
	{
		PlayerPrefs.SetString(GameManager.NameKey, name);
	}

	public override void OnConnectedToMaster()
	{
		isConnectedToMaster = true;

		ui.PlayButtonInteractable = true;
		ui.PlayButtonText = "Play";
	}
		
	public override void OnDisconnectedFromPhoton()
	{
		Debug.LogError("Disconected from Photon");
	}

	public void CreateOrJoinRoom()
	{
		if (isConnectedToMaster)
		{
			RoomOptions roomOptions = new RoomOptions() { IsVisible = false, MaxPlayers = maxPlayers };
			if (!PhotonNetwork.JoinOrCreateRoom("TheRoom", roomOptions, TypedLobby.Default))
			{
				return;
			}
		}
		else
		{
			Debug.LogError("Not connected to Master");
		}
	}

	public override void OnJoinedRoom()
	{
		Debug.Log("OnJoinedRoom() called by PUN");

		if (PhotonNetwork.room.playerCount == 1)
		{
			PhotonNetwork.LoadLevel("Level1");
		}
	}


	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	public void QuitApplication()
	{
		Application.Quit();
	}
}
