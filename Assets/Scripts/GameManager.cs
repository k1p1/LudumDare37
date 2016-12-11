using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour
{
	static public GameManager Instance;

	[SerializeField]
	Transform[] spawnPoints;

	[SerializeField]
	private GameObject playerPrefab;

	[SerializeField]
	private float freezeTimeNewRound = 3.0f;

	[SerializeField]
	private float countdownToNewRound = 3.0f;

	[SerializeField]
	private GameUI ui;

	void Awake ()
	{
		ui = GetComponent<GameUI>();
	}

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

		NewRound();
	}

	private void CheckWinCondition()
	{
		var pcs = FindObjectsOfType<PlayerControl>().Where(x => !x.IsDead);
		if (pcs.Count() == 1)
		{
			EndRound(pcs.FirstOrDefault());
		}
	}

	private void NewRound()
	{
		if (PhotonNetwork.isMasterClient)
		{
			PlayerControl[] players = FindObjectsOfType<PlayerControl>().ToArray();
			for(int index = 0; index < players.Count(); ++index)
			{
				Vector3 spawnPosition = spawnPoints[index % spawnPoints.Length].position;
				players[index].photonView.RPC("RpcSpawnPlayer", PhotonTargets.All, spawnPosition, freezeTimeNewRound);
			}
		}
		
		ui.OnNewRound();
	}

	private void EndRound(PlayerControl winner)
	{
		ui.ShowEndGameImage(winner);
		Invoke("NewRound", countdownToNewRound);
	}

	private void OnTriggerEnter(Collider other)
	{
		PlayerControl pc = other.GetComponent<PlayerControl>();
		if (pc)
		{
			pc.Die();
			CheckWinCondition();
		}
	}
}
