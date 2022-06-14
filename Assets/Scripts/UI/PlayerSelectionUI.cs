using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerSelectionUI : MonoBehaviour
{
    [Tooltip("Populate with weapon hand gameobject")]
    public SpriteRenderer playerHandSpriteRenderer;

    [Tooltip("Populate with handnoweapon gameobject")]
    public SpriteRenderer playerHandNoWeaponSpriteRenderer;

    [Tooltip("Populate with weapon gameobject")]
    public SpriteRenderer playerWeaponSpriteRenderer;

    [Tooltip("Populate with animator component")]
    public Animator animator;

    #region Validation
#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandSpriteRenderer), playerHandSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerHandNoWeaponSpriteRenderer), playerHandNoWeaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(playerWeaponSpriteRenderer), playerWeaponSpriteRenderer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(animator), animator);
    }
#endif
    #endregion
}
