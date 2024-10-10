using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
//[RequireComponent(typeof(SpriteRenderer))]

public class Invader : MonoBehaviour
{
    public Sprite[] animationSprites = new Sprite[2];
    public float animationTime;

    SpriteRenderer spRend;
    int animationFrame;

    float offsetTimer = 0;
    private float shkTime, shkMag, shkDrop;

    private void Awake()
    {
        spRend = GetComponentInChildren<SpriteRenderer>();
        spRend.sprite = animationSprites[0];
    }

    void Start()
    {
        //Anropar AnimateSprite med ett visst tidsintervall
        InvokeRepeating( nameof(AnimateSprite) , animationTime, animationTime);
    }

    private void Update()
    {
        if (shkTime > 0)
        {
            shkTime -= Time.deltaTime;

            if (shkTime > 0)
                spRend.transform.localPosition = new Vector3(Random.Range(-shkMag, shkMag), Random.Range(-shkMag, shkMag));
            else
                spRend.transform.localPosition = Vector3.zero;

            shkMag -= shkDrop * Time.deltaTime;
        }
        else
        {
            offsetTimer += Time.deltaTime;
            if (offsetTimer > 360f)
                offsetTimer -= 360f;

            spRend.transform.localPosition = Vector3.up * Mathf.Sin(offsetTimer) * 0.1f;
        }
    }

    //pandlar mellan olika sprited för att skapa en animation
    private void AnimateSprite()
    {
        animationFrame++;
        if(animationFrame >= animationSprites.Length)
        {
            animationFrame = 0;
        }
        spRend.sprite = animationSprites[animationFrame];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            GameManager.Instance.OnInvaderKilled(this);
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Boundary")) //nått nedre kanten
        {
            GameManager.Instance.OnBoundaryReached();
        }
    }

    public void Shake(float shakeTime, float shakeMagnitude, float shakeDropoff)
    {
        if (shkTime <= 0 || shakeMagnitude > shkMag)
        {
            shkTime = shakeTime;
            shkMag = shakeMagnitude;
            shkDrop = shakeDropoff;
        }
    }
}
