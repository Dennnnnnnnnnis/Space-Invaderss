using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Player player;
    private Invaders invaders;
    private MysteryShip mysteryShip;
    private Bunker[] bunkers;

    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] GameObject invaderDead;
    [SerializeField]
    AudioSource eatingSound;

    private float shkTime, shkMag, shkDrop;
    private float freezeTime = 0;
    private List<Invader> deactivationList = new List<Invader>();

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
        }
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

        player.gameObject.SetActive(false);

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
}
