using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "EnemyData", menuName = "DungeonCardMayhem/Enemy", order = 3)]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHP;
    public Sprite art;
    public List<CardData> deck;
    public List<HeroPowerData> heroPowers;
}
