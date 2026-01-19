using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    int i = 1081;
    GameObject PBgm;
    private GameObject PVgm;
    void Awake()
    {
        PVgm = Instantiate(PBgm);

    }

    // 임시
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("=======================================");
            SceneManager.LoadScene("Lobby");   
        }
    }
}
