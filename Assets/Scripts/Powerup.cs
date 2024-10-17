using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField] GameManager gM;
    [SerializeField]
    GameObject tandBorste;
    bool spawnable = true;
    // Start is called before the first frame update
    void Start()
    {
        gM = GameManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {

        if(gM.score >= 100 && spawnable)
        {
            Instantiate(tandBorste);
            spawnable = false;

        }
    }
}
