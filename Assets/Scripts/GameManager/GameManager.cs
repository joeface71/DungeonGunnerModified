using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header DUNGEON LEVELS

    [Space(10)]
    [Header("DUNGEON LEVELS")]

    #endregion Header DUNGEON LEVELS

    #region Tooltip

    [Tooltip("Populate with the dungeon level scriptable objects")]

    #endregion Tooltip

    [SerializeField] private List<DungeonLevelSO> dungeonLevelList;

    #region Tooltip

    [Tooltip("Poplulate with the starting dungeon level for testing, first level = 0")]

    #endregion Tooltip

    [SerializeField] private int currentDungeonLevelListIndex = 0;

    [HideInInspector] public GameState gameState;

    private void Start()
    {
        gameState = GameState.gameStarted;
    }

    private void Update()
    {
        HandleGameState();

        // for testing
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameState = GameState.gameStarted;
        }
    }

    /// <summary>
    /// Handle game state
    /// </summary>
    private void HandleGameState()
    {
        // Handle game state
        switch (gameState)
        {
            case GameState.gameStarted:

                // Play first level
                PlayDungeonLevel(currentDungeonLevelListIndex);

                gameState = GameState.playingLevel;

                break;
        }
    }

    private void PlayDungeonLevel(int dungeonLevelListIndex)
    {
        // build dungeon for level
        bool dungeonBuiltSuccessfully = DungeonBuilder.Instance.GenerateDungeon(dungeonLevelList[dungeonLevelListIndex]);

        if (!dungeonBuiltSuccessfully)
        {
            Debug.LogError("Couldn't build dungeon from specified rooms and node graphs");
        }
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

#endif

    #endregion Validation
}