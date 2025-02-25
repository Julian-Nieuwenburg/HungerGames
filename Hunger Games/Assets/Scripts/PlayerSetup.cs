using UnityEngine;

public class PlayerSetup : MonoBehaviour
{
    public PlayerMovement movement;
    public GameObject playerCamera;

    public void IsLocalPlayer()
    {
        movement.enabled = true;
        playerCamera.SetActive(true);
    }
}
