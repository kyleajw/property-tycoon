using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Auction : MonoBehaviour
{
    Queue<GameObject> biddingQueue;
    bool biddingInProgress = false;
    GameObject currentPlayer;
    int biddingTotal;
    GameObject propertyBeingAuctioned;
    [SerializeField]TextMeshProUGUI currentBidderText;
    [SerializeField] GameObject auctionGUI;
    [SerializeField]TextMeshProUGUI currentBidAmountText;
    [SerializeField]Button bid1Button;
    [SerializeField]Button bid10Button;
    [SerializeField]Button bid100Button;
    [SerializeField] Button leaveAuctionButton;
    [SerializeField] Board board;
    [SerializeField] GameObject bidAnnouncementHistoryGUI;
    [SerializeField] GameObject bidAnnouncementGUIPrefab;

    [SerializeField] Image displayedPropertyColour;
    [SerializeField] TextMeshProUGUI originalPropertyPriceText;
    [SerializeField] TextMeshProUGUI propertyNameText;
    


    public void Setup(GameObject player, GameObject[] players, GameObject propertyToAuction)
    {
        ClearAnnouncementHistory();
        biddingQueue = new Queue<GameObject>();
        propertyBeingAuctioned = propertyToAuction;
        DisplayCardBeingAuctioned();
        Player playersInfo = player.GetComponent<Player>();
        int currentPlayerIndex = playersInfo.GetPlayerNumber() - 1;
        for (int i = currentPlayerIndex; i < players.Length; i++)
        {
            if (!players[i].GetComponent<Player>().IsInJail())
            {
                biddingQueue.Enqueue(players[i]);
            }
        }
        for (int i = 0; i < currentPlayerIndex; i++)
        {
            if (!players[i].GetComponent<Player>().IsInJail())
            {
                biddingQueue.Enqueue(players[i]);
            }
        }
        biddingTotal = 0;
        biddingInProgress = true;
        currentPlayer = biddingQueue.Dequeue();
        AnnounceCurrentBidder();
        if (currentPlayer.GetComponent<Player>().IsHuman())
        {

            EnableAppropriateBidButtons();
        }
        else
        {
            DisableBidButtons();
            currentPlayer.GetComponent<EasyAgent>().OnMyTurnInAuction();
        }
    }

    void ClearAnnouncementHistory()
    {
        foreach (Transform child in bidAnnouncementHistoryGUI.transform) {
            Destroy(child.gameObject);
        }
    }

    void DisplayCardBeingAuctioned()
    {
        Tile property = propertyBeingAuctioned.GetComponent<Tile>();
        displayedPropertyColour.color = property.GetColor();
        originalPropertyPriceText.text = property.priceText.text;
        propertyNameText.text = property.nameText.text;
    }

    public bool IsBiddingInProgress()
    {
        return biddingInProgress;
    }

    public void OnPlayerLeavesAuction()
    {
        AddBidMsgToBidAnnouncementHistory("Player " + currentPlayer.GetComponent<Player>().GetPlayerNumber() + " has left the auction.");
        NextBidder();
        AnnounceCurrentBidder();

    }

    public void OnPlayerBids(int amount)
    {
        AddBidMsgToBidAnnouncementHistory($"Player {currentPlayer.GetComponent<Player>().GetPlayerNumber()} has bid £{amount + biddingTotal}");
        biddingQueue.Enqueue(currentPlayer);
        biddingTotal += amount;
        UpdateCurrentHighestBid();
        NextBidder();
        AnnounceCurrentBidder();
    }

    void AddBidMsgToBidAnnouncementHistory(string msg)
    {
        if(bidAnnouncementHistoryGUI.transform.childCount == 7)
        {
            Destroy(bidAnnouncementHistoryGUI.transform.GetChild(0).gameObject);
        }
        GameObject newBidAnnouncement = Instantiate(bidAnnouncementGUIPrefab, bidAnnouncementHistoryGUI.transform);
        newBidAnnouncement.GetComponentInChildren<TextMeshProUGUI>().text = msg;
        newBidAnnouncement.GetComponent<Image>().color = currentPlayer.GetComponent<Player>().GetPlayerColour();


    }

    // temp fix, follows same functionality as BuyProperty() method
    void OnAuctionFulfilled()
    {
        Player player = currentPlayer.GetComponent<Player>();
        Property property = propertyBeingAuctioned.GetComponent<Property>();
        bool propertyFoundInBankArray = false;
        int i = 0;
        int j = -1;
        while (!propertyFoundInBankArray || i < board.GetBank().properties.Length)
        {
            string propertyName = propertyBeingAuctioned.GetComponent<Tile>().tileData.spaceName;
            if(board.GetBank().properties[i] != null)
            {
                if (propertyName == board.GetBank().properties[i].GetComponent<Tile>().tileData.spaceName)
                {
                    propertyFoundInBankArray = true;
                    j = i;
                }
            }
            i++;
        }
        if (j == -1)
        {
            Debug.Log("CANT FIND PROPERTY");
        }

        player.ownedProperties[j] = board.GetBank().properties[j];
        board.GetBank().properties[j].GetComponent<Property>().SetOwnedBy(currentPlayer);
        board.GetBank().properties[j] = null;
        player.SetBalance(-biddingTotal);

        //CloseAuction();
        StartCoroutine(WaitForSecondsThenCloseAuction());
        
    }

    void NextBidder()
    {
        DisableBidButtons();
        currentPlayer = biddingQueue.Dequeue();
        Player player = currentPlayer.GetComponent<Player>();
        if( biddingQueue.Count == 0)
        {
            AddBidMsgToBidAnnouncementHistory("Player " + player.GetPlayerNumber() + " wins this bid");
            OnAuctionFulfilled();
        }
        else
        {
            if (player.GetBalance() < biddingTotal + 1)
            {
                AddBidMsgToBidAnnouncementHistory($"Player {player.GetPlayerNumber()} has left the auction (not enough to bid)");
                NextBidder();
            }
            else
            {
                if (player.IsHuman())
                {
                    EnableAppropriateBidButtons();
                }
                else
                {
                    currentPlayer.GetComponent<EasyAgent>().OnMyTurnInAuction();
                }
            }
        }

    }

    void CloseAuction()
    {
        auctionGUI.SetActive(false);
        PlayerManager playerManager = gameObject.GetComponent<PlayerManager>();
        GameObject player = playerManager.GetCurrentPlayer();
        if (!player.GetComponent<Player>().IsHuman())
        {
            player.GetComponent<EasyAgent>().EndTurn();
        }
    }

    IEnumerator WaitForSecondsThenCloseAuction()
    {
        yield return new WaitForSeconds(2.5f);
        auctionGUI.SetActive(false);
        PlayerManager playerManager = gameObject.GetComponent<PlayerManager>();
        GameObject player = playerManager.GetCurrentPlayer();
        if (!player.GetComponent<Player>().IsHuman())
        {
            player.GetComponent<EasyAgent>().EndTurn();
        }
    }

    void DisableBidButtons()
    {
        bid1Button.interactable =false;
        bid10Button.interactable = false;
        bid100Button.interactable = false;
        leaveAuctionButton.interactable = false;
    }

    void EnableAppropriateBidButtons()
    {
        Player player = currentPlayer.GetComponent<Player>();
        if (player.GetBalance() >= biddingTotal + 100)
        {
            bid100Button.interactable = true;
        }
        if (player.GetBalance() >= biddingTotal + 10)
        {
            bid10Button.interactable = true;
        }
        if (player.GetBalance() >= biddingTotal + 1)
        {
            bid1Button.interactable = true;
        }
        leaveAuctionButton.interactable = true;
    }

    void AnnounceCurrentBidder()
    {
        currentBidderText.text = $"Player {currentPlayer.GetComponent<Player>().GetPlayerNumber()}'s turn to bid";
    }
    void UpdateCurrentHighestBid()
    {
        currentBidAmountText.text = $"Current bid: £{biddingTotal}";

    }

    public GameObject GetPropertyBeingAuctioned()
    {
        return propertyBeingAuctioned;
    }

    public int GetHighestBid()
    {
        return biddingTotal;
    }

}
