using System;
using System.Collections.Generic;
using JetBrains.Annotations;
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
    [NotNull] private List<Func<bool>> shootingFuncs = new List<Func<bool>>();
    [NotNull] private List<int> funcsToRemove = new List<int>();

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

    public void GenericStream(
        TargetingManager tm,
        int numberOfShots,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var shotsFired = 0;
        var lastShotTime = Time.time;

        shootingFuncs.Add((() =>
        {
            while (Time.time > lastShotTime + timeBetweenShots && shotsFired < numberOfShots)
            {
                lastShotTime += timeBetweenShots;
                ++shotsFired;
                var dir = tm.Target().normalized;
                var obj = Instantiate(bulletPrefab, firePoint.position, GameplayStatics.GetRotationFromDir(dir));
                var bullet = obj.GetComponent<Bullet>();
                bullet.rb.velocity = dir * bulletSpeed;
                bullet.targetTag = targetTag;
                bullet.SetInstantiator(owner);
            }

            return shotsFired >= numberOfShots;
        }));
    }

    public void StreamFollowingPlayer(
        int numberOfShots,
        float timeBetweenShots,
        float bulletSpeed,
        float offsetDegrees = 0.0f
    )
    {
        var tm = new TargetingManager().InitFollowPlayer(firePoint).Offset(offsetDegrees);
        GenericStream(tm, numberOfShots, timeBetweenShots, bulletSpeed);
    }

    public void Stream(
        Vector2 startingDirection,
        int numberOfShots,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager().InitStartingDirection(startingDirection);
        GenericStream(tm, numberOfShots, timeBetweenShots, bulletSpeed);
    }

    public void Spiral(
        Vector2 startingDirection,
        int numberOfShotsPerLoop,
        float timeToSpiralOnce,
        int loops,
        float bulletSpeed
        )
    {
        var angle = 360.0f / ((float) numberOfShotsPerLoop / loops);
        var shootInterval = timeToSpiralOnce / numberOfShotsPerLoop;
        var tm = new TargetingManager().InitStartingDirection(startingDirection).Spiral(angle);
        GenericStream(tm, numberOfShotsPerLoop * loops, shootInterval, bulletSpeed);
    }
    
    private void RunShootingFuncs() {
        var i = 0;
        shootingFuncs.ForEach(func =>
        {
            var shouldRemove = func();
            if (shouldRemove)
            {
                funcsToRemove.Add(i);
                --i;
            }
            ++i;
        });
        funcsToRemove.ForEach(index =>
        {
            shootingFuncs.RemoveAt(index);
        });
        funcsToRemove.Clear();
    }
    

    private void Update()
    {
        RunShootingFuncs();
    }

    public void Shoot()
    {
        var vel = DetermineVelocity();
        var obj = Instantiate(bulletPrefab, firePoint.position, GameplayStatics.GetRotationFromDir(vel));
        var bullet = obj.GetComponent<Bullet>();
        bullet.targetTag = this.targetTag;
        bullet.rb.velocity = vel;
        bullet.SetInstantiator(owner);
    }
}