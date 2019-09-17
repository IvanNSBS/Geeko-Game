using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetingManager
{
    private Vector2 _initial = Vector2.up;
    [NotNull] private readonly List<Func<Vector2, Vector2>> _targetingList = new List<Func<Vector2, Vector2>>();
    
    public Vector2 Target()
    {
        var target = _initial;
        _targetingList.ForEach((func => { target = func(target); }));
        return target;
    }

    public TargetingManager(Vector2 vec)
    {
        _initial = vec;
    }

    public TargetingManager(Transform firePoint)
    {
        _targetingList.Add((_ =>
        {
            var player = GameObject.FindGameObjectWithTag("Player").transform;
            var center = player.TransformPoint(player.GetComponent<CircleCollider2D>().offset);
            var direction = (center - firePoint.position).normalized;
            return new Vector2(direction.x, direction.y);
        }));
    }

    public TargetingManager(Func<Vector2> func)
    {
        _targetingList.Add((_ => func()));
    }

    public TargetingManager Spiral(float degrees)
    {
        var rotation = Quaternion.identity;
        var rotationIncrement = Quaternion.Euler(0, 0, degrees);
        
        _targetingList.Add((vector2 => {
            var rotated = rotation*vector2;
            rotation *= rotationIncrement;
            return rotated;
        }));
        
        return this;
    }

    public TargetingManager Sine(float amplitudeDegrees, int shotsPerPeriod)
    {
        var accu = 0.0;
        var increment = Math.PI * 2 / shotsPerPeriod;
        var halfAmp = amplitudeDegrees / 2;
        
        _targetingList.Add((vector2 =>
        {
            var sin = (float) Math.Sin(accu);
            accu = (accu + increment);
            var sinQuaternion = Quaternion.Euler(0, 0, sin * halfAmp);
            return sinQuaternion * vector2;
        }));

        return this;
    }

    public TargetingManager Offset(float offset)
    {
        var rotation = Quaternion.Euler(0, 0, offset);
        
        _targetingList.Add((vector2 => rotation*vector2));

        return this;
    }

    private static readonly Func<float> Rand = () => (Random.value - 0.5f);

    public TargetingManager RandomizeUniform(float amplitudeDegrees)
    {
        _targetingList.Add((vector2 =>
        {
            var rot = Quaternion.Euler(0, 0, Rand() * amplitudeDegrees);
            return rot * vector2;
        } ));

        return this;
    }
    
    private static float _prev = Random.value;
    private static float RandGauss()
    {
        var u1 = _prev;
        var u2 = Random.value;
        var randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        _prev = u2;
        return (float) (randStdNormal - 0.5) / 3;
    }

    public TargetingManager RandomizeGauss(float amplitudeDegrees)
    {
        _targetingList.Add((vector2 =>
        {
            var rot = Quaternion.Euler(0, 0, RandGauss() * amplitudeDegrees);
            return rot * vector2;
        } ));

        return this;
    }

    public TargetingManager Clone()
    {
        var tm = new TargetingManager(_initial);
        tm._targetingList.InsertRange(0, this._targetingList);
        return tm;
    }
}