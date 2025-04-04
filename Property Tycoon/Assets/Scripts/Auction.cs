using System.Collections;
using System.Collections.Generic;
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
    


    public void Setup(GameObject player, GameObject[] players, GameObject propertyToAuction)
    {
        biddingQueue = new Queue<GameObject>();
        propertyBeingAuctioned = propertyToAuction;
        Player playersInfo = player.GetComponent<Player>();
        int currentPlayerIndex = playersInfo.GetPlayerNumber() - 1;
        for (int i = currentPlayerIndex; i < players.Length; i++)
        {
            biddingQueue.Enqueue(players[i]);
        }
        for (int i = 0; i < currentPlayerIndex; i++)
        {
            biddingQueue.Enqueue(players[i]);
        }
        biddingTotal = 0;
        biddingInProgress = true;
        currentPlayer = biddingQueue.Dequeue();
        AnnounceCurrentBidder();
    }

    public bool IsBiddingInProgress()
    {
        return biddingInProgress;
    }

    public void OnPlayerLeavesAuction()
    {
        Debug.Log("Player "+currentPlayer.GetComponent<Player>().GetPlayerNumber() + " has left the auction.");
        NextBidder();
        AnnounceCurrentBidder();

    }

    public void OnPlayerBids(int amount)
    {
        biddingQueue.Enqueue(currentPlayer);
        biddingTotal += amount;
        UpdateCurrentHighestBid();
        NextBidder();
        AnnounceCurrentBidder();
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
            if (propertyName == board.GetBank().properties[i].GetComponent<Tile>().tileData.spaceName)
            {
                propertyFoundInBankArray = true;
                j = i;
            }
            i++;
        }
        
        player.ownedProperties[j] = board.GetBank().properties[j];
        board.GetBank().properties[j].GetComponent<Property>().SetOwnedBy(currentPlayer);
        board.GetBank().properties[j] = null;
        player.SetBalance(-biddingTotal);

        CloseAuction();
        
    }

    void NextBidder()
    {
        DisableBidButtons();
        currentPlayer = biddingQueue.Dequeue();
        Player player = currentPlayer.GetComponent<Player>();
        if( biddingQueue.Count == 0)
        {
            Debug.Log("Player " + player.GetPlayerNumber() + " wins this bid");
            OnAuctionFulfilled();
        }
        else
        {
            if (player.GetBalance() < biddingTotal + 1)
            {
                Debug.Log($"Player {player.GetPlayerNumber()} has left the auction (not enough to bid)");
                NextBidder();
            }
            else
            {
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
        }

    }

    void CloseAuction()
    {
        auctionGUI.SetActive(false);
    }

    void DisableBidButtons()
    {
        bid1Button.interactable =false;
        bid10Button.interactable = false;
        bid100Button.interactable = false;
        leaveAuctionButton.interactable = false;
    }

    void AnnounceCurrentBidder()
    {
        currentBidderText.text = $"Player {currentPlayer.GetComponent<Player>().GetPlayerNumber()}'s turn to bid";
    }
    void UpdateCurrentHighestBid()
    {
        currentBidAmountText.text = $"Current bid: £{biddingTotal}";

    }

}
