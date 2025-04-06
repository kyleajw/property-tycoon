using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyAgent : MonoBehaviour
{
    Player player;
    PlayerManager playerManager;

    float minWait = 0.5f;
    float maxWait = 1.5f;
    float buyFactor = 0.7f;
    float houseUpgradeFactor = 0.4f;

    // Start is called before the first frame update
    //if cant pay, retire
    void Start()
    {
        Debug.Log("AI INSTANTIATED");
        //player = gameObject.GetComponent<Player>();
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
    }

    public void OnMyTurn()
    {
        player = gameObject.GetComponent<Player>();
        Debug.Log("my turn");
        Debug.Log(player.IsInJail());
        if (!player.IsInJail())
        {
            StartCoroutine(WaitForSecondsThenComputeTurn());
        }
        else
        {
            EndTurn(); // temp
        }
    }

    public void OnCardDrawn(bool multiChoice)
    {
        StartCoroutine(WaitForSecondsThenCloseCardDialog(multiChoice));
        
    }

    public void OnLandsOnPurchasableProperty()
    {
        StartCoroutine(WaitForSecondsThenBuyOrAuctionProperty());   
    }

    public void OnMyTurnInAuction()
    {
        StartCoroutine(WaitForSecondsThenMakeAuctionDecision());
    }

    public void EndTurn()
    {
        StartCoroutine(WaitForSecondsThenEndTurn());
    }

    IEnumerator WaitForSecondsThenComputeTurn()
    {
        Debug.Log("checking owned properties");
        for (int i = 0; i < player.ownedProperties.Length; i++)
        {
            if(player.ownedProperties[i] != null)
            {
                string group = player.ownedProperties[i].GetComponent<Tile>().tileData.group;
                if (group != "Station" && group != "Utilities" && playerManager.isColourGroupOwned(group, gameObject))
                {
                    int price = 99999;
                    int balance = player.GetBalance();
                    switch (group)
                    {
                        case "Brown":
                            price = 50;
                            break;
                        case "Blue":
                            price = 50;
                            break;
                        case "Purple":
                            price = 100;
                            break;
                        case "Orange":
                            price = 100;
                            break;
                        case "Red":
                            price = 150;
                            break;
                        case "Yellow":
                            price = 150;
                            break;
                        case "Green":
                            price = 200;
                            break;
                        case "Deep Blue":
                            price = 200;
                            break;
                    }
                    if(price < balance * houseUpgradeFactor)
                    {
                        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
                        playerManager.BuildHousePressed(player.ownedProperties[i].GetComponent<Property>());
                        Debug.Log("AI Agent buys house");
                    }
                }
                
            }
        }


        // roll
        StartCoroutine(RollDice());
    }

    IEnumerator RollDice()
    {
        
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        float timeRolling = Random.Range(0.1f, 1.5f);
        yield return new WaitForSeconds(timeRolling);
        playerManager.OnRollButtonReleased(timeRolling);
    }

    IEnumerator WaitForSecondsThenEndTurn()
    {
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        playerManager.OnFinishedTurn();
    }

    IEnumerator WaitForSecondsThenCloseCardDialog(bool multiChoice)
    {
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        int choice = 1;
        if (multiChoice)
        {
            choice = Random.Range(1, 3);
        }

        GameObject.FindGameObjectWithTag("Card").GetComponent<CardDialog>().Close(choice);
    }

    IEnumerator WaitForSecondsThenBuyOrAuctionProperty()
    {
        yield return new WaitForSeconds(Random.Range(minWait,maxWait));
        int balance = player.GetBalance();
        int price = player.GetCurrentTile().GetComponent<Tile>().tileData.purchaseCost;
        if (price < balance * buyFactor)
        {
            playerManager.BuyPressed();
            Debug.Log("Property bought");
            EndTurn();
        }
        else if(playerManager.CheckCycles() >=2)
        {
            Debug.Log("Property Auctioned");
            playerManager.AuctionPressed();
        }
        else
        {
            EndTurn();
        }
    }

    IEnumerator WaitForSecondsThenMakeAuctionDecision()
    {
        yield return new WaitForSeconds(Random.Range(minWait, maxWait));
        Auction auction = playerManager.gameObject.GetComponent<Auction>();
        Tile propertyBeingAuctioned = auction.GetPropertyBeingAuctioned().GetComponent<Tile>();
        if(propertyBeingAuctioned.tileData.purchaseCost > auction.GetHighestBid())
        {
            // bid 1/10/100
            int balance = player.GetBalance();
            int highestBid = auction.GetHighestBid();
            if(balance > highestBid + 100)
            {
                auction.OnPlayerBids(100);
            }else if(balance > highestBid + 10)
            {
                auction.OnPlayerBids(10);
            }
            else
            {
                auction.OnPlayerBids(1);
            }
        }
        else
        {
            auction.OnPlayerLeavesAuction();
        }
    }


}
