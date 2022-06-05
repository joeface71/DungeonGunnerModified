using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    [Space(10)]
    [Header("GameObject References")]

    [Tooltip("Populate with the child bar gameobject")]
    [SerializeField] private GameObject healthBar;

    /// <summary>
    /// Enable health bar
    /// </summary>
    public void EnableHealthBar()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Disable health bar
    /// </summary>
    public void DisableHealthBar()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Set health bar value with health percent between 0 and 1
    /// </summary>
    /// <param name="healthPercent"></param>
    public void SetHealthBarValue(float healthPercent)
    {
        healthBar.transform.localScale = new Vector3(healthPercent, 1f, 1f);
    }
}
