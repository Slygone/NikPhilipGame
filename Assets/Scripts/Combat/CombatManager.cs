using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public enum CombatPhase { Draw, Action, End }

public class CombatManager : MonoBehaviour
{
    public static CombatManager Instance { get; private set; }

    public CombatPhase CurrentPhase { get; private set; }
    public DeckManager playerDeck;
    public DeckManager enemyDeck;
    
    [Header("Combat Stats")]
    public int playerHP = 30;
    public int playerArmor = 0;
    public int enemyHP = 30;
    public int enemyArmor = 0;
    
    [Header("Turn Management")]
    public int turn = 1;
    public bool playerTurn = true;
    public int actionsPerTurn = 1;
    public int actionsRemaining = 1;
    
    [Header("AI Settings")]
    public float enemyThinkTime = 1.5f;
    
    // Events
    public UnityEvent onCombatStart;
    public UnityEvent onTurnStart;
    public UnityEvent onTurnEnd;
    public UnityEvent<CardInstance> onCardPlayed;
    
    // Internal state
    private bool combatActive = false;
    private bool resolvingEffects = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Debug.Log(playerDeck.deckList.Count);
        StartCombat();
    }

    public void StartCombat()
    {
        Debug.Log("[CombatManager] Starting new combat");
        
        // Set initial state
        playerTurn = true;
        turn = 1;
        actionsRemaining = actionsPerTurn;
        CurrentPhase = CombatPhase.Draw;
        combatActive = true;
        
        // Initialize decks
        playerDeck.StartNewCombat();
        enemyDeck.StartNewCombat();
        
        // Trigger event
        onCombatStart?.Invoke();
        
        // Start first turn
        NextPhase();
    }

    public void NextPhase()
    {
        if (!combatActive) return;
        
        switch (CurrentPhase)
        {
            case CombatPhase.Draw:
                Debug.Log($"[CombatManager] {(playerTurn ? "Player" : "Enemy")} Draw Phase");
                
                // Draw cards based on whose turn it is
                if (playerTurn)
                {
                    playerDeck.DrawCard();
                }
                else
                {
                    enemyDeck.DrawCard();
                }
                
                // Move to action phase
                CurrentPhase = CombatPhase.Action;
                
                // Reset actions for the turn
                actionsRemaining = actionsPerTurn;
                
                // Update UI
                GameUIManager.Instance?.UpdateCombatUI();
                
                // If it's the enemy's turn, start AI
                if (!playerTurn)
                {
                    StartCoroutine(PlayEnemyTurn());
                }
                break;
                
            case CombatPhase.Action:
                // This phase is mainly handled by player input or the enemy AI
                // Only triggered when manually ending the turn or out of actions
                CurrentPhase = CombatPhase.End;
                NextPhase();
                break;
                
            case CombatPhase.End:
                Debug.Log($"[CombatManager] {(playerTurn ? "Player" : "Enemy")} End Phase");
                
                // Trigger turn end event
                onTurnEnd?.Invoke();
                
                // Switch turns
                playerTurn = !playerTurn;
                
                if (playerTurn)
                    turn++; // Increment turn counter when returning to player
                
                // Trigger turn start event
                onTurnStart?.Invoke();
                
                // Move back to draw phase
                CurrentPhase = CombatPhase.Draw;
                NextPhase();
                break;
        }
    }

    public void EndTurn()
    {
        Debug.Log($"[CombatManager] {(playerTurn ? "Player" : "Enemy")} ending turn");
        CurrentPhase = CombatPhase.End;
        NextPhase();
    }

    // Called when a card is played from any deck
    public void OnCardPlayed(CardInstance card, DeckManager sourceDeck)
    {
        if (!combatActive || card == null) return;
        
        Debug.Log($"[CombatManager] Card played: {card.data.cardName}");
        
        // Decrease available actions if it's the player's turn
        if (sourceDeck == playerDeck && playerTurn)
        {
            actionsRemaining--;
            Debug.Log($"[CombatManager] Player used an action. Actions remaining: {actionsRemaining}");
            
            // Auto-end turn if out of actions
            if (actionsRemaining <= 0 && CurrentPhase == CombatPhase.Action)
            {
                Debug.Log("[CombatManager] Player out of actions. Ending turn.");
                EndTurn();
            }
        }
        
        // Trigger card played event
        onCardPlayed?.Invoke(card);
    }
    
    // Enemy AI turn logic
    private IEnumerator PlayEnemyTurn()
    {
        Debug.Log("[CombatManager] Enemy AI taking turn");
        
        // Wait a bit for "thinking"
        yield return new WaitForSeconds(enemyThinkTime);
        
        // Simple AI: Keep playing cards until out of actions or cards
        while (actionsRemaining > 0 && enemyDeck.hand.Count > 0 && CurrentPhase == CombatPhase.Action)
        {
            // Choose a card to play (simple: pick the first one)
            CardInstance cardToPlay = ChooseEnemyCard();
            
            if (cardToPlay != null)
            {
                Debug.Log($"[CombatManager] Enemy playing {cardToPlay.data.cardName}");
                
                // Play the card
                enemyDeck.PlayCard(cardToPlay);
                
                // Resolve effects manually since we're not using CardController
                CardEffectResolver.Resolve(cardToPlay, enemyDeck, playerDeck);
                
                // Decrease actions
                actionsRemaining--;
                
                // Wait between actions
                yield return new WaitForSeconds(enemyThinkTime);
            }
            else
            {
                break; // No playable cards
            }
        }
        
        // End turn when done
        EndTurn();
    }
    
    // Enemy AI card selection
    private CardInstance ChooseEnemyCard()
    {
        if (enemyDeck.hand.Count == 0) return null;
        
        // Very basic AI for now: just pick the first card
        // This could be expanded with more sophisticated logic
        return enemyDeck.hand[0];
    }
    
    // Check for victory or defeat conditions
    public void CheckGameEndConditions()
    {
        if (playerHP <= 0)
        {
            Debug.Log("[CombatManager] Player has been defeated!");
            GameOver(false);
        }
        else if (enemyHP <= 0)
        {
            Debug.Log("[CombatManager] Enemy has been defeated!");
            GameOver(true);
        }
    }
    
    // Handle game over
    private void GameOver(bool playerVictory)
    {
        combatActive = false;
        Debug.Log($"[CombatManager] Game Over! Player {(playerVictory ? "wins" : "loses")}!");
        
        // TODO: Handle victory/defeat UI and transitions
    }
}
