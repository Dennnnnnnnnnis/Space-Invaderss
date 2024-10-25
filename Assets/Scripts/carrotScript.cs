using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Timeline.TimelinePlaybackControls;
using UnityEngine.SceneManagement;
using TMPro;

public class carrotScript1 : MonoBehaviour
{
    [SerializeField]
    Rigidbody rb;
    [SerializeField]
    int speed;
    SC_FPSController playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<SC_FPSController>();
    }

    // Update is called once per frame
    void Update()
    {

        rb.AddForce(gameObject.transform.forward * speed, ForceMode.Impulse);
        if (playerController.rabbitCount == 0)
        {
          SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 2);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag != "Player")
        {
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
