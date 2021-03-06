using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
[DisallowMultipleComponent]
public class EnemyMovementAI : MonoBehaviour
{
    [Tooltip("Movement details SO")]
    [SerializeField] private MovementDetailsSO movementDetails;

    private Enemy enemy;
    private Stack<Vector3> movementSteps = new Stack<Vector3>();
    private Vector3 playerReferencePosition;
    private Coroutine moveEnemyRoutine;
    private float currentEnemyPathRebuildCooldown;
    private WaitForFixedUpdate waitForFixedUpdate;
    [HideInInspector] public float moveSpeed;
    private bool chasePlayer = false;
    [HideInInspector] public int updateFrameNumber = 1; // default value.  Set by the enemy spawner
    private List<Vector2Int> surroundingPositionList = new List<Vector2Int>();

    private void Awake()
    {
        enemy = GetComponent<Enemy>();

        moveSpeed = movementDetails.GetMoveSpeed();
    }

    private void Start()
    {
        waitForFixedUpdate = new WaitForFixedUpdate();

        playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();
    }

    private void Update()
    {
        MoveEnemy();
    }

    /// <summary>
    /// use AStar to build a path to the player and then move the enemy to each grid location on the path
    /// </summary>
    private void MoveEnemy()
    {
        currentEnemyPathRebuildCooldown -= Time.deltaTime;

        if (!chasePlayer && Vector3.Distance(transform.position, GameManager.Instance.GetPlayer().GetPlayerPosition()) < enemy.enemyDetails.chaseDistance)
        {
            chasePlayer = true;
        }

        if (!chasePlayer) return;

        // only process A Star path rebuild on certain frames to spread the load
        if (Time.frameCount % Settings.targetFrameRateToSpreadPathfindingOver != updateFrameNumber) return;

        if (currentEnemyPathRebuildCooldown <= 0f || (Vector3.Distance(playerReferencePosition, GameManager.Instance.GetPlayer().GetPlayerPosition()) > Settings.playerMoveDistanceToRebuildPath))
        {
            currentEnemyPathRebuildCooldown = Settings.enemyPathRebuildCooldown;

            playerReferencePosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

            // Move the enemy using AStar - trigger rebuild of path to player
            CreatePath();

            if (movementSteps != null)
            {
                if (moveEnemyRoutine != null)
                {
                    enemy.idleEvent.CallIdleEvent();
                    StopCoroutine(moveEnemyRoutine);
                }

                moveEnemyRoutine = StartCoroutine(MoveEnemyRoutine(movementSteps));
            }
        }
    }

    /// <summary>
    /// coroutine to move enemy to the next location in the path
    /// </summary>
    private IEnumerator MoveEnemyRoutine(Stack<Vector3> movementSteps)
    {
        while (movementSteps.Count > 0)
        {
            Vector3 nextPosition = movementSteps.Pop();

            // while not very close continue to move - when close move onto the next step
            while (Vector3.Distance(nextPosition, transform.position) > 0.2f)
            {
                enemy.movementToPositionEvent.CallMovementToPositionEvent(nextPosition, transform.position, moveSpeed, (nextPosition - transform.position).normalized);

                yield return waitForFixedUpdate;
            }

            yield return waitForFixedUpdate;
        }

        enemy.idleEvent.CallIdleEvent();
    }

    /// <summary>
    /// Use AStar static class to create a path for the enemy
    /// </summary>
    private void CreatePath()
    {
        Room currentRoom = GameManager.Instance.GetCurrentRoom();

        Grid grid = currentRoom.instantiatedRoom.grid;

        Vector3Int playerGridPosition = GetNearestNonObstaclePlayerPosition(currentRoom);

        Vector3Int enemyGridPosition = grid.WorldToCell(transform.position);

        movementSteps = AStar.BuildPath(currentRoom, enemyGridPosition, playerGridPosition);

        if (movementSteps != null)
        {
            // Take off first step of the path because the enemy is already on it
            movementSteps.Pop();
        }
        else
        {
            enemy.idleEvent.CallIdleEvent();
        }
    }

    /// <summary>
    ///  Set the frame number that the enemy path will be calculated on -- avoids performance spikes
    /// </summary>
    public void SetUpdateFrameNumber(int updateFrameNumber)
    {
        this.updateFrameNumber = updateFrameNumber;
    }

    /// <summary>
    /// Get the nearest position to the player that isn't on an obstacle-
    /// </summary>
    /// <param name="currentRoom"></param>
    /// <returns></returns>
    private Vector3Int GetNearestNonObstaclePlayerPosition(Room currentRoom)
    {
        Vector3 playerPosition = GameManager.Instance.GetPlayer().GetPlayerPosition();

        Vector3Int playerCellPosition = currentRoom.instantiatedRoom.grid.WorldToCell(playerPosition);

        Vector2Int adjustedPlayerCellPosition = new Vector2Int(playerCellPosition.x - currentRoom.templateLowerBounds.x, playerCellPosition.y - currentRoom.templateLowerBounds.y);

        int obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y],
            currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPosition.x, adjustedPlayerCellPosition.y]);

        // if the player isn't currently on a cell marked as an obstacle
        if (obstacle != 0)
        {
            return playerCellPosition;
        }
        // find a surrounding cell that isnt an obstacle -- required because player can be a grid square marked as an obstacle if it has a half collision tile
        else
        {
            surroundingPositionList.Clear();

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) continue;

                    surroundingPositionList.Add(new Vector2Int(i, j));
                }
            }

            for (int l = 0; l < 8; l++)
            {
                int index = Random.Range(0, surroundingPositionList.Count);

                try
                {
                    obstacle = Mathf.Min(currentRoom.instantiatedRoom.aStarMovementPenalty[adjustedPlayerCellPosition.x + surroundingPositionList[index].x,
                        adjustedPlayerCellPosition.y + surroundingPositionList[index].y], currentRoom.instantiatedRoom.aStarItemObstacles[adjustedPlayerCellPosition.x +
                        surroundingPositionList[index].x, adjustedPlayerCellPosition.y + surroundingPositionList[index].y]);

                    // if no obstacle return the cell position to navigate to
                    if (obstacle != 0)
                    {
                        return new Vector3Int(playerCellPosition.x + surroundingPositionList[index].x, playerCellPosition.y + surroundingPositionList[index].y, 0);
                    }
                }
                // catch errors where the surrounding position is outside the grid
                catch
                {

                }

                // Remove the surrounding position with the obstacle so we can try again
                surroundingPositionList.RemoveAt(index);
            }

            // if no non-obstacle cells found surrounding the player - send the enemy in the direction of an enemy spawn position
            return (Vector3Int)currentRoom.spawnPositionArray[Random.Range(0, currentRoom.spawnPositionArray.Length)];
        }
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(movementDetails), movementDetails);
    }

#endif

    #endregion Validation
}