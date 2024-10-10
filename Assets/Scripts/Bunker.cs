using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class Bunker : MonoBehaviour
{
    int nrOfHits = 0;
    SpriteRenderer spRend;
    Color originalColor;

    private float shkTime, shkMag, shkDrop;

    private void Awake()
    {
        spRend = GetComponentInChildren<SpriteRenderer>();
        originalColor = spRend.color;
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
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.gameObject.layer == LayerMask.NameToLayer("Missile") || other.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            Shake(0.2f, 0.1f, 1f);
            //Ändrar färgen beroende på antal träffar.
            nrOfHits++;
            Color oldColor = spRend.color;

            Color newColor = new Color(oldColor.r +(nrOfHits*0.1f), oldColor.g + (nrOfHits * 0.1f), oldColor.b + (nrOfHits * 0.1f));
            
            spRend.color = newColor;
            
            if (nrOfHits == 6)
            {
                gameObject.SetActive(false);
            }
            
        }
    }

    public void ResetBunker()
    {
        gameObject.SetActive(true);
        spRend.color = originalColor;
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
