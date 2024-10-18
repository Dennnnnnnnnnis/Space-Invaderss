using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] GameManager gM;
    [SerializeField]
    GameObject tandBorste, tänder;
    int lastScore = 0;
    GameObject tandBorste;
    int xPosition;

    // Start is called before the first frame update
    void Start()
    {
        gM = GameManager.Instance;
        xPosition = Random.Range(-10, 10);
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
        else if(gM.score >= 500 && lastScore < 500)
        {
            Instantiate(tandBorste, (new Vector3(xPosition, 0, 0)), Quaternion.identity);
            lastScore = 500

        }
    }
}
