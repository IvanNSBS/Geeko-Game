using System;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject owner;
    public float speed;
    public float cooldown;
    
    private Func<Vector2> _determineTarget;
    private float _lastShot = 0;

    public void SetTargetingFunction(Func<Vector2> fun)
    {
        _determineTarget = fun;
    }

    public void AttemptToShoot()
    {
        if (_lastShot + cooldown < Time.time)
        {
            _lastShot = Time.time;
            Shoot();
        }
    }

    private Vector2 DetermineVelocity()
    {
        var vec2 = _determineTarget();
        return vec2.normalized * speed;
    }

    private void Shoot()
    {
        var obj = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        var bullet = obj.GetComponent<Bullet>();
        bullet.owner = this.owner;
        bullet.rb.velocity = DetermineVelocity();
    }
}