using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private Room currentRoom;
    private Room previousRoom;
    private PlayerDetailsSO playerDetails;
    private Player player;

    [HideInInspector] public GameState gameState;
    [HideInInspector] public GameState previousGameState;
    private long gameScore;
    private int scoreMultiplier;
    private InstantiatedRoom bossRoom;

    protected override void Awake()
    {
        base.Awake();

        // Set player details - saved in current player so from the main menu
        playerDetails = GameResources.Instance.currentPlayer.playerDetails;

        InstantiatePlayer();
    }

    private void OnEnable()
    {
        StaticEventHandler.OnRoomChanged += StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplier += StaticEventHandler_OnMultiplier;

        StaticEventHandler.OnRoomEnemiesDefeated += StaticEventHandler_OnRoomEnemiesDefeated;

        player.destroyedEvent.OnDestroyed += Player_OnDestroyed;
    }

    private void OnDisable()
    {
        StaticEventHandler.OnRoomChanged -= StaticEventHandler_OnRoomChanged;

        StaticEventHandler.OnPointsScored += StaticEventHandler_OnPointsScored;

        StaticEventHandler.OnMultiplier -= StaticEventHandler_OnMultiplier;

        StaticEventHandler.OnRoomEnemiesDefeated -= StaticEventHandler_OnRoomEnemiesDefeated;

        player.destroyedEvent.OnDestroyed -= Player_OnDestroyed;
    }

    private void StaticEventHandler_OnRoomEnemiesDefeated(RoomEnemiesDefeatedArgs roomEnemiesDefeatedArgs)
    {
        RoomEnemiesDefeated();
    }

    private void Player_OnDestroyed(DestroyedEvent destroyedEvent, DestroyedEventArgs destroyedEventArgs)
    {
        previousGameState = gameState;
        gameState = GameState.gameLost;
    }

    private void StaticEventHandler_OnMultiplier(MultiplierArgs multiplierArgs)
    {
        if (multiplierArgs.multiplier)
        {
            scoreMultiplier++;
        }
        else
        {
            scoreMultiplier--;
        }

        scoreMultiplier = Mathf.Clamp(scoreMultiplier, 1, 30);

        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    private void StaticEventHandler_OnRoomChanged(RoomChangedEventArgs roomChangedEventArgs)
    {
        SetCurrentRoom(roomChangedEventArgs.room);
    }

    private void StaticEventHandler_OnPointsScored(PointsScoredArgs pointsScoredArgs)
    {
        gameScore += pointsScoredArgs.points * scoreMultiplier;

        StaticEventHandler.CallScoreChangedEvent(gameScore, scoreMultiplier);
    }

    /// <summary>
    /// Create player in scene at position
    /// </summary>
    private void InstantiatePlayer()
    {
        GameObject playerGameObject = Instantiate(playerDetails.playerPrefab);

        player = playerGameObject.GetComponent<Player>();

        player.Initialize(playerDetails);
    }

    private void Start()
    {
        previousGameState = GameState.gameStarted;
        gameState = GameState.gameStarted;

        gameScore = 0;

        scoreMultiplier = 1;
    }

    private void Update()
    {
        HandleGameState();

        // for testing
        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    gameState = GameState.gameStarted;
        //}
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

                // in case we have a level with only a boss room
                RoomEnemiesDefeated();

                break;

            case GameState.levelCompleted:

                StartCoroutine(LevelCompleted());

                break;

            case GameState.gameWon:

                // ensures that this is triggered only once
                if (previousGameState != GameState.gameWon)
                    StartCoroutine(GameWon());

                break;

            case GameState.gameLost:

                if (previousGameState != GameState.gameLost)
                {
                    StopAllCoroutines();// Prevent messages if you clear the level just as you die
                    StartCoroutine(GameLost());
                }

                break;

            case GameState.restartGame:

                RestartGame();

                break;
        }
    }

    /// <summary>
    /// Set the current room the player is in
    /// </summary>
    /// <param name="room"></param>
    public void SetCurrentRoom(Room room)
    {
        previousRoom = currentRoom;
        currentRoom = room;
    }

    private void RoomEnemiesDefeated()
    {
        bool isDungeonClearOfRegularEnemies = true;
        bossRoom = null;

        foreach (KeyValuePair<string, Room> keyValuePair in DungeonBuilder.Instance.dungeonBuilderRoomDictionary)
        {
            if (keyValuePair.Value.roomNodeType.isBossRoom)
            {
                bossRoom = keyValuePair.Value.instantiatedRoom;
                continue;
            }

            if (!keyValuePair.Value.isClearedOfEnemies)
            {
                isDungeonClearOfRegularEnemies = false;
                break;
            }
        }

        if ((isDungeonClearOfRegularEnemies && bossRoom == null) || (isDungeonClearOfRegularEnemies && bossRoom.room.isClearedOfEnemies))
        {
            if (currentDungeonLevelListIndex < dungeonLevelList.Count - 1)
            {
                gameState = GameState.levelCompleted;
            }
            else
            {
                gameState = GameState.gameWon;
            }
        }
        else if (isDungeonClearOfRegularEnemies)
        {
            gameState = GameState.bossStage;

            StartCoroutine(BossStage());
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

        StaticEventHandler.CallRoomChangedEvent(currentRoom);

        // set player roughly mid room
        player.gameObject.transform.position = new Vector3((currentRoom.lowerBounds.x + currentRoom.upperBounds.x) / 2f, (currentRoom.lowerBounds.y + currentRoom.upperBounds.y) / 2f, 0f);

        // get nearest spawn point in room nearest player
        player.gameObject.transform.position = HelperUtilities.GetSpawnPositionNearestToPlayer(player.gameObject.transform.position);
    }

    private IEnumerator BossStage()
    {
        // In case room was deactivated when not in minimap view
        bossRoom.gameObject.SetActive(true);

        bossRoom.UnlockDoors(0f);

        yield return new WaitForSeconds(2f);

        Debug.Log("Boss stage - find and destroy the boss");
    }

    private IEnumerator LevelCompleted()
    {
        gameState = GameState.levelCompleted;

        yield return new WaitForSeconds(2f);

        Debug.Log("Level completed. Press return to progress to the next level");

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null; // avoids enter being detected twice

        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;

        Debug.Log("Game won.  Game will restart in 10 seconds");

        yield return new WaitForSeconds(10f);

        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        Debug.Log("Game lost.  Game will restart in 10 seconds");

        yield return new WaitForSeconds(10f);

        gameState = GameState.restartGame;
    }

    private void RestartGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    /// <summary>
    /// Get the player
    /// </summary>
    /// <returns></returns>
    public Player GetPlayer()
    {
        return player;
    }

    /// <summary>
    /// returns the player minimap icon sprite
    /// </summary>
    /// <returns></returns>
    public Sprite GetPlayerMiniMapIcon()
    {
        return playerDetails.playerMiniMapIcon;
    }

    /// <summary>
    /// Get the curret room the player is in
    /// </summary>
    /// <returns></returns>
    public Room GetCurrentRoom()
    {
        return currentRoom;
    }

    /// <summary>
    /// Get the current dungeon level
    /// </summary>
    /// <returns></returns>
    public DungeonLevelSO GetCurrentDungeonLevel()
    {
        return dungeonLevelList[currentDungeonLevelListIndex];
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