using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Board : MonoBehaviour
{
    Bank bank;
    BoardDataHandler boardDataHandler;
    BoardData boardData;
    GameObject[] tiles;
    List<GameObject> tempList = new List<GameObject>();
    [SerializeField] GameObject propertyCardMenu;
    [SerializeField] GameObject propertyCardPrefab;
    [SerializeField] GameObject propertiesLayoutGroup;
    [SerializeField] PropertiesTab propertiesTab;

    int i;
    bool isMenuVisible = false;

    private void Start()
    {
        boardDataHandler = GameObject.Find("GameDataManager").GetComponent<BoardDataHandler>();
        GenerateBoard();
    }
    /// <summary>
    /// Retrieves the board data processed by the <see cref="BoardDataHandler"/> and sets the correct
    /// information for each tile on the board
    /// </summary>
    public void GenerateBoard()
    {
        //Retrieves Board data from BoardDataHandler
        boardData=boardDataHandler.GetBoardData();
        tiles = GameObject.FindGameObjectsWithTag("Tile");
        System.Array.Sort(tiles, (a, b) => { return a.GetComponent<Tile>().position.CompareTo(b.GetComponent<Tile>().position); });
        
        for (i = 0; i < boardData.tiles.Length; i++)
        {
            //Finds the tile in position i and assigns the appropriate data to it
            //Update Text Mesh Pros and colours here
            tiles[i].GetComponent<Tile>().tileData = boardData.tiles[i];
            string group = tiles[i].GetComponent<Tile>().tileData.group;
            if (group != "Unique")
            {
                tiles[i].GetComponent<Tile>().UpdateNameText();
                if (tiles[i].GetComponent<Tile>().tileData.purchasable)
                {
                    tiles[i].GetComponent<Tile>().UpdatePriceText();
                }
                if (group != "Station" && group != "Utilities" && group != "Tax" && group != "Opportunity Knocks" && group != "Pot Luck")
                {
                    tiles[i].GetComponent<Tile>().UpdateColor();
                }
            }

        }
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        AssignBanker(players[0]);
        for (i = 0; i < tiles.Length; i++)
        {
            if (tiles[i].GetComponent<Tile>().tileData.purchasable)
            {
                tempList.Add(tiles[i]);
            }
        }
        bank.properties=new GameObject[tempList.Count];
        bank.properties=tempList.ToArray();
    }
    /// <summary>
    /// Assigns given player the banker role
    /// </summary>
    /// <param name="p">Player game object</param>
    public void AssignBanker(GameObject p)
    {
        p.AddComponent<Bank>();
        bank = p.GetComponent<Bank>();
    }

    public Bank GetBank()
    {
        return bank;
    }
    /// <summary>
    /// Toggles the visibility of the property menu showing owned / mortgaged properties
    /// </summary>
    /// <param name="p">Player game object</param>
    public void TogglePropertyMenu(GameObject p)
    {
        propertiesTab.SetBlank();
        if (propertyCardMenu != null)
        {
            if (!isMenuVisible)
            {
                DestroyAllChildrenInPropertyLayoutGroup();
            }
            isMenuVisible = !isMenuVisible;
            propertyCardMenu.SetActive(isMenuVisible);
            //fill property menu with releveant players owned card data
            RefreshMenu(p);
        }
    }
    /// <summary>
    /// Regenerates the property menu, updating the menu to show which properties are owned in relation to the current player
    /// </summary>
    /// <param name="p">Player game object</param>
    public void RefreshMenu(GameObject p)
    {
        DestroyAllChildrenInPropertyLayoutGroup();
        foreach (GameObject tile in tiles)
        {
            Tile card = tile.GetComponent<Tile>();
            if (card.tileData.purchasable)
            {
                GameObject propertyCard = Instantiate(propertyCardPrefab, propertiesLayoutGroup.transform);
                Property propertyData = propertyCard.GetComponent<Property>();
                propertyData.SetOwnedBy(tile.GetComponent<Property>().GetOwnedBy());
                propertyData.isMortgaged = tile.GetComponent<Property>().isMortgaged;
                propertyData.SetAssociatedTile(card);


                if (propertyCard.GetComponent<Property>().GetOwnedBy() != p)
                {
                    CanvasGroup canvasGroup = propertyCard.GetComponent<CanvasGroup>();
                    canvasGroup.alpha = 0.6f;
                    canvasGroup.interactable = false;
                }
                if(propertyCard.GetComponent<Property>().GetOwnedBy() == p && propertyCard.GetComponent<Property>().isMortgaged)
                {
                    //display mortagge UI
                    //add buy back button - maybe
                }

            }

        }
    }
    /// <summary>
    /// Destroy all property game objects in the property menu
    /// </summary>
    public void DestroyAllChildrenInPropertyLayoutGroup()
    {
        foreach(Property child in propertiesLayoutGroup.GetComponentsInChildren<Property>())
        {
            Destroy(child.gameObject);
        }
    }

    public GameObject[] GetTileArray()
    {
        return tiles;
    }
}
