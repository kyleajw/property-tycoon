using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    Player player;

    int balance = 50000;
    public GameObject[] properties;
    public void SetBankBalance(int cost)
    {
        balance = balance + cost;
    }
}
