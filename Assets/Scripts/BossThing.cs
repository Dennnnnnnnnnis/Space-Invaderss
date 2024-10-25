using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossThing : MonoBehaviour
{
    bool isFalling = false;
    float fallDist = 0f;
    [SerializeField] BossMissile carrotMissile;
    Invader bossInvader;

    // Start is called before the first frame update
    void Start()
    {
        bossInvader = GameManager.Instance.invaders.bossInvader;
    }

    // Update is called once per frame
    void Update()
    {
        if (isFalling)
        {
            // Faller ner och åker fram och tillbaka
            fallDist += Time.deltaTime * 0.6f;
            transform.position = new Vector2(Mathf.Sin(Mathf.Deg2Rad * (fallDist * 80f % 360)) * 10, 16 - fallDist);

            // Om den är för lågt ner så försvinner den
            if(fallDist > 32f / 0.6f)
            {
                isFalling = false;
                fallDist = 0;
            }
        }
    }

    public void StartFalling()
    {
        if (isFalling)
            return;

        // Börja falla
        isFalling = true;
        fallDist = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Laser"))
        {
            // Aktivera missilen när den blir träffad
            BossMissile mis = Instantiate(carrotMissile, transform.position, transform.rotation);
            mis.target = bossInvader;

            isFalling = false;
            fallDist = 0;

            transform.position = Vector3.up * 16;
        }
    }
}
