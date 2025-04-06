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

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("AI INSTANTIATED");
        player = gameObject.GetComponent<Player>();
        playerManager = GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>();
    }

    public void OnMyTurn()
    {
        StartCoroutine(RollDice());
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

    }

    public void EndTurn()
    {
        StartCoroutine(WaitForSecondsThenEndTurn());
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
        player.SetTurn(false);
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
        else
        {
            Debug.Log("Property Auctioned");
            playerManager.AuctionPressed();
        }
    }

}
