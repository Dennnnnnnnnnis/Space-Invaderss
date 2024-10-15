using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public Laser laserPrefab;
    Laser laser;
    [SerializeField] float speed = 5f;
    SpriteRenderer spRend;
    [SerializeField]
    AudioSource throwingAudio;
    [SerializeField]
    AudioSource walkingAudio;
    [SerializeField] ParticleSystem shootParticles;

    private float shkTime, shkMag, shkDrop;
    public float squash = 0f, targetSquash = 0f;
    private float angle = 0;
    private bool playingAudio = false;

    [HideInInspector] public bool controllable = true;

    void Awake()
    {
        spRend = GetComponentInChildren<SpriteRenderer>();
        shootParticles.Stop();
    }

    // Update is called once per frame
    void Update()
    {
        print(playingAudio);
        if (controllable)
        {
            float move = Mathf.Clamp(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime + transform.position.x, GameManager.Instance.lEdge, GameManager.Instance.rEdge) - transform.position.x;

            transform.position += move * Vector3.right;
            angle = Mathf.Lerp(angle, Input.GetAxisRaw("Horizontal") * -8f, 8f * Time.deltaTime);
            spRend.transform.rotation = Quaternion.Euler(0, 0, angle);

            if (Input.GetKeyDown(KeyCode.Space) && laser == null)
            {
                throwingAudio.Play();
                shootParticles.Play();
                laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
                squash = 0.8f;
            }
            if (Input.GetAxisRaw("Horizontal") != 0 && playingAudio == false)
            {
                walkingAudio.Play();
                playingAudio = true;
            } 
            else if(Input.GetAxisRaw("Horizontal") == 0)
            {
                walkingAudio.Pause();
                playingAudio = false;
            }
        }


        if (shkTime > 0)
        {
            shkTime -= Time.deltaTime;

            if (shkTime > 0)
                spRend.transform.localPosition = new Vector3(Random.Range(-shkMag, shkMag), Random.Range(-shkMag, shkMag));
            else
                spRend.transform.localPosition = Vector3.zero;

            shkMag -= shkDrop * Time.deltaTime;
        }

        transform.localScale = new Vector3(squash > 0 ? Mathf.Abs(squash) + 1 : (1f / (Mathf.Abs(squash) + 1)), squash < 0 ? Mathf.Abs(squash) + 1 : (1f / (Mathf.Abs(squash) + 1)), 1);
        if (squash != 0)
        {
            squash = Mathf.Max(Mathf.Abs(squash - targetSquash) - Time.deltaTime * 4f, 0) * Mathf.Sign(squash - targetSquash) + targetSquash;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Missile") || collision.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            GameManager.Instance.OnPlayerKilled(this);
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
