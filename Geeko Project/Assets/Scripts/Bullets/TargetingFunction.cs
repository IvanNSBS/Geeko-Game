using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

public class TargetingManager
{
    private Vector2 initial = Vector2.up;
    [NotNull] private List<Func<Vector2, Vector2>> targetingList = new List<Func<Vector2, Vector2>>();
    
    public Vector2 Target()
    {
        var target = initial;
        targetingList.ForEach((func => { target = func(target); }));
        return target;
    }

    public TargetingManager InitStartingDirection(Vector2 vec)
    {
        initial = vec;
        
        return this;
    }

    public TargetingManager InitDynamicStartingDirection(Func<Vector2> func)
    {
        targetingList.Add((_ => func()));

        return this;
    }

    public TargetingManager InitFollowPlayer(Transform firePoint)
    {
        targetingList.Add((_ =>
        {
            var player = GameObject.FindGameObjectWithTag("Player").transform;
            var center = player.TransformPoint(player.GetComponent<CircleCollider2D>().offset);
            var direction = (center - firePoint.position).normalized;
            return new Vector2(direction.x, direction.y);
        }));

        return this;
    }

    public TargetingManager Spiral(float degrees)
    {
        var rotation = Quaternion.identity;
        var rotationIncrement = Quaternion.Euler(0, 0, degrees);
        
        targetingList.Add((vector2 => {
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
        
        targetingList.Add((vector2 =>
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
        
        targetingList.Add((vector2 => rotation*vector2));

        return this;
    }

    private static Func<float> rand = () => (Random.value - 0.5f);

    public TargetingManager Randomize(float amplitudeDegrees)
    {
        targetingList.Add((vector2 =>
        {
            var rot = Quaternion.Euler(0, 0, rand() * amplitudeDegrees);
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
        targetingList.Add((vector2 =>
        {
            var rot = Quaternion.Euler(0, 0, RandGauss() * amplitudeDegrees);
            return rot * vector2;
        } ));

        return this;
    }
}