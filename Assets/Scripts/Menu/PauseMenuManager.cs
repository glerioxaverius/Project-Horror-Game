using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Pause Menu UI during gameplay.
/// Attach this script to a GameObject named "PauseMenuManager" in your gameplay scene.
/// </summary>
public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Menu Panel")]
    [Tooltip("The dark semi-transparent panel that appears when paused")]
    public GameObject pauseMenuPanel;

    [Header("Buttons")]
    public Button returnToGameButton;
    public Button optionsButton;
    public Button exitToMainMenuButton;

    [Header("Sub Panels")]
    public GameObject optionsPanel;

    [Header("Scene Settings")]
    [Tooltip("Name of the Main Menu scene")]
    public string mainMenuSceneName = "MainMenu";

    [Header("Input")]
    [Tooltip("Key to toggle pause (default: Escape)")]
    public KeyCode pauseKey = KeyCode.Escape;

    // ── State ────────────────────────────────────────────────────────────────

    private bool _isPaused = false;

    public bool IsPaused => _isPaused;

    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    private void Start()
    {
        // Make sure the pause panel is hidden at game start
        HidePauseMenu();

        // Wire up button listeners
        if (returnToGameButton != null)  returnToGameButton.onClick.AddListener(OnReturnToGameClicked);
        if (optionsButton != null)       optionsButton.onClick.AddListener(OnOptionsClicked);
        if (exitToMainMenuButton != null) exitToMainMenuButton.onClick.AddListener(OnExitToMainMenuClicked);
    }

    private void Update()
    {
        // Toggle pause when the player presses the pause key (Escape by default)
        if (Input.GetKeyDown(pauseKey))
        {
            TogglePause();
        }
    }

    // ── Pause Control ────────────────────────────────────────────────────────

    /// <summary>Toggles between paused and unpaused state.</summary>
    public void TogglePause()
    {
        if (_isPaused)
            ResumeGame();
        else
            PauseGame();
    }

    /// <summary>Pauses the game and shows the pause menu.</summary>
    public void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f;          // Stop all game physics/animations
        ShowPauseMenu();
        Debug.Log("Game paused");
    }

    /// <summary>Resumes the game and hides the pause menu.</summary>
    public void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f;          // Resume normal game speed
        HidePauseMenu();
        Debug.Log("Game resumed");
    }

    // ── Button Callbacks ─────────────────────────────────────────────────────

    public void OnReturnToGameClicked()
    {
        ResumeGame();
    }

    public void OnOptionsClicked()
    {
        Debug.Log("Options clicked from Pause Menu");
        ShowOptionsPanel();
    }

    public void OnExitToMainMenuClicked()
    {
        Debug.Log("Exiting to Main Menu...");
        // IMPORTANT: Always reset Time.timeScale before changing scenes,
        // otherwise the main menu will also be frozen!
        Time.timeScale = 1f;
        _isPaused = false;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    // ── Panel Helpers ────────────────────────────────────────────────────────

    private void ShowPauseMenu()
    {
        SetPanelActive(pauseMenuPanel, true);
        SetPanelActive(optionsPanel,   false);
    }

    private void HidePauseMenu()
    {
        SetPanelActive(pauseMenuPanel, false);
        SetPanelActive(optionsPanel,   false);
    }

    private void ShowOptionsPanel()
    {
        SetPanelActive(pauseMenuPanel, false);
        SetPanelActive(optionsPanel,   true);
    }

    public void HideOptionsPanel()
    {
        ShowPauseMenu();
    }

    private static void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }

    // ── Application Focus Helper ─────────────────────────────────────────────

    /// <summary>
    /// Optional: Auto-pause when the game window loses focus.
    /// Unity calls this automatically.
    /// </summary>
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus && !_isPaused)
        {
            PauseGame();
        }
    }
}
