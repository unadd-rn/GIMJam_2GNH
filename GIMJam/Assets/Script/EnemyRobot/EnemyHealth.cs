using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float health = 3;
    public GameObject healthbar;
    public GameObject bar1, bar2, bar3;
    [SerializeField] private GameObject _deathEffectPrefab; //death particle?
    
    public void TakeDamage(float damage)
    {
        health -= damage;

        // Trigger the flash effect
        HitFlash flasher = GetComponent<HitFlash>();
        if (flasher != null) flasher.Flash();
        UpdateUI();
        if (health <= 0)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if(bar3 != null) bar3.SetActive(health >= 3);
        if(bar2 != null) bar2.SetActive(health >= 2);
        if(bar1 != null) bar1.SetActive(health >= 1);
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