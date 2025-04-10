using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BalanceCard : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI playerNameText;
    [SerializeField] TextMeshProUGUI playerBalanceText;
    [SerializeField] Image playerBalanceBorder;

    public void SetBorderColour(Color colour)
    {
        playerBalanceBorder.color = colour;
    }

    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    public void SetPlayerBalance(int balance)
    {
        playerBalanceText.text = $"£{balance}";
    }


}
