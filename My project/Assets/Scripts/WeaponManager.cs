using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main component for managing the player's weapons
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Main component for managing the player's weapons
public class WeaponManager : MonoBehaviour
{
    [Header("Weapon Settings")]
    public int maxWeapons = 6;
    
    [Header("Positioning")]
    public bool useManualPositions = true;
    public Transform[] weaponSlots; // Array of transform slots for weapons
    
    [Header("Auto-Positioning Fallback")]
    public float weaponOrbitHeight = 2.5f;
    public float weaponOrbitRadius = 1.5f;
    public float rotationSpeed = 20f;

    private List<Weapon> activeWeapons = new List<Weapon>();
    private float orbitAngle = 0f;

    void Start()
    {
        // If no weapon slots are manually assigned, try to find child objects with "WeaponSlot" in their name
        if (weaponSlots == null || weaponSlots.Length == 0)
        {
            // Find all child transforms with "WeaponSlot" in their name
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
            // Use manually specified weapon slots
            for (int i = 0; i < activeWeapons.Count; i++)
            {
                // Get slot index, clamped to available slots
                int slotIndex = Mathf.Min(i, weaponSlots.Length - 1);
                
                if (weaponSlots[slotIndex] != null)
                {
                    // Set the weapon's position and rotation to match the slot
                    activeWeapons[i].transform.position = weaponSlots[slotIndex].position;
                    activeWeapons[i].transform.rotation = weaponSlots[slotIndex].rotation;
                }
            }
        }
        else
        {
            // Fallback to rainbow/arc formation
            float angleStep = 180f / Mathf.Max(activeWeapons.Count - 1, 1);
            
            for (int i = 0; i < activeWeapons.Count; i++)
            {
                float angle = -90f + (i * angleStep); 
                float radians = angle * Mathf.Deg2Rad;
                
                // Position in a semicircle above the player
                float xPos = Mathf.Cos(radians) * weaponOrbitRadius;
                float yPos = Mathf.Sin(radians) * weaponOrbitRadius + weaponOrbitHeight;
                
                Vector3 targetPosition = new Vector3(xPos, yPos, 0);
                activeWeapons[i].transform.localPosition = targetPosition;
                
                // Make weapon look at player (optional)
                activeWeapons[i].transform.LookAt(transform.position);
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
        
        // Instantiate weapon as child of player
        GameObject weaponObj = Instantiate(weaponPrefab, transform);
        Weapon weapon = weaponObj.GetComponent<Weapon>();
        
        if (weapon != null)
        {
            weapon.Initialize(transform);
            activeWeapons.Add(weapon);
            
            // Update positions immediately
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







