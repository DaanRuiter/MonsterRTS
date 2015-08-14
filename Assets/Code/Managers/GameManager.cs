using UnityEngine;
using System.Collections;

public enum GameState
{
    None,
    MainMenu,
    Loading,
    Paused,
    Game
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public SaveGame saveGame = null;
    public GameState state;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    public void LoadGame()
    {
        GameObject.Instantiate(Resources.Load("UI/Loading Screen"));
    }
}