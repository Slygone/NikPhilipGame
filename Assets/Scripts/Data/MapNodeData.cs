using UnityEngine;

public enum MapNodeType { NormalCombat, Camp, EliteCombat, Boss }

[CreateAssetMenu(fileName = "MapNodeData", menuName = "DungeonCardMayhem/MapNode", order = 7)]
public class MapNodeData : ScriptableObject
{
    public MapNodeType nodeType;
    public string description;
    public Sprite icon;
    public RewardData[] possibleRewards;
}

[System.Serializable]
public class RewardData
{
    public string rewardName;
    public Sprite icon;
}
