using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class carrotScript1 : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    int speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rb.AddForce(gameObject.transform.forward * speed, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
            if(other.gameObject.tag == "Rabbit")
            {
                Destroy(other.gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

        }

    }
}
