using System;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject owner;
    public float speed;
    public float cooldown;
    public string targetTag;
    
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

    private bool doSpiral = false;
    private float spiralLastShotTime;
    private Vector2 spiralStartingDirection;
    private Quaternion spiralRotation;
    private Quaternion spiralRotationIncrement;
    private int spiralShotsFired;
    private int spiralNumberOfShots;
    private int spiralLoops;
    private float spiralShootInterval;

    public void Spiral(
        Vector2 startingDirection,
        int numberOfShotsPerLoop,
        float timeToSpiralOnce,
        int loops
        )
    {
        doSpiral = true;
        spiralLastShotTime = Time.time;
        spiralStartingDirection = startingDirection;
        spiralRotation = Quaternion.identity;
        var angle = 360 / ((float) numberOfShotsPerLoop / loops);
        spiralRotationIncrement = Quaternion.Euler(0, 0, angle);
        spiralShotsFired = 0;
        spiralNumberOfShots = numberOfShotsPerLoop * loops;
        spiralShootInterval = timeToSpiralOnce / numberOfShotsPerLoop;
    }

    private void SpiralShoot()
    {
        if (doSpiral)
        {
            if (spiralShotsFired > spiralNumberOfShots)
            {
                doSpiral = false;
                return;
            }

            while (Time.time > spiralLastShotTime + spiralShootInterval && spiralShotsFired <= spiralNumberOfShots)
            {
                ++spiralShotsFired;
                spiralLastShotTime += spiralShootInterval;
                var direction = (spiralRotation * spiralStartingDirection).normalized;
                var obj = Instantiate(bulletPrefab, firePoint.position, GameplayStatics.GetRotationFromDir(direction));
                spiralRotation *= spiralRotationIncrement;
                var bullet = obj.GetComponent<Bullet>();
                bullet.rb.velocity = direction * speed;
                bullet.targetTag = this.targetTag;
                bullet.SetInstantiator(gameObject);
            }
        }
    }

    private void Update()
    {
        SpiralShoot();
    }

    private void Shoot()
    {
        var vel = DetermineVelocity();
        var obj = Instantiate(bulletPrefab, firePoint.position, GameplayStatics.GetRotationFromDir(vel));
        var bullet = obj.GetComponent<Bullet>();
        bullet.targetTag = this.targetTag;
        bullet.rb.velocity = vel;
    }
}