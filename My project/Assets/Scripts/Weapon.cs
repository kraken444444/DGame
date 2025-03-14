using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Weapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public string weaponName = "Default Weapon";
    public float damage = 20f;
    public float attackRate = 1f; 
    public float attackRange = 10f;
    public GameObject projectilePrefab; 
    
    [Header("Targeting")]
    public bool autoTarget = true;
    public float targetingSpeed = 5f; 
    public float targetingUpdateRate = 0.2f; 
    public string enemyTag = "Enemy"; 
    
    [Header("Visual")]
    public bool rotateWeaponWhenIdle = true;
    public float idleRotationSpeed = 90f;
    
    private float attackTimer = 0.5f;
    private float targetSearchTimer = 1.0f;
    private Transform playerTransform;
    private Transform currentTarget;
    private Quaternion originalRotation;
    
    public void Initialize(Transform player)
    {
        playerTransform = player;
        originalRotation = transform.rotation;
    }
    
    void Update()
    {
        if (autoTarget)
        {
            targetSearchTimer -= Time.deltaTime;
            if (targetSearchTimer <= 0)
            {
                FindNearestEnemy();
                targetSearchTimer = targetingUpdateRate;
            }
            
            if (currentTarget != null)
            {
                RotateTowardsTarget();
            }
            else if (rotateWeaponWhenIdle)
            {
                Vector3 randomRotation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
                transform.Rotate(randomRotation, idleRotationSpeed * Time.deltaTime);
            }
        }
        else if (rotateWeaponWhenIdle)
        {
            Vector3 randomRotation = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            transform.Rotate(randomRotation, idleRotationSpeed * Time.deltaTime);
        }
        
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0)
        {
            if (currentTarget != null)
            {
                AttackCurrentTarget();
            }
            else
            {
                FindAndAttackEnemy();
            }
            attackTimer = 1f / attackRate; 
        }
    }
    
    void FindNearestEnemy()
    {
        currentTarget = null;
        
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, attackRange);
        
        float closestDistance = float.MaxValue;
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(enemyTag))
            {
                float distance = Vector3.Distance(playerTransform.position, hitCollider.transform.position);
                
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    currentTarget = hitCollider.transform;
                }
            }
        }
    }
    
    void RotateTowardsTarget()
    {
        Vector3 targetDirection = currentTarget.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, targetingSpeed * Time.deltaTime);
    }
    
    void AttackCurrentTarget()
    {
        
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
            WeaponProjectile projectileScript = projectile.GetComponent<WeaponProjectile>();
            
            if (projectileScript != null)
            {
                projectileScript.Initialize(currentTarget, damage);
            }
        }
        else
        {
            Enemy enemyComponent = currentTarget.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.TakeDamage(damage);
                
                StartCoroutine(ShowAttackEffect(currentTarget.position));
            }
        }
    }
    
    void FindAndAttackEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(playerTransform.position, attackRange);
        
        List<Transform> enemies = new List<Transform>();
        
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag(enemyTag))
            {
                enemies.Add(hitCollider.transform);
            }
        }
        
        enemies.Sort((a, b) => 
            Vector3.Distance(a.position, playerTransform.position)
            .CompareTo(Vector3.Distance(b.position, playerTransform.position)));
        
        if (enemies.Count > 0)
        {
            currentTarget = enemies[0];
            AttackEnemy(currentTarget);
        }
    }
    
    void AttackEnemy(Transform enemy)
    {
        transform.LookAt(enemy);
        
        if (projectilePrefab != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, transform.position, transform.rotation);
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
        GameObject attackLine = new GameObject("AttackLine");
        LineRenderer lineRenderer = attackLine.AddComponent<LineRenderer>();
        
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, targetPosition);
        
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        
        lineRenderer.startColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        lineRenderer.endColor = lineRenderer.startColor;
        
        float duration = 0.1f;
        yield return new WaitForSeconds(duration);
        
        Destroy(attackLine);
    }
}