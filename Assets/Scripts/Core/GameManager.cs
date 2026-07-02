using System;
using UnityEngine;

namespace Alchemia.Core
{
    public enum GameState
    {
        Menu,
        Playing
    }
    
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public static event Action<GameState> OnStateChanged;

        public GameState CurrentState { get; private set; } = GameState.Menu;

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

        private void Start()
        {
            SetState(GameState.Menu);
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;

            CurrentState = newState;
            OnStateChanged?.Invoke(newState);
        }

        public void StartGame()
        {
            SetState(GameState.Playing);
        }

        public void ReturnToMenu()
        {
            SetState(GameState.Menu);
        }
    }
}
