using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public GameObject weaponPrefab;
    public float rotationSpeed = 90f;
    public float bobAmount = 0.2f;
    public float bobSpeed = 2f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        float yOffset = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        transform.position = startPosition + new Vector3(0, yOffset, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            WeaponManager weaponManager = other.GetComponent<WeaponManager>();

            if (weaponManager != null)
            {
                weaponManager.AddWeapon(weaponPrefab);

                Destroy(gameObject);
            }
        }
    }
}
