using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Property : MonoBehaviour
{
    GameObject ownedBy;
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
}
