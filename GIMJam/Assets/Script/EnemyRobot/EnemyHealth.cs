using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 3;
    [SerializeField] private GameObject _deathEffectPrefab; //death particle?
    public void TakeDamage(float damage)
    {
        health -= damage;

        // Trigger the flash effect
        // HitFlash flasher = GetComponent<HitFlash>();
        // if (flasher != null) flasher.Flash();

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        if (_deathEffectPrefab != null)
        {
            Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
        }
        
        Destroy(gameObject);
    }
}