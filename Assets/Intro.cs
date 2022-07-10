using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Intro : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Gotogame", 14f);
    }

    void Gotogame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
