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
        private Animator _anim;

        void Awake() 
        {
            _anim = GetComponent<Animator>();
        }

        void Update()
        {
            if (_controller == null) return;

            if (_controller.ExternalAttackDown)
            {
                if (Time.time >= _nextFireTime)
                {
                    TriggerAttack();
                }
                
                _controller.ExternalAttackDown = false;
            }
        }

        private void TriggerAttack()
        {
            _nextFireTime = Time.time + _fireRate;
            if (_anim != null) 
            {
                _anim.SetTrigger("Attack");
            }
            else 
            {
                Shoot();
            }
        }

        public void Shoot()
        {
            if (_bulletPrefab == null || _firePoint == null) return;

            GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
            
            if (bullet.TryGetComponent(out PlayerBulletScript bulletScript))
            {
                bulletScript.ToRight = _controller.FacingDirection > 0;
            }
        }
    }
}