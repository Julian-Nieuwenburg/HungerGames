using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    public string roomName { get; internal set; }

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void JoinRoomButtonPressed()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.JoinOrCreateRoom("RoomName", new RoomOptions { MaxPlayers = 4 }, TypedLobby.Default);
        }
        else
        {
            Debug.LogError("PhotonNetwork is not connected and ready.");
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server.");
        // Now you can join or create a room
        JoinRoomButtonPressed();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined room successfully.");
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        Vector3 spawnPosition = new Vector3(0, 0, 0); // Set your spawn position
        Quaternion spawnRotation = Quaternion.identity; // Set your spawn rotation

        if (playerPrefab != null)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, spawnRotation);
        }
        else
        {
            Debug.LogError("Player prefab is not set in the RoomManager.");
        }
    }
}
