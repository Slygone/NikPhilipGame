using UnityEngine;

public enum GameState
{
    MainMenu,
    CharacterSelect,
    Map,
    Combat,
    Camp,
    GameOver,
    MetaProgression
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        // Add state transition logic here
        Debug.Log($"Game state changed to: {newState}");
    }
}
