using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Tile : MonoBehaviour
{
    public TileData tileData;
    [SerializeField]
    public int position = 0;
    public TMP_Text nameText, priceText;
    MeshRenderer ren;
    IDictionary colorDictionary;

    void Start()
    {
        ren = GetComponentInChildren<MeshRenderer>();
        colorDictionary = new Dictionary<string, Color>();

        colorDictionary.Add("Brown", new Color(0.4f, 0.21f, 0.09f, 1f));
        colorDictionary.Add("Blue", new Color(0.69f, 1f, 1f, 1f));
        colorDictionary.Add("Purple", new Color(1f, 0.15f, 0.57f, 1f));
        colorDictionary.Add("Orange", new Color(1f, 0.64f, 0f, 1f));
        colorDictionary.Add("Red", Color.red);
        colorDictionary.Add("Yellow", Color.yellow);
        colorDictionary.Add("Green", Color.green);
        colorDictionary.Add("Deep Blue", Color.blue);
    }

    public void UpdateText()
    {
        nameText.text=tileData.spaceName;
        priceText.text=tileData.purchaseCost.ToString();
    }
    public void UpdateColor()
    {
        ren.materials[1].SetColor("_Color", (Color)colorDictionary[tileData.group]);
    }

}
