#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public static class CardScriptableObjectGenerator
{
    [MenuItem("DungeonCardMayhem/Generate Example Card ScriptableObjects")]
    public static void GenerateCards()
    {
        string dataPath = "Assets/Data/Cards";
        if (!AssetDatabase.IsValidFolder(dataPath))
        {
            AssetDatabase.CreateFolder("Assets/Data", "Cards");
        }

        // CardType enum: Damage, Armor, Heal, Draw, ExtraAction, HeroPower
        var cardTypes = new[]{
            CardType.Damage, CardType.Armor, CardType.Heal, CardType.Draw, CardType.ExtraAction, CardType.HeroPower
        };
        var rarities = new[]{ Rarity.Common, Rarity.Uncommon, Rarity.Rare, Rarity.Epic, Rarity.Legendary };
        System.Random rng = new System.Random();

        foreach (var type in cardTypes)
        {
            for (int i = 0; i < 5; i++)
            {
                CardData card = ScriptableObject.CreateInstance<CardData>();
                card.cardType = type;
                card.rarity = rarities[i % rarities.Length];
                card.cardName = $"{type} Card {i+1}";
                card.description = $"This is a {type} card. Variation {i+1}.";
                // Value variance per type
                switch(type)
                {
                    case CardType.Damage:
                        card.value = rng.Next(3, 12) + i; // 3-16
                        break;
                    case CardType.Armor:
                        card.value = rng.Next(2, 8) + i; // 2-12
                        break;
                    case CardType.Heal:
                        card.value = rng.Next(2, 10) + i; // 2-14
                        break;
                    case CardType.Draw:
                        card.value = rng.Next(1, 4) + (i/2); // 1-5
                        break;
                    case CardType.ExtraAction:
                        card.value = 1 + (i%2); // 1 or 2
                        break;
                    case CardType.HeroPower:
                        card.value = rng.Next(5, 15) + i; // 5-19
                        break;
                }
                card.art = null; // No image for now
                string assetName = $"{type}_Card_{i+1}.asset";
                AssetDatabase.CreateAsset(card, Path.Combine(dataPath, assetName));
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Example card ScriptableObjects generated!");
    }
}
#endif
