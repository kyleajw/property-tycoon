using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    public void StartGamePressed()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<LobbyHandler>().LoadGame();
    }
}
