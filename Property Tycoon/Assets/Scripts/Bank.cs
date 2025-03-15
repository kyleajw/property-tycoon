using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bank : MonoBehaviour
{
    int balance = 50000;
    public GameObject[] properties;
    public void SetBankBalance(int cost)
    {
        balance = balance + cost;
    }
    public GameObject GetProperty(int i)
    {
        return properties[i];
    }
}
