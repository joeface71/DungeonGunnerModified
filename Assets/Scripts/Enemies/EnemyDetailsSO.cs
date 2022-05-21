using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyDetails_", menuName = "Scriptable Objects/Enemy/EnemyDetails")]
public class EnemyDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("BASE ENEMY DETAILS")]

    [Tooltip("The name of the enemy")]
    public string enemyName;

    [Tooltip("The prefab for the enemy")]
    public GameObject enemyPrefab;

    [Tooltip("Distance to the player before enemy starts chasing")]
    public float chaseDistance = 50f;

    [Space(10)]
    [Header("Enemy Material")]

    [Tooltip("Standard lit shader material to be used after enemy materializes")]
    public Material enemyStandardMaterial;

    [Space(10)]
    [Header("Enemy Materialize Settings")]

    [Tooltip("Time in seconds for enemy to materialize")]
    public float enemyMaterializeTime;

    [Tooltip("Shader to be used when the enemy materializes")]
    public Shader enemyMaterializeShader;

    [ColorUsage(true, true)]
    [Tooltip("HDR color allows intensity adjustment")]
    public Color enemyMaterializeColor;


    #region Validation
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
    }
    #endregion
}