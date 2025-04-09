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

    /// <summary>
    /// Adds all colour groups to the colour dictionary
    /// </summary>
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
    /// <summary>
    /// Sets the visible text on the tile to the name of the associated tile in the JSON file
    /// </summary>
    public void UpdateNameText()
    {
        nameText.text=tileData.spaceName;
    }
    /// <summary>
    /// Sets the visible text on the tile to the purchases cost of the associated tile in the JSON file
    /// </summary>
    public void UpdatePriceText()
    {
        priceText.text = $"£{ tileData.purchaseCost.ToString()}";
    }
    /// <summary>
    /// Sets the visible text on the tile to the associated colour group from the corresponding tile in the JSON file
    /// </summary>
    public void UpdateColor()
    {
        ren.materials[1].SetColor("_Color", (Color)colorDictionary[tileData.group]);
        ren.materials[1].SetColor("_EmissionColor", (Color)colorDictionary[tileData.group]);

    }
    /// <summary>
    /// Gets the colour of the tile. If the dictionary does not contain the name of the tile's colour group, return white (e.g. station)
    /// </summary>
    /// <returns>Colour of the tile object</returns>
    public Color GetColor()
    {
        if (colorDictionary.Contains(tileData.group)){
            return (Color)colorDictionary[tileData.group];
        }
        return Color.white;
    }

}
