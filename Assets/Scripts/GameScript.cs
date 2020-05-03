using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    public string gameMode;

    public void Awake()
    {
        GameObject persistantObj = GameObject.FindGameObjectWithTag("GameMode") as GameObject;
        gameMode = persistantObj.GetComponent<PersistanceScript>().gameMode;
        print(gameMode);
        Destroy(persistantObj);
    }
}
