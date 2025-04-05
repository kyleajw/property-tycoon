using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyAgent : MonoBehaviour
{
    Player player;
    PlayerManager playerManager;

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
        // if price < 40% of balance, buy it
        // else auction
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
        
        yield return new WaitForSeconds(Random.Range(1.0f, 3.0f));
        float timeRolling = Random.Range(0.1f, 1.5f);
        yield return new WaitForSeconds(timeRolling);
        playerManager.OnRollButtonReleased(timeRolling);
    }

    IEnumerator WaitForSecondsThenEndTurn()
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        player.SetTurn(false);
    }

    IEnumerator WaitForSecondsThenCloseCardDialog(bool multiChoice)
    {
        yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
        int choice = 1;
        if (multiChoice)
        {
            choice = Random.Range(1, 3);
        }

        GameObject.FindGameObjectWithTag("Card").GetComponent<CardDialog>().Close(choice);
    }

}
