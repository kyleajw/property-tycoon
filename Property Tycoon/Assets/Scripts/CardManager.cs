using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    
    CardData[] opportunityKnocksCards;
    CardData[] potLuckCards;

    Stack<CardData> shuffledPotLuckCards;
    Stack<CardData> shuffledOpportunityKnocksCards;

    private void Start()
    {
        opportunityKnocksCards = GameObject.Find("GameDataManager").GetComponent<OpportunityKnocksCardCollection>().cards;
        potLuckCards = GameObject.Find("GameDataManager").GetComponent<PotLuckCardCollection>().cards;
        shuffledOpportunityKnocksCards = Shuffle(opportunityKnocksCards);
        shuffledPotLuckCards = Shuffle(potLuckCards);
    }
    /// <summary>
    /// Modern version of the Fisher-Yates shuffle algorithm. Swaps cards in the original unsorted array, then pushes them to a stack.
    /// </summary>
    /// <param name="cards">Array of cards loaded in from the JSON file</param>
    /// <returns>Shuffled cards, as a stack</returns>
    private Stack<CardData> Shuffle(CardData[] cards)
    {
        for (int i = 0; i < cards.Length - 2; i++)
        {
            int j = Random.Range(i, cards.Length);
            CardData tmp = cards[i];
            cards[i] = cards[j];
            cards[j] = tmp;
        }
        Stack<CardData> shuffledCards = new Stack<CardData>();
        for (int i = 0; i < cards.Length; i++)
        {
            shuffledCards.Push(cards[i]);
        }
        return shuffledCards;
    }
    /// <summary>
    /// Removes the pot luck card on the top of the stack and returns it. If the stack is empty, regenerate the stack.
    /// </summary>
    /// <returns>Pot luck card</returns>
    public CardData DrawPotLuckCard()
    {
        CardData card = shuffledPotLuckCards.Pop();
        if(shuffledPotLuckCards.Count == 0)
        {
            shuffledPotLuckCards = Shuffle(potLuckCards);
        }

        return card;
    }
    /// <summary>
    /// Removes the Opportunity Knocks card on the top of the stack and returns it. If the stack is empty, regenerate the stack.
    /// </summary>
    /// <returns>Opportunity knocks card</returns>
    public CardData DrawOpportunityKnocksCard()
    {
        CardData card= shuffledOpportunityKnocksCards.Pop();
        if(shuffledOpportunityKnocksCards.Count == 0)
        {
            shuffledOpportunityKnocksCards = Shuffle(opportunityKnocksCards);
        }
        return card;
    }
}
