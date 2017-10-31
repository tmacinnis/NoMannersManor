using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class LogoControl : MonoBehaviour
{

    float timer = 0;
    public int waitTime = 5;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if (timer > waitTime)
        {
            SceneManager.LoadScene(1);
        }
    }
}