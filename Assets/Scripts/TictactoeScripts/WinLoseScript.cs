using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseScript : MonoBehaviour
{
    public static WinLoseScript instance;
    void Awake()
    {
        MakeInstance();
    }
    void MakeInstance()
    {
        if (instance == null)
        {
            instance = this; //neu instance ==null thi ko co Gamecontroller, cho instance =this, this se tro den lop gan nhat cua Gamecontroller
        }
    }
    [SerializeField]
    public GameObject player1winPanel, player2winPanel, CPUwinPanel;
    // Start is called before the first frame update
    public void Player1Win()
    {
        player1winPanel.SetActive(true);
    }
    public void Player2Win()
    {
        player2winPanel.SetActive(true);
    }
    public void CPUWin(){
        CPUwinPanel.SetActive(true);
    }
    public void AgainButton()
    {
        Application.LoadLevel("MainScreen");
    }
}
