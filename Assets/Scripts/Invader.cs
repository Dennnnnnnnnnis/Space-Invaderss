using System.Collections;
using System.Collections.Generic;
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
    [HideInInspector] public int facingDir = 1;

    float offsetTimer = 0;
    private float shkTime, shkMag, shkDrop;
    public float squash = 0f, targetSquash = 0f;

    public bool isBoss = false;
    [HideInInspector] public int hp = 100;

    private void Awake()
    {
        spRend = GetComponentInChildren<SpriteRenderer>();
        spRend.sprite = animationSprites[0];
        offsetTimer += Random.Range(0f, 359f);
    }

    void Start()
    {
        //Anropar AnimateSprite med ett visst tidsintervall
        InvokeRepeating( nameof(AnimateSprite) , animationTime, animationTime);
    }

    private void Update()
    {
        // Finns för att röra invaders up och ner långsamt och lite
        offsetTimer += Time.deltaTime * 3f;
        if (offsetTimer > 360f)
            offsetTimer -= 360f;

        // Skakar invader, se GameManager
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
            spRend.transform.localPosition = Vector3.up * Mathf.Sin(offsetTimer) * 0.1f;
        }

        // Mosar invader
        transform.localScale = new Vector3((squash > 0 ? Mathf.Abs(squash) + 1 : (1f / (Mathf.Abs(squash) + 1))) * facingDir, squash < 0 ? Mathf.Abs(squash) + 1 : (1f / (Mathf.Abs(squash) + 1)), 1);
        if (squash != 0)
        {
            squash = Mathf.Max(Mathf.Abs(squash - targetSquash) - Time.deltaTime * 4f, 0) * Mathf.Sign(squash - targetSquash) + targetSquash;
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
            if(isBoss)
            {
                // Bossen har liv som försvinner istället för att bara dö
                hp--;
                Shake(0.1f, 0.1f, 1f);
                if(hp <= 0)
                    GameManager.Instance.OnInvaderKilled(this);
            }
            else
                GameManager.Instance.OnInvaderKilled(this, collision.GetComponent<Laser>().indestructable);
        }
        else if(collision.gameObject.layer == LayerMask.NameToLayer("Boundary")) //nått nedre kanten
        {
            GameManager.Instance.OnBoundaryReached();
        }
    }

    // Skakar invader, se GameManager
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
