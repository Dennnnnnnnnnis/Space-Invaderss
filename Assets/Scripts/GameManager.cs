using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Player player;
    private Invaders invaders;
    private MysteryShip mysteryShip;
    private Bunker[] bunkers;
    private Camera cam;

    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] GameObject invaderDead;
    [SerializeField]
    AudioSource eatingSound;

    [HideInInspector] public float rEdge, lEdge;

    private float shkTime, shkMag, shkDrop;
    private float freezeTime = 0;
    private List<Invader> deactivationList = new List<Invader>();

    float camSize, zoom = 1, zoomTo = 1;
    Vector2 positionTo = new Vector2();

    bool killPlayer = false;
    float menuTimer = 4f;

    //Används ej just nu, men ni kan använda de senare
    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        cam = GetComponent<Camera>();
        camSize = cam.orthographicSize;

        lEdge = cam.ViewportToWorldPoint(Vector3.zero).x;
        rEdge = cam.ViewportToWorldPoint(Vector3.right).x;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    private void Start()
    {
        player = FindObjectOfType<Player>();
        invaders = FindObjectOfType<Invaders>();
        mysteryShip = FindObjectOfType<MysteryShip>();
        bunkers = FindObjectsOfType<Bunker>();

        NewGame();
    }

    private void Update()
    {
        if (lives <= 0 && Input.GetKeyDown(KeyCode.Return))
        {
            NewGame();
        }

        if(shkTime > 0)
        {
            shkTime -= Time.unscaledDeltaTime;

            if(shkTime > 0)
                transform.position = new Vector3(Random.Range(-shkMag, shkMag), Random.Range(-shkMag, shkMag), -10);
            else
                transform.position = new Vector3(0, 0, -10);

            shkMag -= shkDrop * Time.unscaledDeltaTime;
        }

        if (freezeTime > 0)
        {
            Time.timeScale = 0;
            freezeTime -= Time.unscaledDeltaTime;
        }
        else
        {
            Time.timeScale = 1;
            if (deactivationList.Count != 0)
                KillDueInvaders();

            if(killPlayer)
            {
                if(player.gameObject.active)
                {
                    SpriteRenderer sr = Instantiate(invaderDead, player.transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
                    sr.sprite = player.GetComponentInChildren<SpriteRenderer>().sprite;
                    sr.transform.localScale = player.GetComponentInChildren<SpriteRenderer>().transform.localScale;

                    player.gameObject.SetActive(false);
                }
                menuTimer -= Time.deltaTime;
                if (menuTimer <= 0)
                    SceneManager.LoadScene(0);
            }
        }

        zoom = Mathf.Lerp(zoom, zoomTo, Time.unscaledDeltaTime * 4f);
        cam.orthographicSize = camSize / zoom;

        transform.position = Vector3.Lerp(transform.position, new Vector3(positionTo.x, positionTo.y, -10), Time.unscaledDeltaTime * 4f);
    }

    private void NewGame()
    {

        SetScore(0);
        SetLives(3);
        NewRound();
    }

    private void NewRound()
    {
        invaders.ResetInvaders();
        invaders.gameObject.SetActive(true);

        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].ResetBunker();
        }

        Respawn();
    }

    private void Respawn()
    {
        Vector3 position = player.transform.position;
        position.x = 0f;
        player.transform.position = position;
        player.gameObject.SetActive(true);
    }

    private void GameOver()
    {
        invaders.gameObject.SetActive(false);
    }

    private void SetScore(int score)
    {
        
    }

    private void SetLives(int lives)
    {
       
    }

    public void OnPlayerKilled(Player player)
    {
        Freeze(2f);
        player.Shake(2f, 0.3f, 0f);
        Zoom(2f);
        Move(player.transform.position);
        player.controllable = false;
        killPlayer = true;
    }

    public void OnInvaderKilled(Invader invader)
    {
        deactivationList.Add(invader);
        eatingSound.Play();
        Freeze(0.05f);
        invader.Shake(0.05f, 0.5f, 0f);
    }

    void KillDueInvaders()
    {
        for(var i = 0; i < deactivationList.Count; i++)
        {
            Instantiate(deathParticles, deactivationList[0].transform.position, Quaternion.identity);

            SpriteRenderer sr = Instantiate(invaderDead, deactivationList[0].transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
            sr.sprite = deactivationList[0].GetComponentInChildren<SpriteRenderer>().sprite;
            sr.transform.localScale = deactivationList[0].GetComponentInChildren<SpriteRenderer>().transform.localScale;

            deactivationList[0].gameObject.SetActive(false);
            deactivationList.RemoveAt(0);
        }

        if (invaders.GetInvaderCount() == 0)
        {
            NewRound();
        }
    }

    public void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        mysteryShip.gameObject.SetActive(false);
    }

    public void OnBoundaryReached()
    {
        if (invaders.gameObject.activeSelf)
        {
            invaders.gameObject.SetActive(false);
            OnPlayerKilled(player);
        }
    }

    public void Shake(float shakeTime, float shakeMagnitude, float shakeDropoff)
    {
        if(shkTime <= 0 || shakeMagnitude > shkMag)
        {
            shkTime = shakeTime;
            shkMag = shakeMagnitude;
            shkDrop = shakeDropoff;
        }
    }

    public void Freeze(float time)
    {
        if (time > freezeTime)
            freezeTime = time;
    }

    public void Zoom(float newZoom, float curZoom = 0)
    {
        if (curZoom != 0)
            zoom = curZoom;
        zoomTo = newZoom;
    }

    public void Move(Vector2 newPos)
    {
        positionTo = newPos;
    }
}
