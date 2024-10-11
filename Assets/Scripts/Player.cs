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

    private float shkTime, shkMag, shkDrop;
    public float squash = 0f, targetSquash = 0f;
    private float angle = 0;

    void Awake()
    {
        spRend = GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 leftEdge = Camera.main.ViewportToWorldPoint(Vector3.zero);
        Vector3 rightEdge = Camera.main.ViewportToWorldPoint(Vector3.right);

        float move = Mathf.Clamp(Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime + transform.position.x, leftEdge.x, rightEdge.x) - transform.position.x;

        transform.position += move * Vector3.right;
        angle = Mathf.Lerp(angle, Input.GetAxisRaw("Horizontal") * -8f, 8f * Time.deltaTime);
        spRend.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (Input.GetKeyDown(KeyCode.Space) && laser == null)
        {
            throwingAudio.Play();
            laser = Instantiate(laserPrefab, transform.position, Quaternion.identity);
            squash = 0.8f;
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
