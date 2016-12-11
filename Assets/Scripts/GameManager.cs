using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.MonoBehaviour
{
	static public GameManager Instance;

	[SerializeField]
	Transform[] spawnPoints;

	[SerializeField]
	private GameObject playerPrefab;


    void Start () 
	{
		Instance = this;

		// in case we started this demo with the wrong scene being active, simply load the menu scene
		if (!PhotonNetwork.connected)
		{
			SceneManager.LoadScene("Launcher");
			return;
		}

		if (PlayerControl.LocalPlayerInstance==null)
		{
			Debug.Log("We are Instantiating LocalPlayer from "+SceneManagerHelper.ActiveSceneName);

			// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
			int localPlayerIndexPlayerInList = Array.IndexOf(PhotonNetwork.playerList, PhotonNetwork.player);
			Vector3 spawnPosition = spawnPoints[localPlayerIndexPlayerInList % spawnPoints.Length].position;
			PhotonNetwork.Instantiate(this.playerPrefab.name, spawnPosition, Quaternion.identity, 0);
		}else{

			Debug.Log("Ignoring scene load for "+ SceneManagerHelper.ActiveSceneName);
		}
	}
}
