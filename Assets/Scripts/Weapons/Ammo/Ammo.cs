using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Ammo : MonoBehaviour, IFireable
{
    [Tooltip("Populate with child TrailRenderer component")]
    [SerializeField] private TrailRenderer trailRenderer;

    private float ammoRange = 0f; // the range of each ammo
    private float ammoSpeed;
    private Vector3 fireDirectionVector;
    private float fireDirectionAngle;
    private SpriteRenderer spriteRenderer;
    private AmmoDetailsSO ammoDetails;
    private float ammoChargeTimer;
    private bool isAmmoMaterialSet = false;
    private bool overrideAmmoMovement;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Ammo charge effect
        if (ammoChargeTimer > 0f)
        {
            ammoChargeTimer -= Time.deltaTime;
            return;
        }
        else if (!isAmmoMaterialSet)
        {
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        // Calculate distance vector to move ammo
        Vector3 distanceVector = fireDirectionVector * ammoSpeed * Time.deltaTime;

        transform.position += distanceVector;

        // Disable after max range reached
        ammoRange -= distanceVector.magnitude;

        if (ammoRange < 0f)
        {
            DisableAmmo();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DisableAmmo();
    }

    /// <summary>
    /// Initialize the ammo being fired - using the ammodetails, the aimangle, weaponAngle and weaponaimdirectionvector.  if this ammo is part of a pattern the ammo movement can be overriden.
    /// </summary>
    /// <param name="ammoDetails"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="ammoSpeed"></param>
    /// <param name="weaponAimDirectionVector"></param>
    /// <param name="overrideAmmoMovement"></param>
    public void InitialiseAmmo(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, float ammoSpeed, Vector3 weaponAimDirectionVector, bool overrideAmmoMovement = false)
    {
        #region Ammo

        this.ammoDetails = ammoDetails;

        // Set fire direction
        SetFireDirection(ammoDetails, aimAngle, weaponAimAngle, weaponAimDirectionVector);

        // Set ammo sprite
        spriteRenderer.sprite = ammoDetails.ammoSprite;

        // set initial ammo material depending on whether there is an ammo charge period
        if (ammoDetails.ammoChargeTime > 0f)
        {
            // set ammo charge timer
            ammoChargeTimer = ammoDetails.ammoChargeTime;
            SetAmmoMaterial(ammoDetails.ammoChargeMaterial);
            isAmmoMaterialSet = false;
        }
        else
        {
            ammoChargeTimer = 0f;
            SetAmmoMaterial(ammoDetails.ammoMaterial);
            isAmmoMaterialSet = true;
        }

        // set ammo range
        ammoRange = ammoDetails.ammoRange;

        // set ammo speed
        this.ammoSpeed = ammoSpeed;

        // Override ammo movement
        this.overrideAmmoMovement = overrideAmmoMovement;

        // Activate ammo gameobject
        gameObject.SetActive(true);

        #endregion

        #region Trail

        if (ammoDetails.isAmmoTrail)
        {
            trailRenderer.gameObject.SetActive(true);
            trailRenderer.emitting = true;
            trailRenderer.material = ammoDetails.ammoMaterial;
            trailRenderer.startWidth = ammoDetails.ammoTrailStartWidth;
            trailRenderer.endWidth = ammoDetails.ammoTrailEndWidth;
            trailRenderer.time = ammoDetails.ammoTrailTime;
        }
        else
        {
            trailRenderer.emitting = false;
            trailRenderer.gameObject.SetActive(false);
        }

        #endregion
    }

    /// <summary>
    /// Set ammo fire direction and angle based on the input angle and direction adjusted by the random spread
    /// </summary>
    /// <param name="ammoDetails"></param>
    /// <param name="aimAngle"></param>
    /// <param name="weaponAimAngle"></param>
    /// <param name="weaponAimDirectionVector"></param>
    private void SetFireDirection(AmmoDetailsSO ammoDetails, float aimAngle, float weaponAimAngle, Vector3 weaponAimDirectionVector)
    {
        // Calculate random spread angle between min and max
        float randomSpread = Random.Range(ammoDetails.ammoSpreadMin, ammoDetails.ammoSpreadMax);

        // Get a random spread toggle of 1 or -1 **** remember this formula for future use ****
        int spreadToggle = Random.Range(0, 2) * 2 - 1;

        if (weaponAimDirectionVector.magnitude < Settings.useAimAngleDistance)
        {
            fireDirectionAngle = aimAngle;
        }
        else
        {
            fireDirectionAngle = weaponAimAngle;
        }

        // Adjust ammo fire angle by random spread
        fireDirectionAngle += spreadToggle * randomSpread;

        // Set ammo rotation
        transform.eulerAngles = new Vector3(0f, 0f, fireDirectionAngle);

        // Set ammo fire direction using helper method
        fireDirectionVector = HelperUtilities.GetDirectionVectorFromAngle(fireDirectionAngle);
    }

    /// <summary>
    /// Disable the ammo - returns it to the object pool
    /// </summary>
    private void DisableAmmo()
    {
        gameObject.SetActive(false);
    }

    public void SetAmmoMaterial(Material material)
    {
        spriteRenderer.material = material;
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    #region Validation

#if UNITY_EDITOR

    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(trailRenderer), trailRenderer);
    }

#endif

    #endregion

}
