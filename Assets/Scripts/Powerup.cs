using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] GameManager gM;
    [SerializeField]
    GameObject tandBorste, tänder;
    int last = 0;
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

        if(gM.score % 400 >= 200 && last < 1)
        {
            Instantiate(tandBorste);
            last = 1;

        }
        else if (gM.score % 400 < 200 && last == 1)
        {
            Instantiate(tänder);
            last = 0;
        }
    }
}
