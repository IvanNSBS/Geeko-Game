using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

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

    //homing vars
    private Transform _homingTarget;
    private bool _homingRotational = false;
    private float _homingDegreesPerSecond;
    private bool _homingDirectional = false;
    private float _homingAcceleration;
    
    //sine vars
    private bool _sine = false;
    private bool _sineFlip;
    private float _sineAmplitude;
    private float _sinePeriod;
    
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
        Func<float> bulletSpeed,
        Action<Bullet> callback = null
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
                var bs = bulletSpeed();
                bullet.rb.velocity = dir * bs;
                bullet.targetTag = targetTag;
                bullet.maxSpeed = bs;
                bullet.SetInstantiator(owner);
                if (_homingRotational)
                {
                    bullet.HomeRotational(_homingTarget, _homingDegreesPerSecond);
                } else if (_homingDirectional)
                {
                    bullet.HomeDirectional(_homingTarget, _homingAcceleration);
                }

                callback?.Invoke(bullet);
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
            GenericStream(localTM, numberOfShotsPerStream, timeBetweenShots, () => bulletSpeed);
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
        GenericStream(tm, numberOfShots, timeBetweenShots, () => bulletSpeed);
    }

    public void LinearLockOn(
        int numberOfShots,
        float timeBetweenShots,
        float bulletSpeed
    )
    {
        var tm = new TargetingManager(firePoint);
        GenericStream(tm, numberOfShots, timeBetweenShots, () => bulletSpeed);
    }

    public void SineWave(
        Vector2 principalDirection,
        float amplitude,
        int numberOfShots,
        float wavelength,
        float timeBetweenShots,
        float principalDirectionBulletSpeed,
        bool flip = false
    )
    {
        var period = wavelength / principalDirectionBulletSpeed;
        var tm = new TargetingManager(principalDirection);
        void Callback(Bullet bullet) => bullet.Sine(amplitude, period, flip);
        GenericStream(tm, numberOfShots, timeBetweenShots, () => principalDirectionBulletSpeed, Callback);
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
        GenericStream(tm, numberOfShots, timeBetweenShots, () => bulletSpeed);
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
        GenericStream(tm, numberOfShots, timeBetweenShots, () => bulletSpeed);
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
        GenericStream(tm, numberOfShotsPerLoop * loops, shootInterval, () => bulletSpeed);
    }

    public void RandomSpeedAndSpread(
        Vector2 principalDirection,
        int numberOfShots,
        float amplitudeDegrees,
        float minimumSpeed,
        float maximumSpeed
    )
    {
        var tm = new TargetingManager(principalDirection).RandomizeUniform(amplitudeDegrees);
        float RandomizeSpeed() => Random.Range(minimumSpeed, maximumSpeed);
        GenericStream(tm, numberOfShots, 0, RandomizeSpeed);
    }

    public void StopShooting() {
        shootingFuncs.Clear();
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
        bullet.maxSpeed = vel.magnitude;
        bullet.SetInstantiator(owner);
        if (_homingRotational)
        {
            bullet.HomeRotational(_homingTarget, _homingDegreesPerSecond);
        } else if (_homingDirectional)
        {
            bullet.HomeDirectional(_homingTarget, _homingAcceleration);
        } else if (_sine)
        {
            bullet.Sine(_sineAmplitude, _sinePeriod, _sineFlip);
        }
    }

    public void SetHomingRotational(
        Transform target,
        float rotationDegreesPerSecond
    )
    {
        _homingRotational = true;
        _homingTarget = target;
        _homingDegreesPerSecond = rotationDegreesPerSecond;
    }

    public void SetHomingDirectional(
        Transform target,
        float brakingTime
    )
    {
        _homingDirectional = true;
        _homingTarget = target;
        _homingAcceleration = speed / brakingTime;
    }
}