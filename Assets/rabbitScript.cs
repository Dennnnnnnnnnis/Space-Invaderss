using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class rabbitScript : MonoBehaviour
{
    // får tag på navmeshens agen för att kunna sätta in en destination
    [SerializeField] NavMeshAgent agent;
    // får tag i spelaren för att kunna se dens position
    [SerializeField] GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //sätter destinationen till spelaren
        agent.SetDestination(player.transform.position);
    }
}
