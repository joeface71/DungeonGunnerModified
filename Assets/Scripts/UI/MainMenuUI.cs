using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Space(10)]
    [Header("Object References")]

    [Tooltip("Populate with the enter the dungeon play button gameobject")]
    [SerializeField] private GameObject playButton;

    [Tooltip("Populate with the high scores button gameobject")]
    [SerializeField] private GameObject highScoresButton;

    [Tooltip("Populate with the return to main menu button gameobject")]
    [SerializeField] private GameObject returnToMainMenuButton;

    [Tooltip("Populate with the quit button gameobject")]
    [SerializeField] GameObject quitButton;

    [Tooltip("Populate with the instructions button gameobject")]
    [SerializeField] GameObject instructionsButton;

    private bool isHighScoresSceneLoaded = false;
    private bool isInstructionsSceneLoaded = false;

    private void Start()
    {
        MusicManager.Instance.PlayMusic(GameResources.Instance.mainMenuMusic, 0f, 2f);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);

        returnToMainMenuButton.SetActive(false);
    }

    /// <summary>
    /// Called from the Play Game / Enter the Dungeon Button
    /// </summary>
    public void PlayGame()
    {
        SceneManager.LoadScene("MainGameScene");
    }

    /// <summary>
    /// Called from the high scores button
    /// </summary>
    public void LoadHighScores()
    {
        playButton.SetActive(false);
        highScoresButton.SetActive(false);
        quitButton.SetActive(false);
        instructionsButton.SetActive(false);
        isHighScoresSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("HighScoreScene", LoadSceneMode.Additive);
    }

    public void LoadCharacterSelector()
    {
        returnToMainMenuButton.SetActive(false);

        if (isHighScoresSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("HighScoreScene");
            isHighScoresSceneLoaded = false;
        }
        else if (isInstructionsSceneLoaded)
        {
            SceneManager.UnloadSceneAsync("InstructionsScene");
            isInstructionsSceneLoaded = false;
        }

        playButton.SetActive(true);
        quitButton.SetActive(true);
        instructionsButton.SetActive(true);
        highScoresButton.SetActive(true);

        SceneManager.LoadScene("CharacterSelectorScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Called from Load instructions button
    /// </summary>
    public void LoadInstructions()
    {
        playButton.SetActive(false);
        quitButton.SetActive(false);
        highScoresButton.SetActive(false);
        instructionsButton.SetActive(false);
        isInstructionsSceneLoaded = true;

        SceneManager.UnloadSceneAsync("CharacterSelectorScene");

        returnToMainMenuButton.SetActive(true);

        SceneManager.LoadScene("InstructionsScene", LoadSceneMode.Additive);
    }

    /// <summary>
    /// Called from quit button -- wont work in the unity editor
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(playButton), playButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(highScoresButton), highScoresButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(returnToMainMenuButton), returnToMainMenuButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(instructionsButton), instructionsButton);
        HelperUtilities.ValidateCheckNullValue(this, nameof(quitButton), quitButton);
    }

#endif
}