using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Invaders : MonoBehaviour
{
    [SerializeField]
    AudioSource shootingSound;
    public Invader[] prefab = new Invader[5];

    private int row = 5;
    private int col = 11;

    private Vector3 initialPosition;
    private Vector3 direction = Vector3.right;

    public Missile missilePrefab;
    [HideInInspector] public Invader bossInvader;

    private void Awake()
    {
        bossInvader = GetComponentInChildren<Invader>();
        initialPosition = transform.position;
        CreateInvaderGrid();
        bossInvader.gameObject.SetActive(false);
    }

    private void Start()
    {
        InvokeRepeating(nameof(MissileAttack), 1, 1); //Hur ofta ska den skjuta iväg missiler
    }

    //Skapar själva griden med alla invaders.
    void CreateInvaderGrid()
    {
        for(int r = 0; r < row; r++)
        {
            float width = 2f * (col - 1);
            float height = 2f * (row - 1);

            //för att centerar invaders
            Vector2 centerOffset = new Vector2(-width * 0.5f, -height * 0.5f);
            Vector3 rowPosition = new Vector3(centerOffset.x, (2f * r) + centerOffset.y, 0f);
            
            for (int c = 0; c < col; c++)
            {
                Invader tempInvader = Instantiate(prefab[r], transform);

                Vector3 position = rowPosition;
                position.x += 2f * c;
                tempInvader.transform.localPosition = position;
            }
        }
    }
    
    //Aktiverar alla invaders igen och placerar från ursprungsposition
    public void ResetInvaders()
    {
        direction = Vector3.right;
        transform.position = initialPosition;

        foreach(Transform invader in transform)
        {
            Invader inv = invader.GetComponent<Invader>();

            invader.gameObject.SetActive(!inv.isBoss);
            inv.facingDir = 1;
        }
    }

    public void SpawnBoss()
    {
        direction = Vector3.right;
        transform.position = initialPosition;

        bossInvader.gameObject.SetActive(true);
        bossInvader.hp = 100;
    }

    //Skjuter slumpmässigt iväg en missil.
    void MissileAttack()
    {
        int nrOfInvaders = GetInvaderCount();

        if (nrOfInvaders == 0)
        {
            return;
        }

        if (!GameManager.Instance.boss)
        {
            foreach (Transform invader in transform)
            {

                if (!invader.gameObject.activeInHierarchy) //om en invader är död ska den inte kunna skjuta...
                    continue;


                float rand = UnityEngine.Random.value;
                if (rand < 0.2)
                {
                    shootingSound.Play();
                    Instantiate(missilePrefab, invader.position, Quaternion.identity);
                    Invader inv = invader.GetComponent<Invader>();
                    inv.Shake(0.1f, 0.1f, 1f);
                    inv.squash = 1f;
                    break;
                }
            }
        }
        else
        {
            // Boss attacker
            float rand = UnityEngine.Random.value;
            if(rand < 0.5f)
            {
                shootingSound.Play();
                Instantiate(missilePrefab, bossInvader.transform.position, Quaternion.identity);
                bossInvader.Shake(0.1f, 0.1f, 1f);
                bossInvader.squash = 1f;
            }
            else if(rand < 0.7f)
            {
                float step = (GameManager.Instance.rEdge - GameManager.Instance.lEdge) / 9f;

                shootingSound.Play();
                for (int i = 0; i < 9; i++)
                {
                    Instantiate(missilePrefab, new Vector2((i - 4.5f) * step, 10), Quaternion.identity);
                }
                GameManager.Instance.Shake(0.1f, 0.2f, 0f);
            }
        }
    }

    //Kollar hur många invaders som lever
    public int GetInvaderCount()
    {
        int nr = 0;

        foreach(Transform invader in transform)
        {
            if (invader.gameObject.activeSelf)
                nr++;
        }
        return nr;
    }

    //Flyttar invaders åt sidan
    void Update()
    {
        float speed = 1f;
        transform.position += speed * Time.deltaTime * direction;

        foreach (Transform invader in transform)
        {
            if (!invader.gameObject.activeInHierarchy) //Kolla bara invaders som lever
                continue;

            if (direction == Vector3.right && invader.position.x >= GameManager.Instance.rEdge - 1f)
            {
                AdvanceRow();
                break;
            }
            else if (direction == Vector3.left && invader.position.x <= GameManager.Instance.lEdge + 1f)
            {
                AdvanceRow();
                break;
            }
        }

        bool kill = true;
        if(Input.GetKeyDown(KeyCode.K)) // Fusk
        {
            foreach (Transform invader in transform)
            {
                if (kill)
                {
                    GameManager.Instance.OnInvaderKilled(invader.GetComponent<Invader>());
                    kill = false;
                }
                else
                    invader.gameObject.SetActive(false);
            }
        }
    }
    //Byter riktning och flytter ner ett steg.
    void AdvanceRow()
    {
        direction = new Vector3(-direction.x, 0, 0);
        Vector3 position = transform.position;
        position.y -= 1f;
        transform.position = position;

        foreach (Transform invader in transform)
        {
            invader.GetComponent<Invader>().facingDir = Mathf.RoundToInt(direction.x);
        }
    }
}
