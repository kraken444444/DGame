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
        Destroy(gameObject, lifetime); // Self-destruct after lifetime
    }

    void Update()
    {
        if (target == null)
        {
            // Target was destroyed, destroy projectile
            Destroy(gameObject);
            return;
        }

        // Move towards target
        Vector3 direction = (target.position - transform.position).normalized;

        if (homing)
        {
            transform.forward = Vector3.Lerp(transform.forward, direction, 10f * Time.deltaTime);
            transform.position += transform.forward * speed * Time.deltaTime;
        }
        else
        {
            // Direct movement
            transform.position += direction * speed * Time.deltaTime;
            transform.forward = direction;
        }

        // Check if we hit the target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget < 0.5f)
        {
            // Apply damage
            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }

            // Destroy projectile
            Destroy(gameObject);
        }
    }
}