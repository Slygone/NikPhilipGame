using UnityEngine;

[CreateAssetMenu(fileName = "TrinketData", menuName = "DungeonCardMayhem/Trinket", order = 4)]
public class TrinketData : ScriptableObject
{
    public string trinketName;
    public string description;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "EnchantmentData", menuName = "DungeonCardMayhem/Enchantment", order = 5)]
public class EnchantmentData : ScriptableObject
{
    public string enchantmentName;
    public string description;
    public Sprite icon;
}

[CreateAssetMenu(fileName = "MetaUpgradeData", menuName = "DungeonCardMayhem/MetaUpgrade", order = 6)]
public class MetaUpgradeData : ScriptableObject
{
    public string upgradeName;
    public string description;
    public Sprite icon;
}
