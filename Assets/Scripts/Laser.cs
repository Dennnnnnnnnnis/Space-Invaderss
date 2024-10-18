using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Laser : Projectile
{
    [SerializeField] bool indestructable = false;
    private void Awake()
    {
        direction = Vector3.up;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * direction;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision);
    }

    void CheckCollision(Collider2D collision)
    {
        Bunker bunker = collision.gameObject.GetComponent<Bunker>();

        if(bunker == null) //Om det inte är en bunker vi träffat så ska skottet försvinna.
        {
            if(!indestructable || collision.gameObject.layer == 10)
            {
                if (indestructable)
                {
                    SpriteRenderer sr = Instantiate(GameManager.Instance.invaderDead, transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
                    sr.sprite = GetComponentInChildren<SpriteRenderer>().sprite;
                    sr.transform.localScale = transform.localScale;
                }
                Destroy(gameObject);
            }
        }

    }
}
