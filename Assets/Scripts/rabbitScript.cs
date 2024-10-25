using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class rabbitScript : MonoBehaviour
{
    // f�r tag p� navmeshens agen f�r att kunna s�tta in en destination
    [SerializeField] NavMeshAgent agent;
    // f�r tag i spelaren f�r att kunna se dens position
    [SerializeField] GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //s�tter destinationen till spelaren
        agent.SetDestination(player.transform.position);
    }
}
