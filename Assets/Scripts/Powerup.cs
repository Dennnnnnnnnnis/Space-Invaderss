using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] GameManager gM;
    [SerializeField]
    GameObject tandBorste, tänder;
    int lastScore = 0;
    // Start is called before the first frame update
    void Start()
    {
        gM = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

        if(gM.score >= 100 && lastScore < 100)
        {
            Instantiate(tandBorste);
            lastScore = 100;

        }
        else if (gM.score >= 200 && lastScore < 200)
        {
            Instantiate(tänder);
            lastScore = 200;

        }
    }
}
