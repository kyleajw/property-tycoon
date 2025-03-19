using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropertyCard : MonoBehaviour
{
    PropertiesTab propertiesTab;
    // Start is called before the first frame update
    void Start()
    {
        propertiesTab = GetComponentInParent<PropertiesTab>();
    }

    public void OnSelected(Property property)
    {
        propertiesTab.SetCardOnDisplay(property);
    }
}
