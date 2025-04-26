using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System;

// Add this component to each card prefab
public class CardController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private int originalSiblingIndex;
    private bool isHovered = false;
    
    // Card data reference
    [HideInInspector] public CardInstance cardInstance;
    [HideInInspector] public int handIndex;
    
    // Event for when card is played
    public event Action<CardController> OnCardPlayed;
    
    // Configurable hover effects
    [Header("Hover Settings")]
    public float hoverScaleMultiplier = 1.2f;
    public float hoverYOffset = 30f;
    public float hoverTransitionSpeed = 0.1f;
    
    // Whether this card can be played right now
    private bool isPlayable = true;
    
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalScale = Vector3.one;
    }
    
    // When this card is enabled (added to hand)
    private void OnEnable()
    {
        // Store original position when layout is finalized
        StartCoroutine(SaveOriginalPositionDelayed());
    }
    
    private IEnumerator SaveOriginalPositionDelayed()
    {
        // Wait for the layout system to finalize positions
        yield return new WaitForEndOfFrame();
        
        originalPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();
        
        Debug.Log($"[CardController] Card {name} original pos: {originalPosition}, index: {originalSiblingIndex}");
    }
    
    // Called when pointer enters this card
    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log($"[CardController] Mouse ENTER on card {name}");
        isHovered = true;
        originalPosition = rectTransform.anchoredPosition; // Update the position
        originalSiblingIndex = transform.GetSiblingIndex();
        
        // Bring to front
        transform.SetAsLastSibling();
        
        // Scale up and move card
        StartCoroutine(AnimateHover(true));
    }
    
    // Called when pointer exits this card
    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log($"[CardController] Mouse EXIT on card {name}");
        isHovered = false;
        
        // Return to original position in the hierarchy
        transform.SetSiblingIndex(originalSiblingIndex);
        
        // Scale down and move card back
        StartCoroutine(AnimateHover(false));
    }
    
    // Called when card is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isPlayable || cardInstance == null) return;
        
        Debug.Log($"[CardController] Card clicked: {cardInstance.data.cardName}");
        
        // Invoke the card played event
        OnCardPlayed?.Invoke(this);
        
        // Play the card through the DeckManager
        if (CombatManager.Instance != null && 
            CombatManager.Instance.CurrentPhase == CombatPhase.Action && 
            CombatManager.Instance.playerTurn)
        {
            PlayCard();
        }
        else
        {
            Debug.Log("[CardController] Cannot play card now - not player's action phase");
        }
    }
    
    // Play this card
    public void PlayCard()
    {
        // Get references needed to play the card
        CombatManager combatManager = CombatManager.Instance;
        if (combatManager == null) return;
        
        DeckManager playerDeck = combatManager.playerDeck;
        DeckManager enemyDeck = combatManager.enemyDeck;
        
        if (playerDeck == null || enemyDeck == null) return;
        
        // Play the card
        playerDeck.PlayCard(cardInstance);
        
        // Resolve card effect
        CardEffectResolver.Resolve(cardInstance, playerDeck, enemyDeck);
        
        // If this was a card from the player deck, update the UI
        GameUIManager.Instance?.UpdateCombatUI();
    }
    
    // Set whether this card is playable
    public void SetPlayable(bool playable)
    {
        isPlayable = playable;
        // You could add visual feedback here (e.g., graying out unplayable cards)
    }
    
    // Animate the hover effect
    private IEnumerator AnimateHover(bool hovering)
    {
        Vector2 targetPos;
        Vector3 targetScale;
        
        if (hovering)
        {
            // Hover position and scale
            targetPos = new Vector2(originalPosition.x, originalPosition.y + hoverYOffset);
            targetScale = originalScale * hoverScaleMultiplier;
        }
        else
        {
            // Return to original state
            targetPos = originalPosition;
            targetScale = originalScale;
        }
        
        float startTime = Time.time;
        float elapsedTime = 0;
        
        while (elapsedTime < hoverTransitionSpeed)
        {
            elapsedTime = Time.time - startTime;
            float t = Mathf.Clamp01(elapsedTime / hoverTransitionSpeed);
            
            rectTransform.anchoredPosition = Vector2.Lerp(rectTransform.anchoredPosition, targetPos, t);
            rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, targetScale, t);
            
            yield return null;
        }
        
        // Ensure we reach the exact target values
        rectTransform.anchoredPosition = targetPos;
        rectTransform.localScale = targetScale;
    }
}
