using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float maxHealth = 100f;
    public float currentHealth;
    
    void Start()
    {
        currentHealth = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashColor());
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    IEnumerator FlashColor()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            Color originalColor = renderer.material.color;
            renderer.material.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            renderer.material.color = originalColor;
        }
    }
    
    void Die()
    {
        // Handle enemy death
        Destroy(gameObject);
    }
}
