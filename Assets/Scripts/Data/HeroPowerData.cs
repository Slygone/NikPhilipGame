using UnityEngine;

[CreateAssetMenu(fileName = "HeroPowerData", menuName = "DungeonCardMayhem/HeroPower", order = 8)]
public class HeroPowerData : ScriptableObject
{
    public string powerName;
    public string description;
    public Sprite icon;
    // Add fields for effects, cost, etc. as needed
}
