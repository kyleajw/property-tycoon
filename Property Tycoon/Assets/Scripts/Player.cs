using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject[] pieces;
    GameObject playerPiece;
    public void AssignPiece(int i)
    {
        playerPiece=pieces[i];
        Debug.Log(playerPiece);
    }

}
