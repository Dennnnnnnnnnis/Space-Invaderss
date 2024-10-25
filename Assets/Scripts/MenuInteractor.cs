using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuInteractor : MonoBehaviour
{
    public AudioSource buttonSound;

    // Update is called once per frame
    public void OnButtonClick()
    {
        buttonSound.Play();
    }
}
