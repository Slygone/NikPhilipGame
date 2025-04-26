#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public static class EnemyScriptableObjectGenerator
{
    [MenuItem("DungeonCardMayhem/Generate Example Enemy ScriptableObjects")]
    public static void GenerateEnemies()
    {
        string dataPath = "Assets/Data/Enemies";
        if (!AssetDatabase.IsValidFolder(dataPath))
        {
            AssetDatabase.CreateFolder("Assets/Data", "Enemies");
        }
        // Example CardData asset references (assumes you have created these already)
        CardData attackCard = AssetDatabase.LoadAssetAtPath<CardData>("Assets/Data/Cards/AttackCard.asset");
        CardData armorCard = AssetDatabase.LoadAssetAtPath<CardData>("Assets/Data/Cards/ArmorCard.asset");
        CardData berserkSmashCard = AssetDatabase.LoadAssetAtPath<CardData>("Assets/Data/Cards/BerserkSmashCard.asset");
        CardData shieldBlockCard = AssetDatabase.LoadAssetAtPath<CardData>("Assets/Data/Cards/ShieldBlockCard.asset");
        CardData rallyCard = AssetDatabase.LoadAssetAtPath<CardData>("Assets/Data/Cards/RallyCard.asset");
        HeroPowerData orcBossPower = AssetDatabase.LoadAssetAtPath<HeroPowerData>("Assets/Data/HeroPowers/OrcBossPower.asset");
        HeroPowerData soldierCaptainPower = AssetDatabase.LoadAssetAtPath<HeroPowerData>("Assets/Data/HeroPowers/SoldierCaptainPower.asset");

        // Assign unique sprites for each enemy
        Sprite orcGruntSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Tiny RPG Character Asset Pack v1.03b -Free Soldier&Orc/Tiny RPG Character Asset Pack v1.03 -Free Soldier&Orc/Characters(100x100)/Orc/Orc/Orc-Idle.png");
        Sprite orcBossSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Tiny RPG Character Asset Pack v1.03b -Free Soldier&Orc/Tiny RPG Character Asset Pack v1.03 -Free Soldier&Orc/Characters(100x100)/Orc/Orc/Orc-Attack01.png");
        Sprite soldierGuardSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Tiny RPG Character Asset Pack v1.03b -Free Soldier&Orc/Tiny RPG Character Asset Pack v1.03 -Free Soldier&Orc/Characters(100x100)/Soldier/Soldier/Soldier-Idle.png");
        Sprite soldierCaptainSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/Tiny RPG Character Asset Pack v1.03b -Free Soldier&Orc/Tiny RPG Character Asset Pack v1.03 -Free Soldier&Orc/Characters(100x100)/Soldier/Soldier/Soldier-Attack03.png");

        // Orc Grunt
        EnemyData orcGrunt = ScriptableObject.CreateInstance<EnemyData>();
        orcGrunt.enemyName = "Orc Grunt";
        orcGrunt.maxHP = 12;
        orcGrunt.art = orcGruntSprite;
        orcGrunt.deck = new System.Collections.Generic.List<CardData> { attackCard, attackCard, armorCard };
        orcGrunt.heroPowers = new System.Collections.Generic.List<HeroPowerData>();
        AssetDatabase.CreateAsset(orcGrunt, Path.Combine(dataPath, "OrcGrunt.asset"));

        // Orc Boss
        EnemyData orcBoss = ScriptableObject.CreateInstance<EnemyData>();
        orcBoss.enemyName = "Orc Boss";
        orcBoss.maxHP = 30;
        orcBoss.art = orcBossSprite;
        orcBoss.deck = new System.Collections.Generic.List<CardData> { attackCard, berserkSmashCard, armorCard };
        orcBoss.heroPowers = new System.Collections.Generic.List<HeroPowerData> { orcBossPower };
        AssetDatabase.CreateAsset(orcBoss, Path.Combine(dataPath, "OrcBoss.asset"));

        // Soldier Guard
        EnemyData soldierGuard = ScriptableObject.CreateInstance<EnemyData>();
        soldierGuard.enemyName = "Soldier Guard";
        soldierGuard.maxHP = 14;
        soldierGuard.art = soldierGuardSprite;
        soldierGuard.deck = new System.Collections.Generic.List<CardData> { attackCard, armorCard, shieldBlockCard };
        soldierGuard.heroPowers = new System.Collections.Generic.List<HeroPowerData>();
        AssetDatabase.CreateAsset(soldierGuard, Path.Combine(dataPath, "SoldierGuard.asset"));

        // Soldier Captain
        EnemyData soldierCaptain = ScriptableObject.CreateInstance<EnemyData>();
        soldierCaptain.enemyName = "Soldier Captain";
        soldierCaptain.maxHP = 22;
        soldierCaptain.art = soldierCaptainSprite;
        soldierCaptain.deck = new System.Collections.Generic.List<CardData> { attackCard, rallyCard, armorCard };
        soldierCaptain.heroPowers = new System.Collections.Generic.List<HeroPowerData> { soldierCaptainPower };
        AssetDatabase.CreateAsset(soldierCaptain, Path.Combine(dataPath, "SoldierCaptain.asset"));

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Example enemy ScriptableObjects generated with unique art!");
    }
}
#endif
