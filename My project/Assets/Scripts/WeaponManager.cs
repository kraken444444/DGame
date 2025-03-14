using UnityEngine;
using System.Collections.Generic;
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int maxWeapons = 6;
    
    [Header("Positioning")]
    public bool useManualPositions = true;
    public Transform[] weaponSlots;
    
    [Header("Auto-Positioning Fallback")]
    public float rotationSpeed = 20f;
    public float orbitRadius = 2f; // Added for auto-positioning

    private List<Weapon> activeWeapons = new List<Weapon>();
    private float orbitAngle = 0f;

    void Start()
    {
        if (weaponSlots == null || weaponSlots.Length == 0)
        {
            Transform[] childSlots = transform.GetComponentsInChildren<Transform>();
            List<Transform> slots = new List<Transform>();
            
            foreach(Transform child in childSlots)
            {
                if (child != transform && child.name.Contains("WeaponSlot"))
                {
                    slots.Add(child);
                }
            }
            
            if (slots.Count > 0)
            {
                weaponSlots = slots.ToArray();
                useManualPositions = true;
            }
        }
    }

    void Update()
    {
        orbitAngle += rotationSpeed * Time.deltaTime;
        UpdateWeaponPositions();
    }

    void UpdateWeaponPositions()
    {
        if (activeWeapons.Count == 0) return;
        
        if (useManualPositions && weaponSlots != null && weaponSlots.Length > 0)
        {
            for (int i = 0; i < activeWeapons.Count; i++)
            {
                int slotIndex = Mathf.Min(i, weaponSlots.Length - 1);
                
                if (weaponSlots[slotIndex] != null)
                {
                    activeWeapons[i].transform.position = weaponSlots[slotIndex].position;
                    // Don't force rotation here - let the Weapon handle its own rotation for targeting
                }
            }
        }
        else
        {
            // Auto-position weapons in a circle if no manual slots are available
            float angleStep = 360f / activeWeapons.Count;
            
            for (int i = 0; i < activeWeapons.Count; i++)
            {
                float angle = orbitAngle + i * angleStep;
                float x = transform.position.x + orbitRadius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float z = transform.position.z + orbitRadius * Mathf.Sin(angle * Mathf.Deg2Rad);
                
                Vector3 newPos = new Vector3(x, transform.position.y, z);
                activeWeapons[i].transform.position = newPos;
                // Again, don't set rotation - let the Weapon handle it
            }
        }
    }

    public void AddWeapon(GameObject weaponPrefab)
    {
        if (activeWeapons.Count >= maxWeapons)
        {
            Debug.Log("Maximum weapons reached!");
            return;
        }
        
        GameObject weaponObj = Instantiate(weaponPrefab, transform);
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        
        if (weapon != null)
        {
            // Make sure auto-targeting is enabled
            weapon.autoTarget = true;
            
            // Initialize the weapon with this transform so it knows the player position
            weapon.Initialize(transform);
            
            activeWeapons.Add(weapon);
            UpdateWeaponPositions();
        }
    }
    
    public void RemoveWeapon(Weapon weapon)
    {
        if (activeWeapons.Contains(weapon))
        {
            activeWeapons.Remove(weapon);
            Destroy(weapon.gameObject);
            UpdateWeaponPositions();
        }
    }
}





