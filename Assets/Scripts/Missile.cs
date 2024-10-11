using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Missile : Projectile
{
    Transform spr;
    float spd;

    private void Awake()
    {
        spr = GetComponentInChildren<SpriteRenderer>().transform;
        spd = Random.Range(4f, 26f) * (Random.Range(0, 2) * 2 - 1);
        direction = Vector3.down;
    }
   
    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;

        spr.rotation = Quaternion.Euler(0, 0, spr.rotation.eulerAngles.z + Time.deltaTime * 10f * spd);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Destroy(gameObject); //så fort den krockar med något så ska den försvinna.
    }
   
}
