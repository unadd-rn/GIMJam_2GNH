using UnityEngine;

namespace RobotController
{
    public class RobotAttack : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RobotController _controller;
        [SerializeField] private Transform _firePoint;
        [SerializeField] private GameObject _bulletPrefab;

        [Header("Settings")]
        [SerializeField] private float _fireRate = 0.5f;
        private float _nextFireTime;

        void Update()
        {
            if (_controller.ExternalAttackDown && Time.time >= _nextFireTime)
            {
                GetComponent<Animator>().SetTrigger("Attack"); 
                
                _nextFireTime = Time.time + _fireRate;
                _controller.ExternalAttackDown = false;
            }
        }

        public void Shoot()
        {
            if (_bulletPrefab == null || _firePoint == null) return;

            GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
            
            var bulletScript = bullet.GetComponent<PlayerBulletScript>();
            if (bulletScript != null)
            {
                bulletScript.ToRight = _controller.FacingDirection > 0;
            }
        }
    }
}