using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMissile : MonoBehaviour
{
    [HideInInspector] public Invader target;
    float spin = 1f;
    float prec = 1f;
    bool destroy = false;

    // Update is called once per frame
    void Update()
    {
        if(spin <= 0f)
        {
            // Åker mot bossen
            prec += Time.deltaTime;
            transform.up = Vector3.Lerp(transform.up, target.transform.position - transform.position, prec * Time.deltaTime).normalized;
            transform.position += transform.up * 12f * Time.deltaTime;
        }
        else
        {
            // Gör en fin liten snurr
            transform.up = target.transform.position - transform.position;
            transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + (360f * spin * spin));
            spin = Mathf.Max(spin - Time.deltaTime * 2f, 0f);
        }

        // För freeze frame
        if (destroy && Time.timeScale != 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!destroy && collision.transform == target.transform)
        {
            // Skada bossen när den kolliderar med den
            target.hp -= 20;
            target.Shake(0.4f, 0.5f, 1f);
            if (target.hp <= 0)
                GameManager.Instance.OnInvaderKilled(target);

            destroy = true;
            GameManager.Instance.Freeze(0.1f);
            GameManager.Instance.Shake(0.1f, 0.1f, 0f);
        }
    }
}
