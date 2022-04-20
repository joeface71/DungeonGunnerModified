using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AmmoDetails_", menuName = "Scriptable Objects/Weapons/AmmoDetails")]
public class AmmoDetailsSO : ScriptableObject
{
    [Space(10)]
    [Header("BASIC AMMO DETAILS")]

    [Tooltip("Name for the ammo")]
    public string ammoName;
    public bool isPlayerAmmo;


    [Space(10)]
    [Header("AMMO SPRITE, PREFAB AND MATERIALS")]

    [Tooltip("Sprite to be used for the ammo")]
    public Sprite ammoSprite;

    [Tooltip("Populate with prefab to be used for ammo.  Can place multiple prefabs and a random one will be selected.  Can be an ammo pattern if it implements IFireable")]
    public GameObject[] ammoPrefabArray;

    [Tooltip("Material for the ammo")]
    public Material ammoMaterial;

    [Tooltip("Amount of time to charge ammo if applicable")]
    public float ammoChargeTime = 0.1f;

    [Tooltip("Material to be applied while charging")]
    public Material ammoChargeMaterial;


    [Space(10)]
    [Header("AMMO BASE PARAMETERS")]

    [Tooltip("The damage each ammo deals")]
    public int ammoDamage = 1;

    [Tooltip("Minimum speed of ammo - speed will be random value between min and max")]
    public float ammoSpeedMin = 20f;

    [Tooltip("Maximum speed of ammo - speed will be random value between min and max")]
    public float ammoSpeedMax = 20f;

    [Tooltip("Range of ammo in unity units")]
    public float ammoRange = 20f;

    [Tooltip("Rotation speed in degrees per second of the ammo pattern")]
    public float ammoRotationSpeed = 1f;


    [Space(10)]
    [Header("AMMO SPREAD DETAILS")]

    [Tooltip("Minimum spread angle of the ammo. A random value between min and max will be selected")]
    public float ammoSpreadMin = 0f;

    [Tooltip("Maximum spread angle of the ammo. A random value between min and max will be selected")]
    public float ammoSpreadMax = 0f;


    [Space(10)]
    [Header("AMMO SPAWN DETAILS")]

    [Tooltip("Minimum number of ammo that are spawned per shot.  A random number between min and max is selected")]
    public int ammoSpawnAmountMin = 1;

    [Tooltip("Maximum number of ammo that are spawned per shot.  A random number between min and max is selected")]
    public int ammoSpawnAmountMax = 1;

    [Tooltip("Minimum spawn interval time in seconds.  Random value selected between min and max")]
    public float ammoSpawnIntervalMin = 0f;

    [Tooltip("Maximum spawn interval time in seconds.  Random value selected between min and max")]
    public float ammoSpawnIntervalMax = 0f;


    [Space(10)]
    [Header("AMMO TRAIL DETAILS")]

    [Tooltip("If true, the rest of the ammo trail details should be populated.")]
    public bool isAmmoTrail = false;

    [Tooltip("Lifetime in seconds")]
    public float ammoTrailTime = 3f;

    [Tooltip("Ammo trail material")]
    public Material ammoTrailMaterial;

    [Tooltip("Starting width for the ammo trail")]
    [Range(0f, 1f)] public float ammoTrailStartWidth;

    [Tooltip("Ending width for the ammo trail")]
    [Range(0f, 1f)] public float ammoTrailEndWidth;

    #region Validation
#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckEmptyString(this, nameof(ammoName), ammoName);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoSprite), ammoSprite);
        HelperUtilities.ValidateCheckEnumerableValues(this, nameof(ammoPrefabArray), ammoPrefabArray);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoMaterial), ammoMaterial);
        if (ammoChargeTime > 0)
        {
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoChargeMaterial), ammoMaterial);
        }
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoDamage), ammoDamage, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpeedMin), ammoSpeedMin, nameof(ammoSpeedMax), ammoSpeedMax, false);
        HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoRange), ammoRange, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpreadMin), ammoSpreadMin, nameof(ammoSpreadMax), ammoSpreadMax, true);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnAmountMin), ammoSpawnAmountMin, nameof(ammoSpawnAmountMax), ammoSpawnAmountMax, false);
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(ammoSpawnIntervalMin), ammoSpawnIntervalMin, nameof(ammoSpawnIntervalMax), ammoSpawnIntervalMax, true);
        if (isAmmoTrail)
        {
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailTime), ammoTrailTime, false);
            HelperUtilities.ValidateCheckNullValue(this, nameof(ammoTrailMaterial), ammoTrailMaterial);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailStartWidth), ammoTrailStartWidth, false);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(ammoTrailEndWidth), ammoTrailEndWidth, false);
        }

    }

#endif
    #endregion
}
