using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayersList : MonoBehaviour
{
    [SerializeField] GameObject playerCardPrefab;
    [SerializeField] Transform addPlayerButton;
    [SerializeField] Button beginGameButton;

    List<GameObject> playerCards;
    const int MAX_PLAYERS = 5;
    const int MIN_PLAYERS = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerCards = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(addPlayerButton.gameObject.activeInHierarchy && playerCards.Count >= MAX_PLAYERS)
        {
            addPlayerButton.gameObject.SetActive(false);
        }else if (!addPlayerButton.gameObject.activeInHierarchy && playerCards.Count < MAX_PLAYERS)
        {
            addPlayerButton.gameObject.SetActive(true);
        }

        if (playerCards.Count < MIN_PLAYERS && beginGameButton.IsInteractable())
        {
            beginGameButton.interactable = false;
        }
        else if (playerCards.Count >= MIN_PLAYERS && !beginGameButton.IsInteractable())
        {
            beginGameButton.interactable = true;
        }
    }

    public void AddPlayerCard()
    {
        GameObject newPlayer = Instantiate(playerCardPrefab,transform.position, Quaternion.identity, this.gameObject.transform);
        PlayerCard playerCard = newPlayer.GetComponent<PlayerCard>();
        playerCards.Add(newPlayer);
        playerCard.SetPlayerNumber(playerCards.Count);


        addPlayerButton.SetAsLastSibling();
    }

    public void RemovePlayerCard(int index)
    {
        playerCards.RemoveAt(index-1);
        UpdatePlayerNumbers();
    }

    void UpdatePlayerNumbers()
    {
        for (int i = 0; i < playerCards.Count; i++)
        {
            playerCards[i].GetComponent<PlayerCard>().SetPlayerNumber(i+1);
        }
    }

}
