using UnityEngine;

public class WeaponProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifetime = 5f;
    public bool homing = true;

    private Transform target;
    private float damage;

    public void Initialize(Transform enemyTarget, float weaponDamage)
    {
        target = enemyTarget;
        damage = weaponDamage;
        Destroy(gameObject, lifetime); //die :)
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        
        Vector3 direction = (target.position - transform.position).normalized;

        if (homing)
        {
            transform.forward = Vector3.Lerp(transform.forward, direction, 10f * Time.deltaTime);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            transform.position += direction * speed * Time.deltaTime;
            transform.forward = direction;
        }
        
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 0.5f)
        {
            // Apply damage
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            
            Destroy(gameObject);
        }
    }
}