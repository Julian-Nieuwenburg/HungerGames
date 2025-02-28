using UnityEngine;

public class RoomItemButton : MonoBehaviour
{
    public string Roomname;
    public void OnButtonPressed()
    {
        RoomList.Instance.JoinRoomByName(Roomname);
    }
}
