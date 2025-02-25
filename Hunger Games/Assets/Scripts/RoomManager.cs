using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public GameObject player;
    [Space]
    public Transform[] spawnPoints;
    private List<int> availableSpawnIndices = new List<int>();

    [Space]
    public GameObject roomCam;

    void Start()
    {
        Debug.Log("Connecting...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        PhotonNetwork.JoinOrCreateRoom("test", null, null);
        Debug.Log("We zitten nu in een room");
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        roomCam.SetActive(false);

        // Vul de lijst met alle beschikbare spawn indices
        if (PhotonNetwork.IsMasterClient)
        {
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                availableSpawnIndices.Add(i);
            }
        }

        StartCoroutine(SpawnPlayer());
    }

    IEnumerator SpawnPlayer()
    {
        while (availableSpawnIndices.Count == 0)
        {
            yield return null;
        }

        int randomIndex = availableSpawnIndices[Random.Range(0, availableSpawnIndices.Count)];
        availableSpawnIndices.Remove(randomIndex);

        Vector3 spawnPosition = spawnPoints[randomIndex].position;
        GameObject _player = PhotonNetwork.Instantiate(this.player.name, spawnPosition, Quaternion.identity);
        _player.GetComponent<PlayerSetup>().IsLocalPlayer();
    }
}
