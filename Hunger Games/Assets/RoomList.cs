using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;


public class RoomList : MonoBehaviourPunCallbacks
{

    public static RoomList Instance;

    public GameObject roomManagerGameObject;
    public RoomManager roomManager;

    [Header("UI")]
    public Transform roomListParent;
    public GameObject roomListItemPrefab;

    private List<RoomInfo> cashedRoomList = new List<RoomInfo>();


    public void ChangeRoomToCreateName(string _roomName)
    {
        roomManager.roomNameToJoin = _roomName;
    }


    private void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }

        yield return new WaitUntil(() => !PhotonNetwork.IsConnected);


        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();

        PhotonNetwork.JoinLobby();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomlist)
    {
        if(cashedRoomList.Count <= 0)
        {
            cashedRoomList = roomlist;
        } 
        else
        {
            foreach(var room in roomlist)
            {
                for (int i = 0; i < cashedRoomList.Count; i++)
                {
                    if(cashedRoomList[i].Name == room.Name)
                    {
                        List<RoomInfo> newList = cashedRoomList;

                        if (room.RemovedFromList)
                        {
                            newList.Remove(newList[i]);
                        }
                        else
                        {
                            newList[i] = room;

                            cashedRoomList = newList;
                        }
                    }
                }
            }
        }

    
        updateUI();
    }

    void updateUI()
    {
        foreach (Transform roomItem in roomListParent)
        {
            Destroy(roomItem.gameObject);
        }

        foreach (var room in cashedRoomList)
        {
            GameObject roomItem = Instantiate(roomListItemPrefab, roomListParent);

            roomItem.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = room.Name;
            roomItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = room.PlayerCount + "/12";

            roomItem.GetComponent<RoomItemButton>().Roomname = room.Name;
        }
    }

    public void JoinRoomByName(string name)
    {
        roomManager.roomNameToJoin = name;
        roomManagerGameObject.SetActive(true);
    }
}
