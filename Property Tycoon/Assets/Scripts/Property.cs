using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Property : MonoBehaviour
{
    GameObject ownedBy;
    bool isMortgaged=false;
    int houses = 0;
    public TextMeshProUGUI rentText,nameText;

    public void UpdateNameText()
    {
        nameText.text = "a";
    }
    public void UpdateRentText()
    {
        rentText.text = "b";
    }
    public GameObject GetOwnedBy(){ 
        return ownedBy;
    }
    public int GetHouses()
    {
        return houses;
    }
    public void SetHouses(int num)
    {
        houses += num;
    }
    public void SetOwnedBy(GameObject player)
    {
        ownedBy = player;
    }
}
