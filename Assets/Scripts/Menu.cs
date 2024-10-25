using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //när knappen trycks på ökar scenen med 1, villket tar en till huvudspelet
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    //ökar scenen med 2 och tar en till 3d-spelet
    public void Play3DGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }

    //stänger av spelet
    public void QuitGame()
    {
        Application.Quit();
    }
}
