using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline.Actions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(-1)]
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private Player player;
    [HideInInspector] public Invaders invaders;
    private MysteryShip mysteryShip;
    private Bunker[] bunkers;
    private Camera cam;

    [SerializeField] ParticleSystem deathParticles;
    public GameObject invaderDead;
    [SerializeField]
    AudioSource eatingSound;
    [SerializeField] TextMeshProUGUI scoreText, livesText, roundText;
    [SerializeField] GameObject pointsObj;
    [SerializeField] Canvas worldCanvas;
    [SerializeField] GameObject hitEffect;

    [HideInInspector] public float rEdge, lEdge;

    private float shkTime, shkMag, shkDrop;
    private float freezeTime = 0;
    private List<GameObject> deactivationList = new List<GameObject>();

    // Flytta och zooma med kameran
    float camSize, zoom = 1, zoomTo = 1;
    Vector2 positionTo = new Vector2();

    // Mellan rundor och s�nt
    bool killPlayer = false;
    float menuTimer = 2f;
    float roundTimer = 0f;

    // Boss battle
    public bool infiniteMode = false;
    public bool boss = false;
    float bossThingTimer = 6f;
    BossThing bossMissileThing;
    float bossFade = 0f;
    [SerializeField] SpriteRenderer overlay, overlay2;

    //Anv�nds ej just nu, men ni kan anv�nda de senare
    public int score { get; private set; } = 0;
    public int lives { get; private set; } = 3;
    int wave = 0;

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

        roundText.gameObject.SetActive(false);

        lEdge = cam.ViewportToWorldPoint(Vector3.zero).x;
        rEdge = cam.ViewportToWorldPoint(Vector3.right).x;

        bossMissileThing = FindObjectOfType<BossThing>();
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
        // Skakar objektet
        if(shkTime > 0)
        {
            shkTime -= Time.unscaledDeltaTime;

            if(shkTime > 0)
                transform.position = new Vector3(Random.Range(-shkMag, shkMag), Random.Range(-shkMag, shkMag), -10);
            else
                transform.position = new Vector3(0, 0, -10);

            shkMag -= shkDrop * Time.unscaledDeltaTime;
        }

        // Frys effekten
        if (freezeTime > 0)
        {
            Time.timeScale = 0;
            freezeTime -= Time.unscaledDeltaTime;
        }
        else
        {
            // Och n�r den �r �ver s� 'd�dar' den allt som beh�vs. Det ger en liten cool effekt.
            Time.timeScale = 1;
            if (deactivationList.Count != 0)
                KillDueInvaders();

            if(killPlayer)
            {
                if(lives <= 0)
                {
                    if (player.gameObject.active)
                    {
                        // Om det �r spelarens sista liv s� flyger dem av sk�rmen
                        SpriteRenderer sr = Instantiate(invaderDead, player.transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
                        sr.sprite = player.GetComponentInChildren<SpriteRenderer>().sprite;
                        sr.transform.localScale = player.GetComponentInChildren<SpriteRenderer>().transform.localScale;

                        player.gameObject.SetActive(false);
                    }
                    menuTimer -= Time.deltaTime;
                    if (menuTimer <= 0)
                        SceneManager.LoadScene(0);
                }
                else
                {
                    killPlayer = false;
                    player.controllable = true;
                    Zoom(1f);
                    Move(Vector2.zero);
                }
            }
        }

        // Zoom och position som r�r sig mot sitt m�l
        zoom = Mathf.Lerp(zoom, zoomTo, Time.unscaledDeltaTime * 4f);
        cam.orthographicSize = camSize / zoom;

        transform.position = Vector3.Lerp(transform.position, new Vector3(positionTo.x, positionTo.y, -10), Time.unscaledDeltaTime * 4f);
    
        // Texten i mellan rundor, mest d�r s� att spelet inte krashar av alla partiklar, men �ven ganska bra f�r en liten lugn stund.
        if(roundTimer > 0f)
        {
            roundTimer -= Time.deltaTime;
            if (roundTimer <= 0)
            {
                roundText.gameObject.SetActive(false);
                NewRound();
            }
            else
            {
                roundText.gameObject.SetActive(true);

                // �ndrar texten beroende p� omst�ndigheterna
                if (infiniteMode)
                    roundText.text = "Wave " + (wave+1);
                else
                {
                    if(wave == 5)
                        roundText.text = "You finished the game. The little baby game. Are you proud?";
                    else
                        roundText.text = "Wave " + (wave + 1) + "/5";
                }
            }
        }

        if (boss)
        {
            // Jag vet att "Boss thing" �r ett v�ldigt d�ligt namn, men det �r den stora moroten som man ska skjuta f�r att aktivera
            bossThingTimer -= Time.deltaTime;
            if(bossThingTimer <= 0f)
            {
                bossThingTimer = Random.Range(10f, 15f);
                bossMissileThing.StartFalling();
            }

            // Detta �r f�r att l�ngsamt ta in regnet
            if(bossFade < 1f)
            {
                bossFade = Mathf.Min(bossFade + Time.deltaTime * 0.2f, 1f);
                overlay.color = new Color(28f / 255f, 19f / 255f, 58f / 255f, bossFade * 0.5f);
                overlay2.color = new Color(1f, 1f, 1f, bossFade * 0.3f);
            }
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
        wave++;
        boss = (wave == 5 && !infiniteMode);
        if (wave > 5 && !infiniteMode)
        {
            SceneManager.LoadScene(0);
            return;
        }

        // Spawnar invaders eller boss beroende p� om det �r en boss runda eller inte
        if (!boss)
        {
            invaders.ResetInvaders();
            invaders.gameObject.SetActive(true);
        }
        else
        {
            invaders.SpawnBoss();
        }

        for (int i = 0; i < bunkers.Length; i++)
        {
            bunkers[i].ResetBunker();
        }

        mysteryShip.gameObject.SetActive(true);
        player.realism += 0.05f; // G�r spelet mer realistiskt ju l�ngre man kommer

        //Respawn(); // Tog bort detta f�r att det var bara lite konstigt? Det �r b�ttre att vara d�r man vet n�r rundan b�rjar ist�llet f�r att f�rsvinna till mitten igen.
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

    private void SetScore(int points)
    {
        score = points;
        scoreText.text = "Score: " + score;
    }

    private void ChangeScore(int points, Vector3 pos)
    {
        // Det h�r visar hur mycket po�ng man f�r med en liten text animation
        GameObject newPoints = Instantiate(pointsObj, pos, Quaternion.Euler(0, 0, Random.Range(-10, 10)), worldCanvas.transform);
        newPoints.GetComponentInChildren<TextMeshProUGUI>().text = "+" + points;
        Destroy(newPoints, 0.667f);
        SetScore(score + points);
    }

    private void SetLives(int _lives)
    {
        lives = _lives;
        livesText.text = "Lives: " + lives;
    }

    public void OnPlayerKilled(Player player)
    {
        SetLives(lives - 1);
        if(lives <= 0)
        {
            // Man m�ste vara lite extra n�r man ska d�, s� allt �r mer drastiskt
            Freeze(0.5f);
            player.Shake(0.5f, 0.2f, 0f);
            Zoom(2f);
            Move(player.transform.position);
            player.PlayVoiceLine(5);
        }
        else
        {
            // Och detta �r mindre drastiskt
            Freeze(0.3f);
            player.Shake(0.3f, 0.2f, 0f);
            Zoom(1.5f);
            Move(player.transform.position * 0.5f);
            player.PlayVoiceLine();
        }
        player.controllable = false; // Ta bort kontroll fr�n spelaren s� att den inte kan skjuta och s�nt
        killPlayer = true;
    }

    public void OnInvaderKilled(Invader invader, bool deathByBeingConsumed = false)
    {
        deactivationList.Add(invader.gameObject);

        if (deathByBeingConsumed)
            eatingSound.pitch = Random.Range(0.5f, 0.9f);
        else
            eatingSound.pitch = Random.Range(0.8f, 1.2f);
        eatingSound.Play();

        // Coolhet
        Freeze(0.05f);
        invader.Shake(0.05f, 0.5f, 0f);
    }

    void KillDueInvaders()
    {
        // Detta finns f�r att beh�lla de 'd�ende' invaders under freeze framen
        for(var i = 0; i < deactivationList.Count; i++)
        {
            // Partiklar
            GameObject part = Instantiate(deathParticles, deactivationList[0].transform.position, Quaternion.identity).gameObject;
            Destroy(part, 4f);

            // Flyger av sk�rmen
            SpriteRenderer sr = Instantiate(invaderDead, deactivationList[0].transform.position, Quaternion.identity).GetComponent<SpriteRenderer>();
            sr.sprite = deactivationList[0].GetComponentInChildren<SpriteRenderer>().sprite;
            sr.transform.localScale = deactivationList[0].GetComponentInChildren<SpriteRenderer>().transform.localScale;

            // Ger po�ng
            if (deactivationList[0].TryGetComponent<MysteryShip>(out MysteryShip ms))
                ChangeScore(200, deactivationList[0].transform.position);
            else
                ChangeScore(10, deactivationList[0].transform.position);

            // Och f�rsvinner
            deactivationList[0].gameObject.SetActive(false);
            deactivationList.RemoveAt(0);
        }

        if (invaders.GetInvaderCount() == 0 && roundTimer <= 0f)
        {
            // Om bossen �r besegrad s� f�r man mer tid f�r sina sista stunder
            if (wave == 5 && !infiniteMode)
                roundTimer = 5f;
            else
                roundTimer = 2.4f;
        }
    }

    public void OnMysteryShipKilled(MysteryShip mysteryShip)
    {
        deactivationList.Add(mysteryShip.gameObject);
        eatingSound.Play();
        Freeze(0.05f);
        mysteryShip.Shake(0.05f, 0.5f, 0f);
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
        // Det h�r f�r kameran att skaka
        if(shkTime <= 0 || shakeMagnitude > shkMag)
        {
            shkTime = shakeTime; // Hur l�nge det skakas
            shkMag = shakeMagnitude; // Hur mycket det skakas
            shkDrop = shakeDropoff; // Och hur mycket skak som f�rsvinner per sekund
        }
        // Detta �r dessamma f�r alla Shake funktioner, s� jag kunde s�kert gjort detta p� ett b�ttre s�tt. Aja
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

    public void CreateHitEffect(Vector2 pos)
    {
        // Detta spawnar (och f�rst�r) den knappast m�rkbara hit effekten som kommer n�r skott tr�ffar saker
        Destroy(Instantiate(hitEffect, pos, Quaternion.identity), 0.17f);
    }
}
