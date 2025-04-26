using UnityEngine;
using System.Collections.Generic;

public class DeckManager : MonoBehaviour
{
    public List<CardData> deckList;
    public List<CardInstance> deck = new List<CardInstance>();
    public List<CardInstance> hand = new List<CardInstance>();
    public List<CardInstance> discard = new List<CardInstance>();
    public int maxHandSize = 5;

    public void StartNewCombat()
    {
        Debug.Log("Starting new combat");
        deck.Clear();
        hand.Clear();
        discard.Clear();
        foreach (var card in deckList)
            deck.Add(new CardInstance(card));
        ShuffleDeck();
        DrawStartingHand();
    }

    private void DrawStartingHand()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        if (hand.Count >= maxHandSize)
            return;
        if (deck.Count == 0)
            ReshuffleDiscardIntoDeck();
        if (deck.Count == 0)
            return;
        var card = deck[0];
        deck.RemoveAt(0);
        hand.Add(card);
        // Update UI after drawing
        if (GameUIManager.Instance != null)
            GameUIManager.Instance.UpdateCombatUI();
    }

    public void PlayCard(CardInstance card)
    {
        // Can't play null cards
        if (card == null) return;
        
        Debug.Log($"[DeckManager] Playing card: {card.data.cardName}");
        
        // Remove card from hand
        if (hand.Contains(card))
        {
            hand.Remove(card);
            discard.Add(card);
            
            // Notify the game manager that a card was played
            if (CombatManager.Instance != null)
            {
                CombatManager.Instance.OnCardPlayed(card, this);
            }
            
            // Update the UI
            GameUIManager.Instance?.UpdateCombatUI();
        }
        else
        {
            Debug.LogWarning($"[DeckManager] Attempted to play card {card.data.cardName} that is not in hand!");
        }
    }

    public void ReshuffleDiscardIntoDeck()
    {
        deck.AddRange(discard);
        discard.Clear();
        ShuffleDeck();
    }

    public void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            var temp = deck[i];
            int rand = Random.Range(i, deck.Count);
            deck[i] = deck[rand];
            deck[rand] = temp;
        }
    }
}
