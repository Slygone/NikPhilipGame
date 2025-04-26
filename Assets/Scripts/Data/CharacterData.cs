using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "CharacterData", menuName = "DungeonCardMayhem/Character", order = 1)]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public int baseHP;
    public Sprite portrait;
    public List<CardData> starterDeck;
    public List<HeroPowerData> heroPowers;
}
