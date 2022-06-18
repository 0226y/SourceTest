using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInit : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadScene("SceneGame");
    }
}
