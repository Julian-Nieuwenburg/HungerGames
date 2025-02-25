using UnityEngine;

public class Pickup : MonoBehaviour
{
    private Transform heldObject;
    private Vector3 offset;

    public GameObject bulletPrefab; // Prefab for the bullet
    public Transform firePoint; // Point from where the bullet will be fired
    public float bulletSpeed = 10f; // Speed of the bullet
    public float fireRate = 0.5f; // Time between shots
    private float nextFireTime = 0f; // Time when the player can fire next

    void Update()
    {
        if (heldObject != null)
        {
            MoveHeldObject();
            CheckDrop();
        }
        else
        {
            CheckPickup();
        }

        CheckShoot();
    }

    void MoveHeldObject()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.WorldToScreenPoint(heldObject.position).z; // Maintain the Z position
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        heldObject.position = worldPosition + offset;
    }

    void CheckPickup()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider != null && hit.collider.CompareTag("Pickup"))
                {
                    heldObject = hit.transform;
                    offset = heldObject.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.WorldToScreenPoint(heldObject.position).z));
                }
            }
        }
    }

    void CheckDrop()
    {
        if (Input.GetMouseButtonDown(1))
        {
            heldObject = null;
        }
    }

    void CheckShoot()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * bulletSpeed;
    }
}
