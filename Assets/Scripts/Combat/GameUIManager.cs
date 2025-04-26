using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class GameUIManager : MonoBehaviour
{
    [Header("Hand Fan Layout Settings")]
    [SerializeField] public float fanAngle = 30f; // Total angle of the fan (degrees)
    [SerializeField] public float fanRadius = 200f; // Radius of the arc (pixels)
    
    [Header("Card Hover Effects")]
    [SerializeField] public float hoverCardScale = 1.2f; // Scale of card when hovered
    [SerializeField] public float hoverCardOffset = 30f; // How much to lift the card when hovered
    [SerializeField] public float hoverTransitionSpeed = 0.1f; // Time to transition to hover state
    public static GameUIManager Instance { get; private set; }

    // Combat UI
    public DeckManager playerDeck;
    public DeckManager enemyDeck;
    public Transform playerHandPanel;
    public GameObject cardUIPrefab;
    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI playerArmorText;
    public TextMeshProUGUI enemyHPText;
    public TextMeshProUGUI enemyArmorText;
    public Button endTurnButton;

    // Future: Inventory, Shop, etc. UI fields can go here

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
        // Ensure we have an EventSystem in the scene
        if (FindObjectOfType<EventSystem>() == null)
        {
            Debug.LogWarning("[GameUIManager] No EventSystem found in scene. Adding one.");
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }
        
        // Ensure the Canvas has a GraphicRaycaster
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null && canvas.GetComponent<GraphicRaycaster>() == null)
        {
            Debug.LogWarning("[GameUIManager] Canvas has no GraphicRaycaster. Adding one.");
            canvas.gameObject.AddComponent<GraphicRaycaster>();
        }
        
        if (endTurnButton != null)
            endTurnButton.onClick.AddListener(OnEndTurnClicked);
            
        UpdateCombatUI();
    }

    public void UpdateCombatUI()
    {
        if (playerHPText != null)
            playerHPText.text = $"HP: {CombatManager.Instance.playerHP}";
        if (playerArmorText != null)
            playerArmorText.text = $"Armor: {CombatManager.Instance.playerArmor}";
        if (enemyHPText != null)
            enemyHPText.text = $"HP: {CombatManager.Instance.enemyHP}";
        if (enemyArmorText != null)
            enemyArmorText.text = $"Armor: {CombatManager.Instance.enemyArmor}";
        // Clear current hand UI
        if (playerHandPanel != null)
        {
            foreach (Transform child in playerHandPanel)
                Destroy(child.gameObject);
            if (playerDeck != null)
            {
                Debug.Log($"[GameUIManager] Player hand count: {playerDeck.hand.Count}");
                int cardIndex = 0;
                foreach (var card in playerDeck.hand)
                {
                    Debug.Log($"[GameUIManager] Instantiating card UI for: {card.data.cardName}");
                    var cardGO = Instantiate(cardUIPrefab, playerHandPanel);
                    
                    // Ensure anchors are centered for proper positioning
                    RectTransform cardRectTransform = cardGO.GetComponent<RectTransform>();
                    cardRectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                    cardRectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                    cardRectTransform.pivot = new Vector2(0.5f, 0.5f);
                    
                    // Ensure card has a raycast target Image component
                    Image cardImage = cardGO.GetComponent<Image>();
                    if (cardImage == null)
                    {
                        cardImage = cardGO.AddComponent<Image>();
                        // Set a transparent image but enable raycast
                        cardImage.color = new Color(1, 1, 1, 0.01f);
                    }
                    cardImage.raycastTarget = true;
                    
                    // Add CardController component for hover effects and click handling
                    CardController cardController = cardGO.GetComponent<CardController>();
                    if (cardController == null)
                    {
                        cardController = cardGO.AddComponent<CardController>();
                        
                        // Set hover effect properties from GameUIManager settings
                        cardController.hoverScaleMultiplier = hoverCardScale;
                        cardController.hoverYOffset = hoverCardOffset;
                        cardController.hoverTransitionSpeed = hoverTransitionSpeed;
                        
                        Debug.Log($"[GameUIManager] Added CardController to {cardGO.name}");
                    }
                    
                    // Link card data to the controller
                    cardController.cardInstance = card;
                    cardController.handIndex = cardIndex++;
                    
                    // Set card playability based on game state
                    bool canPlayCard = CombatManager.Instance.CurrentPhase == CombatPhase.Action &&
                                      CombatManager.Instance.playerTurn;
                    cardController.SetPlayable(canPlayCard);
                    // Set card name
                    var nameObj = cardGO.transform.Find("cardName");
                    if (nameObj != null)
                    {
                        var nameTMP = nameObj.GetComponent<TMPro.TextMeshProUGUI>();
                        if (nameTMP != null)
                            nameTMP.text = card.data.cardName;
                        else
                            Debug.LogWarning("[GameUIManager] cardName child exists but has no TextMeshProUGUI!");
                    }
                    else
                    {
                        Debug.LogWarning("[GameUIManager] cardName child not found on cardUIPrefab instance!");
                    }

                    // Set card description
                    var descObj = cardGO.transform.Find("cardDescription");
                    if (descObj != null)
                    {
                        var descTMP = descObj.GetComponent<TMPro.TextMeshProUGUI>();
                        if (descTMP != null)
                            descTMP.text = card.data.description;
                        else
                            Debug.LogWarning("[GameUIManager] cardDescription child exists but has no TextMeshProUGUI!");
                    }
                    else
                    {
                        Debug.LogWarning("[GameUIManager] cardDescription child not found on cardUIPrefab instance!");
                    }
                    // Optionally add click handler for playing cards
                }
                // Start the delayed fan layout
                StartCoroutine(DelayedFanLayout());
            }
            else
            {
                Debug.LogWarning("[GameUIManager] playerDeck is null in UpdateCombatUI!");
            }
        }
        else
        {
            Debug.LogWarning("[GameUIManager] playerHandPanel is null in UpdateCombatUI!");
        }
    }

    private void OnEndTurnClicked()
    {
        CombatManager.Instance.EndTurn();
        UpdateCombatUI();
    }

    private IEnumerator DelayedFanLayout()
    {
        // Wait for end of frame
        yield return new WaitForEndOfFrame();
        
        // Apply fan layout
        ApplyFanLayout();
    }

    private void ApplyFanLayout()
    {
        if (playerDeck == null || playerHandPanel == null) return;
        
        int cardCount = playerHandPanel.childCount;
        if (cardCount == 0) return;
        
        Debug.Log($"[FanLayout] Applying fan layout to {cardCount} cards");
        
        // Get card size for overlap calculation
        RectTransform firstCard = playerHandPanel.GetChild(0).GetComponent<RectTransform>();
        float cardWidth = firstCard.rect.width;
        
        // Calculate a reasonable card overlap based on count and available space
        float panelWidth = playerHandPanel.GetComponent<RectTransform>().rect.width;
        float spacing = Mathf.Min(cardWidth * 0.7f, panelWidth / (cardCount + 1));
        
        // Adjust radius based on card count to prevent excessive spreading
        float effectiveRadius = Mathf.Min(fanRadius, panelWidth * 0.8f);
        
        // Calculate card spread based on count and effective radius
        // This creates more of a straight line with large radius values
        float spreadFactor = Mathf.Lerp(1.0f, 0.1f, effectiveRadius / 1000f);
        float effectiveAngle = fanAngle * spreadFactor;
        
        // The center position index
        float centerIdx = (cardCount - 1) / 2f;
        
        for (int i = 0; i < cardCount; i++)
        {
            RectTransform cardRect = playerHandPanel.GetChild(i).GetComponent<RectTransform>();
            if (cardRect == null) continue;
            
            // Calculate normalized position (-1 to 1)
            float normalizedPos = cardCount > 1 ? (i - centerIdx) / centerIdx : 0;
            
            // Calculate horizontal spread (more linear with large radius)
            // Negative sign to flip the fan direction along X axis
            float horizontalSpread = -normalizedPos * effectiveRadius;
            
            // Calculate the fan angle - decreases as radius increases
            float angle = normalizedPos * effectiveAngle;
            float rad = angle * Mathf.Deg2Rad;
            
            // Calculate vertical offset - subtle curve regardless of radius
            // Use absolute value of normalized position to create a U-shaped curve
            float verticalFactor = 0.15f; // How pronounced the curve is
            float verticalOffset = -Mathf.Abs(normalizedPos) * effectiveRadius * verticalFactor;
            
            // Set position and rotation
            Vector2 newPos = new Vector2(horizontalSpread, verticalOffset);
            Quaternion newRot = Quaternion.Euler(0, 0, angle);
            
            Debug.Log($"[FanLayout] Card {i}: {cardRect.name} pos {newPos} rot {angle}");
            
            // Apply position and rotation
            cardRect.anchoredPosition = newPos;
            cardRect.localRotation = newRot;
            
            // Force refresh layout
            LayoutRebuilder.ForceRebuildLayoutImmediate(cardRect);
        }
    }


    
    // Future: Add methods for other UI panels
}
