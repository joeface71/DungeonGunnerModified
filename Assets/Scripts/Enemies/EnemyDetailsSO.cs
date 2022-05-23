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

    [Space(10)]
    [Header("Enemy Weapon Setting")]

    [Tooltip("Then enemy weapon - None if the enemy doesn't have a weapon")]
    public WeaponDetailsSO enemyWeapon;

    [Tooltip("Minimum delay in seconds between bursts of enemy shooting. A random value between min and max will be selected")]
    public float firingIntervalMin = 0.1f;

    [Tooltip("Maximum delay in seconds between bursts of enemy shooting. A random value between min and max will be selected")]
    public float firingIntervalMax = 1f;

    [Tooltip("Minimum firing duration.  Random value between min and max will be selected")]
    public float firingDurationMin = 1f;

    [Tooltip("Maximum firing duration.  Random value between min and max will be selected")]
    public float firingDurationMax = 1f;

    [Tooltip("Is line of sight needed for firing?")]
    public bool firingLineOfSightRequired;

    [Space(10)]
    [Header("Enemy Health")]

    [Tooltip("The health of the enemy for each level")]
    public EnemyHealthDetails[] enemyHealthDetailsArray;

    [Tooltip("Select if has immunity period immediately after being hit.")]
    public bool isImmuneAfterHit = false;

    [Tooltip("Immunity time in seconds after being hit")]
    public float hitImmunityTime;

    #region Validation
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(enemyName), enemyName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyPrefab), enemyPrefab);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(chaseDistance), chaseDistance, false);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyStandardMaterial), enemyStandardMaterial);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(enemyMaterializeTime), enemyMaterializeTime, true);
        HelperUtilities.ValidateCheckNullValue(this, nameof(enemyMaterializeShader), enemyMaterializeShader);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingIntervalMin), firingIntervalMin, nameof(firingIntervalMax), firingIntervalMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(firingDurationMin), firingDurationMin, nameof(firingDurationMax), firingDurationMax, false);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(enemyHealthDetailsArray), enemyHealthDetailsArray);
        if (isImmuneAfterHit)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(hitImmunityTime), hitImmunityTime, false);
        }
    }
    #endregion
}