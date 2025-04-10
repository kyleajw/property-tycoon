using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayPlayerBalances : MonoBehaviour
{
    [SerializeField]PlayerManager playerManager;
    [SerializeField] GameObject playerBalanceCardPrefab;
    Dictionary<string, GameObject> playerBalanceCards;

    private void Start()
    {
        playerBalanceCards = new Dictionary<string, GameObject>();
        foreach(GameObject playerObject in playerManager.GetPlayers())
        {
            Player player = playerObject.GetComponent<Player>();
            GameObject playerBalanceCard = Instantiate(playerBalanceCardPrefab, transform);
            playerBalanceCard.GetComponent<BalanceCard>().SetPlayerBalance(player.GetBalance());
            playerBalanceCard.GetComponent<BalanceCard>().SetPlayerName(player.GetPlayerName());
            playerBalanceCard.GetComponent<BalanceCard>().SetBorderColour(player.GetPlayerColour());
            playerBalanceCards.Add(player.GetPlayerName(), playerBalanceCard);

        }
    }
    // Update is called once per frame
    void Update()
    {
        foreach (GameObject playerObject in playerManager.GetPlayers())
        {
            Player player = playerObject.GetComponent<Player>();
            playerBalanceCards[player.GetPlayerName()].GetComponent<BalanceCard>().SetPlayerBalance(player.GetBalance());
        }
    }
}
