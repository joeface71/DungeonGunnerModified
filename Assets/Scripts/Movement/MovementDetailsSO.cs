using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MovementDetails_", menuName = "Scriptable Objects/Movement/MovementDetails")]
public class MovementDetailsSO : ScriptableObject
{

    [Space(10)]
    [Header("MOVEMENT DETAILS")]

    [Tooltip("The minimum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum")]
    public float minMoveSpeed = 8f;

    [Tooltip("The maximum move speed. The GetMoveSpeed method calculates a random value between the minimum and maximum")]
    public float maxMoveSpeed = 8f;

    [Tooltip("Roll speed if a roll movement exists")]
    public float rollSpeed; // for player

    [Tooltip("Roll distance if a roll movement exists")]
    public float rollDistance; // for player

    [Tooltip("Cooldown time to prevent spamming of roll")]
    public float rollCoolDownTime;  // for player

    /// <summary>
    /// Get a random movement speed between the min and the max
    /// </summary>
    /// <returns></returns>
    public float GetMoveSpeed()
    {
        if (minMoveSpeed == maxMoveSpeed)
        {
            return minMoveSpeed;
        }
        else
        {
            return Random.Range(minMoveSpeed, maxMoveSpeed);
        }

    }

#if UNITY_EDITOR

    private void OnValidate()
    {

        HelperUtilities.ValidateCheckPositiveRange(this, nameof(minMoveSpeed), minMoveSpeed, nameof(maxMoveSpeed), maxMoveSpeed, false);

        if (rollDistance != 0f || rollSpeed != 0f || rollDistance != 0f)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollDistance), rollDistance, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollSpeed), rollSpeed, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(rollCoolDownTime), rollCoolDownTime, false);

        }

    }

#endif


}
