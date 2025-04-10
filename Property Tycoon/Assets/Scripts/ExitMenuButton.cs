using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitMenuButton : MonoBehaviour
{
    public void ExitMenuPressed()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<LobbyHandler>().ExitToMenu();
    }
}
