using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class Settings
{

    #region Units
    public const float pixelsPerUnit = 16f;
    public const float tileSizePixels = 16f;
    #endregion

    #region DUNGEON BUILD SETTINGS

    public const int maxDungeonRebuildAttemptsForRoomGraph = 1000;
    public const int maxDungeonBuildAttempts = 10;

    #endregion


    #region ROOM SETTINGS

    public const int maxChildCorridors = 3; // Max number of child corridors leading from a room. - maximum should be 3 although this is not recommended since it can cause the dungeon building to fail since the rooms are more likely to not fit together;
    public const float fadeInTime = 0.5f;
    public const float doorUnlockDelay = 1f;

    #endregion

    #region ANIMATION PARAMETERS
    // Animator parameters - Player
    public static int aimUp = Animator.StringToHash("aimUp");
    public static int aimDown = Animator.StringToHash("aimDown");
    public static int aimUpRight = Animator.StringToHash("aimUpRight");
    public static int aimUpLeft = Animator.StringToHash("aimUpLeft");
    public static int aimRight = Animator.StringToHash("aimRight");
    public static int aimLeft = Animator.StringToHash("aimLeft");
    public static int isIdle = Animator.StringToHash("isIdle");
    public static int isMoving = Animator.StringToHash("isMoving");
    public static int rollUp = Animator.StringToHash("rollUp");
    public static int rollDown = Animator.StringToHash("rollDown");
    public static int rollLeft = Animator.StringToHash("rollLeft");
    public static int rollRight = Animator.StringToHash("rollRight");
    public static int flipUp = Animator.StringToHash("flipUp");
    public static int flipRight = Animator.StringToHash("flipRight");
    public static int flipLeft = Animator.StringToHash("flipLeft");
    public static int flipDown = Animator.StringToHash("flipDown");
    public static int use = Animator.StringToHash("use");
    public static float baseSpeedForPlayerAnimations = 8f;

    // Enemy animator parameters
    public static float baseSpeedForEnemyAnimations = 3f;

    // animator parameters - Door
    public static int open = Animator.StringToHash("open");

    // animator parameters - DamageableDecoration
    public static int destroy = Animator.StringToHash("destroy");
    public static string stateDestroyed = "Destroyed";

    #endregion

    #region Gameobject tags
    public const string playerTag = "Player";
    public const string playerWeapon = "playerWeapon";
    #endregion

    #region Firing Control
    public const float useAimAngleDistance = 3.5f; // if the target distance is less than this amount the aim angle (player pivot) will be used.  Otherwise the weapon aim angle will be used.
    #endregion

    #region AStar Pathfinding Parameters
    public const int defaultAStarMovementPenalty = 40;
    public const int preferredPathAStarMovementPenalty = 1;
    public const float playerMoveDistanceToRebuildPath = 3f;
    public const float enemyPathRebuildCooldown = 2f;
    public const int targetFrameRateToSpreadPathfindingOver = 60;
    #endregion

    #region Enemy Parameters
    public const int defaultEnemyHealth = 20;
    #endregion

    #region UI Parameters
    public const float uiAmmoIconSpacing = 4f;
    public const float uiHeartSpacing = 16f;
    #endregion

    #region Contact Damage Parameters
    public const float contactDamageCollisionResetDelay = 0.5f;
    #endregion

    #region Audio
    public const float musicFadeOutTime = 0.5f;
    public const float musicFadeInTime = 0.5f;
    #endregion

}
