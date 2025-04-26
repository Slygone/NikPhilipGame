using UnityEngine;

public enum CardType { Damage, Armor, Heal, Draw, ExtraAction, HeroPower }

[CreateAssetMenu(fileName = "CardData", menuName = "DungeonCardMayhem/Card", order = 2)]
public class CardData : ScriptableObject
{
    public string cardName;
    public CardType cardType;
    public int value;
    public Sprite art;
    public Rarity rarity;
    public string description;
}

public enum Rarity { Common, Uncommon, Rare, Epic, Legendary }
