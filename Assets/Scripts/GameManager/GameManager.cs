using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class GameManager : SingletonMonobehaviour<GameManager>
{
    #region Header GAMEOBJECT REFERENCES
    [Space(10)]
    [Header("Gameobject References")]
    #endregion

    #region Tooltip
    [Tooltip("Populate with the MessageText textmeshpro component in the FadeScreenUI")]
    #endregion
    [SerializeField] private TextMeshProUGUI messageTextTMP;

    #region Tooltip
    [Tooltip("Populate with the FadeImage canvasgroup component in the FadeScreenUI")]
    #endregion
    [SerializeField] private CanvasGroup canvasGroup;

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

        StartCoroutine(Fade(0f, 1f, 0f, Color.black));
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

        StartCoroutine(DisplayDungeonLevelText());

        // test code
        RoomEnemiesDefeated();
    }

    private IEnumerator DisplayDungeonLevelText()
    {
        StartCoroutine(Fade(0f, 1f, 0f, Color.black));

        GetPlayer().playerControl.DisablePlayer();

        string messageText = "LEVEL " + (currentDungeonLevelListIndex + 1).ToString() + "\n\n" + dungeonLevelList[currentDungeonLevelListIndex].levelName.ToUpper();

        yield return StartCoroutine(DisplayMessageRoutine(messageText, Color.white, 2f));

        GetPlayer().playerControl.EnablePlayer();

        yield return StartCoroutine(Fade(1f, 0f, 2f, Color.black));
    }

    private IEnumerator DisplayMessageRoutine(string text, Color textColor, float displaySeconds)
    {
        messageTextTMP.SetText(text);
        messageTextTMP.color = textColor;

        // Display for a given time
        if (displaySeconds > 0f)
        {
            float timer = displaySeconds;

            while (timer > 0f && !Input.GetKeyDown(KeyCode.Return))
            {
                timer -= Time.deltaTime;
                yield return null;
            }
        }
        // else display until return is pressed
        else
        {
            while (!Input.GetKeyDown(KeyCode.Return))
            {
                yield return null;
            }
        }

        yield return null;

        messageTextTMP.SetText("");
    }

    private IEnumerator BossStage()
    {
        // In case room was deactivated when not in minimap view
        bossRoom.gameObject.SetActive(true);

        bossRoom.UnlockDoors(0f);

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("Well done " + GameResources.Instance.currentPlayer.playerName + "! You've survived...so far\n" +
            "\nNow find and defeat the boss", Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));
    }

    private IEnumerator LevelCompleted()
    {
        gameState = GameState.playingLevel;

        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        yield return StartCoroutine(DisplayMessageRoutine("Well done " + GameResources.Instance.currentPlayer.playerName + "! \n\nYou've survived this level", Color.white, 5f));

        yield return StartCoroutine(DisplayMessageRoutine("Collect loot...then press return to descend further.", Color.white, 5f));

        yield return StartCoroutine(Fade(1f, 0f, 2f, new Color(0f, 0f, 0f, 0.4f)));

        while (!Input.GetKeyDown(KeyCode.Return))
        {
            yield return null;
        }

        yield return null; // avoids enter being detected twice

        currentDungeonLevelListIndex++;

        PlayDungeonLevel(currentDungeonLevelListIndex);
    }

    private IEnumerator Fade(float startFadeAlpha, float targetFadeAlpha, float fadeSeconds, Color backgroundColor)
    {
        Image image = canvasGroup.GetComponent<Image>();
        image.color = backgroundColor;

        float time = 0;

        while (time <= fadeSeconds)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startFadeAlpha, targetFadeAlpha, time / fadeSeconds);
            yield return null;
        }
    }

    private IEnumerator GameWon()
    {
        previousGameState = GameState.gameWon;

        GetPlayer().playerControl.DisablePlayer();

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        yield return StartCoroutine(DisplayMessageRoutine("Well done " + GameResources.Instance.currentPlayer.playerName + "! You defeated the dungeon", Color.white, 2f));

        yield return StartCoroutine(DisplayMessageRoutine("You scored " + gameScore.ToString("###,###0"), Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("Press return to restart the game", Color.white, 0f));

        gameState = GameState.restartGame;
    }

    private IEnumerator GameLost()
    {
        previousGameState = GameState.gameLost;

        GetPlayer().playerControl.DisablePlayer();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(Fade(0f, 1f, 2f, Color.black));

        Enemy[] enemyArray = GameObject.FindObjectsOfType<Enemy>();
        foreach (Enemy enemy in enemyArray)
        {
            enemy.gameObject.SetActive(false);
        }

        yield return StartCoroutine(DisplayMessageRoutine("Too bad " + GameResources.Instance.currentPlayer.playerName + "! You were defeated", Color.white, 2f));

        yield return StartCoroutine(DisplayMessageRoutine("You scored " + gameScore.ToString("###,###0"), Color.white, 4f));

        yield return StartCoroutine(DisplayMessageRoutine("Press return to restart the game", Color.white, 0f));

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
        HelperUtilities.ValidateCheckNullValue(this, nameof(messageTextTMP), messageTextTMP);
        HelperUtilities.ValidateCheckNullValue(this, nameof(canvasGroup), canvasGroup);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(dungeonLevelList), dungeonLevelList);
    }

#endif

    #endregion Validation
}