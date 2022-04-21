using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SetActiveWeaponEvent))]
[DisallowMultipleComponent]
public class ActiveWeapon : MonoBehaviour
{
    [Tooltip("Populate with the SpriteRender on the child Weapon gameobject")]
    [SerializeField] private SpriteRenderer weaponSpriteRender;

    [Tooltip("Populate with the PolygonCollider2D on the child Weapon gameobject")]
    [SerializeField] private PolygonCollider2D weaponPolygonCollider2D;

    [Tooltip("Populate with the Transform on the WeaponShootPosition gameobject")]
    [SerializeField] private Transform weaponShootPositionTransform;
}
