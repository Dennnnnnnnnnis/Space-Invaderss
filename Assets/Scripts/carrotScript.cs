using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class carrotScript1 : MonoBehaviour
{
    //h�mtar rigidbodyn f�r att kunna l�gga till kraft p� den
    [SerializeField]
    Rigidbody rb;
    //s�ttar farten p� mor�tterna
    int speed = 1;
    //spelarens kontroller
    SC_FPSController playerController;
    // Start is called before the first frame update
    void Start()
    {
        // h�mtar spelarens skript f�r att kunna f� tag p� en variabel
        playerController = FindObjectOfType<SC_FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        //n�r moroten spawnar �ker den fram�t med en kraft som bara trycker i b�rjan
        rb.AddForce(gameObject.transform.forward * speed, ForceMode.Impulse);
        //n�r antalet kaniner �r 0 g�r man tillbaka till menyn och kan anv�nda musen igen
        if (playerController.rabbitCount == 0)
        {
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //om det man kolliderar med inte �r spelaren tas moroten bort
        if(other.gameObject.tag != "Player")
        {
            // om man tr�ffar en kanin tas den bort och antalet kaniner �kar.
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
