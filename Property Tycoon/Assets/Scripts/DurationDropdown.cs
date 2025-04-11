using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DurationDropdown : MonoBehaviour
{
    int dropdownIndex = 0;
    TMPro.TMP_Dropdown dropdown;
    private void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }
    public void VersionDropdownPressed()
    {
        GameObject.FindGameObjectWithTag("GameManager").GetComponent<LobbyHandler>().SetGameDuration(dropdown.value);
    }
}
