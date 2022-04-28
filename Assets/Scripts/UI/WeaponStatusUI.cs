using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponStatusUI : MonoBehaviour
{
    [Space(10)]
    [Header("OBJECT REFERENCES")]

    [Tooltip("Populate with image component on the child WeaponImage gameobject")]
    [SerializeField] private Image weaponImage;

    [Tooltip("Populate with the Transform from the child AmmoHolder gameobject")]
    [SerializeField] private Transform ammoHolderTransform;

    [Tooltip("Populate with the TextMeshPro-Text component on the child ReloadText gameobject")]
    [SerializeField] private TextMeshProUGUI reloadText;

    [Tooltip("Populate with the TextMeshPro-Text component on the child AmmoRemainingText gameobject")]
    [SerializeField] private TextMeshProUGUI ammoRemainingText;

    [Tooltip("Populate with the TextMeshPro-Text component on the child WeaponNameText gameobject")]
    [SerializeField] private TextMeshProUGUI weaponNameText;

    [Tooltip("Populate with the RectTransform of the child gameobject ReloadBar")]
    [SerializeField] private Transform reloadBar;

    [Tooltip("Populate with the Image component of the child gameobject BarImage")]
    [SerializeField] private Image barImage;

    private Player player;
    private List<GameObject> ammoIconList = new List<GameObject>();
    private Coroutine reloadWeaponCoroutine;
    private Coroutine blinkingReloadTextCoroutine;

    private void Awake()
    {
        player = GameManager.Instance.GetPlayer();
    }

    private void OnEnable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon += SetActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.OnWeaponFired += WeaponFiredEvent_OnWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon += ReloadWeaponEvent_OnReloadWeapon;

        player.weaponReloadedEvent.OnWeaponReloaded += WeaponReloadedEvent_OnWeaponReloaded;
    }
    private void OnDisable()
    {
        player.setActiveWeaponEvent.OnSetActiveWeapon -= SetActiveWeaponEvent_OnSetActiveWeapon;

        player.weaponFiredEvent.OnWeaponFired -= WeaponFiredEvent_OnWeaponFired;

        player.reloadWeaponEvent.OnReloadWeapon -= ReloadWeaponEvent_OnReloadWeapon;

        player.weaponReloadedEvent.OnWeaponReloaded -= WeaponReloadedEvent_OnWeaponReloaded;
    }

    private void Start()
    {
        // Update active weapon status on the UI
        SetActiveWeapon(player.activeWeapon.GetCurrentWeapon());
    }

    /// <summary>
    /// Handle set active weapon event on the UI
    /// </summary>
    /// <param name="setActiveWeaponEvent"></param>
    /// <param name="setActiveWeaponEventArgs"></param>
    private void SetActiveWeaponEvent_OnSetActiveWeapon(SetActiveWeaponEvent setActiveWeaponEvent, SetActiveWeaponEventArgs setActiveWeaponEventArgs)
    {
        SetActiveWeapon(setActiveWeaponEventArgs.weapon);
    }

    /// <summary>
    /// Handle Weapon fired event on the UI
    /// </summary>
    /// <param name="weaponFiredEvent"></param>
    /// <param name="weaponFiredEventArgs"></param>
    private void WeaponFiredEvent_OnWeaponFired(WeaponFiredEvent weaponFiredEvent, WeaponFiredEventArgs weaponFiredEventArgs)
    {
        WeaponFired(weaponFiredEventArgs.weapon);
    }

    /// <summary>
    /// Weapon fired update UI
    /// </summary>
    /// <param name="weapon"></param>
    private void WeaponFired(Weapon weapon)
    {
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);
        UpdateReloadText(weapon);
    }

    /// <summary>
    /// Handle weapon reload event on the UI
    /// </summary>
    /// <param name="reloadWeaponEvent"></param>
    /// <param name="reloadWeaponEventArgs"></param>
    private void ReloadWeaponEvent_OnReloadWeapon(ReloadWeaponEvent reloadWeaponEvent, ReloadWeaponEventArgs reloadWeaponEventArgs)
    {
        UpdateWeaponReloadBar(reloadWeaponEventArgs.weapon);
    }

    /// <summary>
    /// Handle weapon reloaded event on the UI
    /// </summary>
    /// <param name="weaponReloadedEvent"></param>
    /// <param name="weaponReloadedEventArgs"></param>
    private void WeaponReloadedEvent_OnWeaponReloaded(WeaponReloadedEvent weaponReloadedEvent, WeaponReloadedEventArgs weaponReloadedEventArgs)
    {
        WeaponReloaded(weaponReloadedEventArgs.weapon);
    }

    /// <summary>
    /// Weapon has been reloaded - update UI if current weapon
    /// </summary>
    /// <param name="weapon"></param>
    private void WeaponReloaded(Weapon weapon)
    {
        if (player.activeWeapon.GetCurrentWeapon() == weapon)
        {
            UpdateReloadText(weapon);
            UpdateAmmoText(weapon);
            UpdateAmmoLoadedIcons(weapon);
            ResetWeaponReloadedBar();
        }
    }

    private void SetActiveWeapon(Weapon weapon)
    {
        UpdateActiveWeaponImage(weapon.weaponDetails);
        UpdateActiveWeaponName(weapon);
        UpdateAmmoText(weapon);
        UpdateAmmoLoadedIcons(weapon);

        if (weapon.isWeaponReloading)
        {
            UpdateWeaponReloadBar(weapon);
        }
        else
        {
            ResetWeaponReloadedBar();
        }

        UpdateReloadText(weapon);
    }

    /// <summary>
    /// Populate active weapon image
    /// </summary>
    /// <param name="weaponDetails"></param>
    private void UpdateActiveWeaponImage(WeaponDetailsSO weaponDetails)
    {
        weaponImage.sprite = weaponDetails.weaponSprite;
    }

    /// <summary>
    /// Populate active weapon name
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateActiveWeaponName(Weapon weapon)
    {
        weaponNameText.text = "(" + weapon.weaponListPosition + ")" + weapon.weaponDetails.weaponName.ToUpper();
    }

    /// <summary>
    /// Update ammo remaining text on the UI
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateAmmoText(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteAmmo)
        {
            ammoRemainingText.text = "INFINITE AMMO";
        }
        else
        {
            ammoRemainingText.text = weapon.weaponRemainingAmmo.ToString() + "/" + weapon.weaponDetails.weaponAmmoCapacity.ToString();
        }
    }

    /// <summary>
    /// Update ammo clip icons on the UI
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateAmmoLoadedIcons(Weapon weapon)
    {
        ClearAmmoLoadedIcons();

        for (int i = 0; i < weapon.weaponClipRemainingAmmo; i++)
        {
            GameObject ammoIcon = Instantiate(GameResources.Instance.ammoIconPrefab, ammoHolderTransform);

            ammoIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, Settings.uiAmmoIconSpacing * i);

            ammoIconList.Add(ammoIcon);
        }
    }

    /// <summary>
    /// Clear ammo icons
    /// </summary>
    private void ClearAmmoLoadedIcons()
    {
        foreach (GameObject ammoIcon in ammoIconList)
        {
            Destroy(ammoIcon);
        }

        ammoIconList.Clear();
    }

    /// <summary>
    /// Reload weapon - update the reload bar on the UI
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateWeaponReloadBar(Weapon weapon)
    {
        if (weapon.weaponDetails.hasInfiniteClipCapacity) return;

        StopReloadWeaponCoroutine();
        UpdateReloadText(weapon);

        reloadWeaponCoroutine = StartCoroutine(UpdateWeaponReloadBarRoutine(weapon));
    }

    /// <summary>
    /// Animate reload weapon bar image
    /// </summary>
    /// <param name="currentWeapon"></param>
    /// <returns></returns>
    private IEnumerator UpdateWeaponReloadBarRoutine(Weapon currentWeapon)
    {
        barImage.color = Color.red;

        while (currentWeapon.isWeaponReloading)
        {
            float barfill = currentWeapon.weaponReloadTimer / currentWeapon.weaponDetails.weaponReloadTime;

            reloadBar.transform.localScale = new Vector3(barfill, 1f, 1f);

            yield return null;
        }
    }

    /// <summary>
    /// Initialize the weapon reload bar on the UI
    /// </summary>
    private void ResetWeaponReloadedBar()
    {
        StopReloadWeaponCoroutine();

        barImage.color = Color.green;

        reloadBar.transform.localScale = new Vector3(1f, 1f, 1f);
    }


    /// <summary>
    /// Stop coroutine updating weapon reload progress bar
    /// </summary>
    private void StopReloadWeaponCoroutine()
    {
        if (reloadWeaponCoroutine != null)
        {
            StopCoroutine(reloadWeaponCoroutine);
        }
    }

    /// <summary>
    /// Updates the reload text
    /// </summary>
    /// <param name="weapon"></param>
    private void UpdateReloadText(Weapon weapon)
    {
        if ((!weapon.weaponDetails.hasInfiniteClipCapacity) && (weapon.weaponClipRemainingAmmo <= 0 || weapon.isWeaponReloading))
        {
            barImage.color = Color.red;

            StopBlinkingReloadTextCoroutine();

            blinkingReloadTextCoroutine = StartCoroutine(StartBlinkingReloadTextRoutine());
        }
        else
        {
            StopBlinkingReloadText();
        }
    }

    /// <summary>
    /// Start the coroutine to blink the reload weapon text
    /// </summary>
    /// <returns></returns>
    private IEnumerator StartBlinkingReloadTextRoutine()
    {
        while (true)
        {
            reloadText.text = "RELOAD";
            yield return new WaitForSeconds(0.3f);
            reloadText.text = "";
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// Stop the blinking reload text
    /// </summary>
    private void StopBlinkingReloadText()
    {
        StopBlinkingReloadTextCoroutine();

        reloadText.text = "";
    }

    /// <summary>
    /// Stop the blinking reload text coroutine
    /// </summary>
    private void StopBlinkingReloadTextCoroutine()
    {
        if (blinkingReloadTextCoroutine != null)
        {
            StopCoroutine(blinkingReloadTextCoroutine);
        }
    }

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponImage), weaponImage);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoHolderTransform), ammoHolderTransform);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadText), reloadText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoRemainingText), ammoRemainingText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(weaponNameText), weaponNameText);
        HelperUtilities.ValidateCheckNullValue(this, nameof(reloadBar), reloadBar);
        HelperUtilities.ValidateCheckNullValue(this, nameof(barImage), barImage);

    }
#endif
    #endregion








}
