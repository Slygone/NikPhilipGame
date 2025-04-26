using UnityEngine;

public static class CardEffectResolver
{
    public static void Resolve(CardInstance card, DeckManager source, DeckManager target)
    {
        if (card == null || card.data == null) return;
        
        Debug.Log($"[CardEffectResolver] Resolving card effect: {card.data.cardName}, Type: {card.data.cardType}, Value: {card.data.value}");
        
        switch (card.data.cardType)
        {
            case CardType.Damage:
                DealDamage(target, card.data.value);
                break;
            case CardType.Armor:
                AddArmor(source, card.data.value);
                break;
            case CardType.Heal:
                Heal(source, card.data.value);
                break;
            case CardType.Draw:
                for (int i = 0; i < card.data.value; i++)
                    source.DrawCard();
                break;
            case CardType.ExtraAction:
                GainExtraAction(card.data.value);
                break;
            case CardType.HeroPower:
                ApplyHeroPower(source, target, card.data.value);
                break;
            default:
                Debug.LogWarning($"[CardEffectResolver] Unknown card type: {card.data.cardType}");
                break;
        }
        
        // Update the UI after applying effects
        GameUIManager.Instance?.UpdateCombatUI();
    }

    static void DealDamage(DeckManager target, int amount)
    {
        CombatManager combatManager = CombatManager.Instance;
        if (combatManager == null) return;
        
        Debug.Log($"[CardEffectResolver] Dealing {amount} damage");
        
        // Determine if target is player or enemy based on the DeckManager reference
        bool targetIsPlayer = target == combatManager.playerDeck;
        
        // Apply damage to armor first, then to health
        if (targetIsPlayer)
        {
            int remainingDamage = amount;
            // Armor absorbs damage first
            if (combatManager.playerArmor > 0)
            {
                int armorDamage = Mathf.Min(remainingDamage, combatManager.playerArmor);
                combatManager.playerArmor -= armorDamage;
                remainingDamage -= armorDamage;
                Debug.Log($"[CardEffectResolver] Player armor absorbed {armorDamage} damage. Armor remaining: {combatManager.playerArmor}");
            }
            
            // Apply remaining damage to health
            if (remainingDamage > 0)
            {
                combatManager.playerHP -= remainingDamage;
                Debug.Log($"[CardEffectResolver] Player took {remainingDamage} damage. HP remaining: {combatManager.playerHP}");
                
                // Check for game over
                if (combatManager.playerHP <= 0)
                {
                    Debug.Log("[CardEffectResolver] Player has been defeated!");
                    // TODO: Handle player defeat
                }
            }
        }
        else
        {
            int remainingDamage = amount;
            // Armor absorbs damage first
            if (combatManager.enemyArmor > 0)
            {
                int armorDamage = Mathf.Min(remainingDamage, combatManager.enemyArmor);
                combatManager.enemyArmor -= armorDamage;
                remainingDamage -= armorDamage;
                Debug.Log($"[CardEffectResolver] Enemy armor absorbed {armorDamage} damage. Armor remaining: {combatManager.enemyArmor}");
            }
            
            // Apply remaining damage to health
            if (remainingDamage > 0)
            {
                combatManager.enemyHP -= remainingDamage;
                Debug.Log($"[CardEffectResolver] Enemy took {remainingDamage} damage. HP remaining: {combatManager.enemyHP}");
                
                // Check for victory
                if (combatManager.enemyHP <= 0)
                {
                    Debug.Log("[CardEffectResolver] Enemy has been defeated!");
                    // TODO: Handle enemy defeat
                }
            }
        }
    }

    static void AddArmor(DeckManager target, int amount)
    {
        CombatManager combatManager = CombatManager.Instance;
        if (combatManager == null) return;
        
        Debug.Log($"[CardEffectResolver] Adding {amount} armor");
        
        // Determine if target is player or enemy based on the DeckManager reference
        bool targetIsPlayer = target == combatManager.playerDeck;
        
        if (targetIsPlayer)
        {
            combatManager.playerArmor += amount;
            Debug.Log($"[CardEffectResolver] Player gained {amount} armor. Total armor: {combatManager.playerArmor}");
        }
        else
        {
            combatManager.enemyArmor += amount;
            Debug.Log($"[CardEffectResolver] Enemy gained {amount} armor. Total armor: {combatManager.enemyArmor}");
        }
    }

    static void Heal(DeckManager target, int amount)
    {
        CombatManager combatManager = CombatManager.Instance;
        if (combatManager == null) return;
        
        Debug.Log($"[CardEffectResolver] Healing for {amount}");
        
        // Determine if target is player or enemy based on the DeckManager reference
        bool targetIsPlayer = target == combatManager.playerDeck;
        
        if (targetIsPlayer)
        {
            // TODO: Get max player HP from somewhere
            int maxPlayerHP = 30; // Placeholder value
            int actualHeal = Mathf.Min(amount, maxPlayerHP - combatManager.playerHP);
            combatManager.playerHP += actualHeal;
            Debug.Log($"[CardEffectResolver] Player healed for {actualHeal}. Total HP: {combatManager.playerHP}");
        }
        else
        {
            // TODO: Get max enemy HP from somewhere
            int maxEnemyHP = 30; // Placeholder value
            int actualHeal = Mathf.Min(amount, maxEnemyHP - combatManager.enemyHP);
            combatManager.enemyHP += actualHeal;
            Debug.Log($"[CardEffectResolver] Enemy healed for {actualHeal}. Total HP: {combatManager.enemyHP}");
        }
    }
    
    static void GainExtraAction(int amount)
    {
        CombatManager combatManager = CombatManager.Instance;
        if (combatManager == null) return;
        
        Debug.Log($"[CardEffectResolver] Gaining {amount} extra action(s)");
        
        // This could be implemented by giving the player more cards to play this turn
        // or by extending their turn in some other way
        // For now, we'll just draw more cards as an example
        for (int i = 0; i < amount; i++)
        {
            if (combatManager.playerTurn)
                combatManager.playerDeck.DrawCard();
            else
                combatManager.enemyDeck.DrawCard();
        }
    }
    
    static void ApplyHeroPower(DeckManager source, DeckManager target, int value)
    {
        CombatManager combatManager = CombatManager.Instance;
        if (combatManager == null) return;
        
        Debug.Log($"[CardEffectResolver] Applying hero power with value {value}");
        
        // Hero power effects could vary widely
        // For this implementation, we'll treat it as a stronger damage card that also adds armor
        DealDamage(target, value);
        AddArmor(source, value / 2);  // Gives armor equal to half the damage value
    }
}
