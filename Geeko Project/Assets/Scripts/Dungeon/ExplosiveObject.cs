using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WeaponComponent))]
public class ExplosiveObject : DestructibleObject
{
    [Header("Explosion")]
    public int numberOfBullets;
    public float speed;
    public GameObject explosionPrefab;

    public override void DoItBeforeDestroy()
    {
        Instantiate(explosionPrefab, this.transform);
        CirclePattern(transform.position, Vector3.zero, numberOfBullets, 0, 1);
    }

    private void CirclePattern(Vector3 origin, Vector3 target, int numbersOfShootsPerLoop, float timeToSpiralOnce, int loops)
    {
        var weaponComponent = this.gameObject.GetComponent<WeaponComponent>();
        weaponComponent.firePoint.position = origin;
        var vec3 = target - origin;
        var vec2 = new Vector2(vec3.x, vec3.y);
        weaponComponent.Spiral(vec2, numbersOfShootsPerLoop, timeToSpiralOnce, loops, speed);
        Debug.Log("ixprudiu");
    }
}
