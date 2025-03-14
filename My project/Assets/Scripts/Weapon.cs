using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public string weaponName = "Default Weapon";
    public float damage = 20f;
    public float attackRate = 1f; // Attacks per second
    public float attackRange = 10f;
    public GameObject projectilePrefab; // For projectile weapons
    
    [Header("Visual")]
    public bool rotateWeapon = true;
    public float rotationSpeed = 90f;
    
    private float attackTimer;
    private Transform playerTransform;
    
    public void Initialize(Transform player)
    {
        playerTransform = player;
        attackTimer = Random.Range(0f, 1f/attackRate); // Randomize initial attack time
    }
    
    void Update()
    {
        if (rotateWeapon)
        {
            transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        }
        
        // Attack timer
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            FindAndAttackEnemy();
            attackTimer = 1f / attackRate; // Reset timer based on attack rate
        }
    }
    
    void FindAndAttackEnemy()
    {
        // Find all enemies in range
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, attackRange);
        
        // Filter for enemies and sort by distance
        List<Transform> enemies = new List<Transform>();
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                enemies.Add(hitCollider.transform);
            }
        }
        
        // Sort by distance to player
        enemies.Sort((a, b) => 
            Vector3.Distance(a.position, playerTransform.position)
            .CompareTo(Vector3.Distance(b.position, playerTransform.position)));
        
        // Attack closest enemy if any found
        if (enemies.Count > 0)
        {
            AttackEnemy(enemies[0]);
        }
    }
    
    void AttackEnemy(Transform enemy)
    {
        // Face the enemy
        transform.LookAt(enemy);
        
        // If it's a projectile weapon
        if (projectilePrefab != null)
        {
            // Spawn projectile
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
            WeaponProjectile projectileScript = projectile.GetComponent<WeaponProjectile>();
            
            if (projectileScript != null)
            {
                projectileScript.Initialize(enemy, damage);
            }
        }
        else
        {
            Enemy enemyComponent = enemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(damage);
                
                
                StartCoroutine(ShowAttackEffect(enemy.position));
            }
        }
    }
    
    IEnumerator ShowAttackEffect(Vector3 targetPosition)
    {
        // Create a line renderer for the attack
        GameObject attackLine = new GameObject("AttackLine");
        LineRenderer lineRenderer = attackLine.AddComponent<LineRenderer>();
        
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPosition);
        
        // Set the material (you can assign your own material)
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        // Set a random color for visual variety
        lineRenderer.startColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        lineRenderer.endColor = lineRenderer.startColor;
        
        // Let the effect linger briefly
        float duration = 0.1f;
        yield return new WaitForSeconds(duration);
        
        // Clean up
        Destroy(attackLine);
    }
}