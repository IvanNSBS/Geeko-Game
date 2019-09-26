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

    private bool _homing;
    private Transform _homingTarget;
    private float _homingDegreesPerSecond;

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
        var lastShotTime = Time.time - timeBetweenShots;

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
                if (_homing)
                {
                    bullet.Home(_homingTarget, _homingDegreesPerSecond);
                }
            }

            return shotsFired >= numberOfShots;
        }));
    }

    public void NwaySpread(
        TargetingManager tm,
        int numberOfStreams,
        int numberOfShotsPerStream,
        float amplitude,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var starter = -amplitude / 2.0f;
        var step = amplitude / (numberOfStreams - 1);
        for (var i = 0; i < numberOfStreams; i++)
        {
            var localTM = tm.Clone().Offset(starter + step * i);
            GenericStream(localTM, numberOfShotsPerStream, timeBetweenShots, bulletSpeed);
        }
    }

    public void SpreadThreeWay(
        Vector2 mainDirection,
        int numberOfShotsPerWay,
        float amplitude,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(mainDirection);
        NwaySpread(tm, 3, numberOfShotsPerWay, amplitude, timeBetweenShots, bulletSpeed);
    }

    public void FourDiagonals(
        Vector2 mainDirection,
        int numberOfShotsPerDiagonal,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(mainDirection);
        NwaySpread(tm, 4, numberOfShotsPerDiagonal, 270, timeBetweenShots, bulletSpeed);
    }

    public void Linear(
        Vector2 startingDirection,
        int numberOfShots,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(startingDirection);
        GenericStream(tm, numberOfShots, timeBetweenShots, bulletSpeed);
    }

    public void LinearLockOn(
        int numberOfShots,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(firePoint);
        GenericStream(tm, numberOfShots, timeBetweenShots, bulletSpeed);
    }

    public void SineWave(
        Vector2 startingDirection,
        float amplitudeDegrees,
        int numberOfShots,
        int timesToWave,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var shotsPerPeriod = numberOfShots / timesToWave;
        var tm = new TargetingManager(startingDirection).Sine(amplitudeDegrees, shotsPerPeriod);
        GenericStream(tm, numberOfShots, timeBetweenShots, bulletSpeed);
    }

    public void RandomUniform(
        Vector2 startingDirection,
        float amplitudeDegrees,
        int numberOfShots,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(startingDirection).RandomizeUniform(amplitudeDegrees);
        GenericStream(tm, numberOfShots, timeBetweenShots, bulletSpeed);
    }
    
    public void RandomGauss(
        Vector2 startingDirection,
        float amplitudeDegrees,
        int numberOfShots,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(startingDirection).RandomizeGauss(amplitudeDegrees);
        GenericStream(tm, numberOfShots, timeBetweenShots, bulletSpeed);
    }
    
    public void SpreadFiveWay(
        Vector2 mainDirection,
        int numberOfShotsPerWay,
        float amplitude,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(mainDirection);
        NwaySpread(tm, 5, numberOfShotsPerWay, amplitude, timeBetweenShots, bulletSpeed);
    }

    public void EightWayAllRange(
        Vector2 mainDirection,
        int numberOfShotsPerWay,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(mainDirection);
        NwaySpread(tm, 8, numberOfShotsPerWay, 315, timeBetweenShots, bulletSpeed);
    }

    public void SpreadSevenWay(
        Vector2 mainDirection,
        int numberOfShotsPerWay,
        float amplitude,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(mainDirection);
        NwaySpread(tm, 7, numberOfShotsPerWay, amplitude, timeBetweenShots, bulletSpeed);
    }

    public void SpreadSevenWayLockOn(
        int numberOfShotsPerWay,
        float amplitude,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(firePoint);
        NwaySpread(tm, 7, numberOfShotsPerWay, amplitude, timeBetweenShots, bulletSpeed);
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
        var tm = new TargetingManager(startingDirection).Spiral(angle);
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
    
    private void FixedUpdate()
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
        if (_homing)
        {
            bullet.Home(_homingTarget, _homingDegreesPerSecond);
        }
    }

    public void SetHoming(
        Transform target,
        float rotationDegreesPerSecond
    )
    {
        _homing = true;
        _homingTarget = target;
        _homingDegreesPerSecond = rotationDegreesPerSecond;
    }
}