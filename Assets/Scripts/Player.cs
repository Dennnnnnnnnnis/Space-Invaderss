using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Player : MonoBehaviour
{
    public Laser laserPrefab, bigLaserPrefab;
    Laser laser;
    [SerializeField] float speed = 5f;
    SpriteRenderer spRend;
    [SerializeField]
    AudioSource throwingAudio;
    [SerializeField]
    AudioSource walkingAudio;
    [SerializeField] ParticleSystem shootParticles;
    float immunityTimer = 0f;
    [SerializeField] GameObject immuneEffect;

    public int bigLasers = 0;

    private float shkTime, shkMag, shkDrop;
    public float squash = 0f, targetSquash = 0f;
    private float angle = 0;
    private bool playingAudio = false;
    bool immuneBool = true;

    [HideInInspector] public bool controllable = true;

    [SerializeField] AudioClip[] lines;
    int lastLine = -1;
    AudioSource linePlayer;
    [Range(0.0f, 1.0f)] public float realism = 0.2f;

    void Awake()
    {
        spRend = GetComponentInChildren<SpriteRenderer>();
        shootParticles.Stop();
        immuneEffect.SetActive(false);
        linePlayer = spRend.GetComponent<AudioSource>();
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
                if(bigLasers <= 0)
                {
                    GameManager.Instance.Shake(0.2f, 0.1f, 0.1f);
                    laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
                    squash = 0.8f;
                }
                else
                {
                    GameManager.Instance.Shake(0.2f, 0.2f, 0.2f);
                    laser = Instantiate(bigLaserPrefab, transform.position, Quaternion.identity);
                    squash = 0.4f;
                    bigLasers--;
                }
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

        if(immunityTimer > 0)
        {
            immuneEffect.SetActive(true);
            immunityTimer -= Time.deltaTime;
        }
        else
        {
            immuneEffect.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Missile") || collision.gameObject.layer == LayerMask.NameToLayer("Invader"))
        {
            if(immunityTimer <= 0)
            {
                GameManager.Instance.OnPlayerKilled(this);
            }
        }
        if (collision.tag == "borste")
        {
            Destroy(collision.gameObject);
            immunityTimer = 6f;
        }
        else if (collision.tag == "tänder")
        {
            Destroy(collision.gameObject);
            bigLasers++;
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

    public void PlayVoiceLine(int ln = -1)
    {
        if (Random.Range(0f, 1f) > realism)
            return;

        int clip = ln;
        if(clip == -1)
        {
            int count = lines.Length;
            if (lastLine != -1)
                count--;

            clip = Random.Range(0, count);
            if (clip >= lastLine)
                clip++;
        }

        linePlayer.clip = lines[clip];
        linePlayer.Play();
        lastLine = clip;
    }
}
