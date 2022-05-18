
using UnityEngine;

[System.Serializable]
public class RoomEnemySpawnParameters
{
    [Tooltip("Defines the dungeon level for this room with regard to how many enemies in total should be spawned")]
    public DungeonLevelSO dungeonLevel;

    [Tooltip("Minimum number of enemies to spawn")]
    public int minTotalEnemiesToSpawn;

    [Tooltip("Maximum number of enemies to spawn")]
    public int maxTotalEnemiesToSpawn;

    [Tooltip("Minimum concurrent enemies")]
    public int minConcurrentEnemies;

    [Tooltip("Maximum concurrent enemies")]
    public int maxConcurrentEnemies;

    [Tooltip("Minimum spawn interval")]
    public int minSpawnInterval;

    [Tooltip("Maximum spawn interval")]
    public int maxSpawnInterval;
}
