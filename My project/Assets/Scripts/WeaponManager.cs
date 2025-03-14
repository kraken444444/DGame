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
    public float weaponOrbitHeight = 2.5f;
    public float weaponOrbitRadius = 1.5f;
    public float rotationSpeed = 20f;

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
                    activeWeapons[i].transform.rotation = weaponSlots[slotIndex].rotation;
                }
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







