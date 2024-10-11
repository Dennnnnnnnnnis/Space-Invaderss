using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corpse : MonoBehaviour
{
    Rigidbody2D rb;
    float angle = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(Random.Range(3f, 8f) * (Random.Range(0, 2) * 2 - 1), 16f);
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        angle -= Mathf.Clamp(Mathf.Abs(rb.velocity.y) * 40f * Time.deltaTime, 0.5f, 10f) * Mathf.Sign(rb.velocity.x);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
