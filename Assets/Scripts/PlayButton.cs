using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public string gameMode;
    public GameObject PersistantObject;

    public void OnMouseDown()
    {
        PersistantObject.GetComponent<PersistanceScript>().gameMode = gameMode;
        SceneManager.LoadScene("GameScreen");
    }
}
