using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class carrotScript1 : MonoBehaviour
{
    //hämtar rigidbodyn för att kunna lägga till kraft på den
    [SerializeField]
    Rigidbody rb;
    //sättar farten på morötterna
    int speed = 1;
    //spelarens kontroller
    SC_FPSController playerController;
    // Start is called before the first frame update
    void Start()
    {
        // hämtar spelarens skript för att kunna få tag på en variabel
        playerController = FindObjectOfType<SC_FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        //när moroten spawnar åker den framåt med en kraft som bara trycker i början
        rb.AddForce(gameObject.transform.forward * speed, ForceMode.Impulse);
        //när antalet kaniner är 0 går man tillbaka till menyn och kan använda musen igen
        if (playerController.rabbitCount == 0)
        {
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //om det man kolliderar med inte är spelaren tas moroten bort
        if(other.gameObject.tag != "Player")
        {
            // om man träffar en kanin tas den bort och antalet kaniner ökar.
            if(other.gameObject.tag == "Rabbit")
            {
                Destroy(other.gameObject);
                playerController.rabbitCount -= 1;
            }
            else
            {
                Destroy(gameObject);
            }

        }

    }
}
