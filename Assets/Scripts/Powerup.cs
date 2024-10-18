using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] GameManager gM;
    [SerializeField]
    GameObject tandBorste;
    bool spawnable = true;
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

        if(gM.score >= 500 && spawnable)
        {
            Instantiate(tandBorste, (new Vector3(xPosition, 0, 0)), Quaternion.identity);
            spawnable = false;

        }
    }
}
