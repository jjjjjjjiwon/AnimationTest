using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Test : MonoBehaviour
{
    int i = 1081;
    public GameObject PBgm;
    private GameObject PVgm;
    void Awake()
    {
        PVgm = Instantiate(PBgm);

    }

    // 임시
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SceneManager.LoadScene("Lobby");
    }
}
