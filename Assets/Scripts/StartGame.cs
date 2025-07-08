using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class StartGame : MonoBehaviour
{
    public void LoadNextScene()
    {
        SceneManager.LoadScene("GameScene");

    }


}

