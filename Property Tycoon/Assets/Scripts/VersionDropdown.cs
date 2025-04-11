using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VersionDropdown : MonoBehaviour
{
    int dropdownIndex = 0;
    TMPro.TMP_Dropdown dropdown;
    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }
    public void VersionDropdownPressed()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<LobbyHandler>().SetGameVersion(dropdown.value);
    }
}
