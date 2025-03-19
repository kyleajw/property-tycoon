using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Property : MonoBehaviour
{
    GameObject ownedBy;
    bool isMortgaged=false;
    int houses = 0;
    public TextMeshProUGUI rentText,nameText;
    [SerializeField] Image cardColour;

    public void UpdateNameText(string name)
    {
        nameText.text = name;
    }
    public void UpdateRentText(int price)
    {
        rentText.text = $"£{price}";
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
    public void UISetColour(Color colour)
    {
        cardColour.color = colour;
    }
}
