using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PauseMenuUI : MonoBehaviour
{
    [Tooltip("Populate with the music volume level")]
    [SerializeField] private TextMeshProUGUI musicLevelText;

    [Tooltip("Populate with the sounds volume level")]
    [SerializeField] private TextMeshProUGUI soundsLevelText;

    private void Start()
    {
        gameObject.SetActive(false);
    }



    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    private void OnDisable()
    {
        Time.timeScale = 1;
    }
}
