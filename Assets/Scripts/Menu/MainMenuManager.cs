using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the Main Menu UI.
/// Attach this script to a GameObject named "MainMenuManager" in your Main Menu scene.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Main Menu Panel")]
    public GameObject mainMenuPanel;

    [Header("Buttons")]
    public Button continueButton;
    public Button newGameButton;
    public Button loadButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button exitButton;

    [Header("Sub Panels")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;
    public GameObject loadPanel;

    [Header("Scene Settings")]
    [Tooltip("Name of the gameplay scene to load")]
    public string gameplaySceneName = "GameScene";

    // The key used to check if a saved game exists
    private const string SAVE_EXISTS_KEY = "SaveExists";

    private void Start()
    {
        // Show main menu, hide sub-panels
        ShowMainMenu();

        // Disable Continue button if no save exists
        if (continueButton != null)
        {
            bool hasSave = PlayerPrefs.GetInt(SAVE_EXISTS_KEY, 0) == 1;
            continueButton.interactable = hasSave;
        }

        // Wire up button listeners
        if (continueButton != null)  continueButton.onClick.AddListener(OnContinueClicked);
        if (newGameButton != null)   newGameButton.onClick.AddListener(OnNewGameClicked);
        if (loadButton != null)      loadButton.onClick.AddListener(OnLoadClicked);
        if (optionsButton != null)   optionsButton.onClick.AddListener(OnOptionsClicked);
        if (creditsButton != null)   creditsButton.onClick.AddListener(OnCreditsClicked);
        if (exitButton != null)      exitButton.onClick.AddListener(OnExitClicked);
    }

    // ── Button Callbacks ────────────────────────────────────────────────────

    public void OnContinueClicked()
    {
        Debug.Log("Continue clicked — loading saved game...");
        // TODO: Load your save data here before loading the scene
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnNewGameClicked()
    {
        Debug.Log("New Game clicked — starting fresh...");
        // Clear any existing save so the game starts fresh
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(gameplaySceneName);
    }

    public void OnLoadClicked()
    {
        Debug.Log("Load clicked");
        ShowLoadPanel();
    }

    public void OnOptionsClicked()
    {
        Debug.Log("Options clicked");
        ShowOptionsPanel();
    }

    public void OnCreditsClicked()
    {
        Debug.Log("Credits clicked");
        ShowCreditsPanel();
    }

    public void OnExitClicked()
    {
        Debug.Log("Exit clicked — quitting application...");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ── Panel Helpers ────────────────────────────────────────────────────────

    public void ShowMainMenu()
    {
        SetPanelActive(mainMenuPanel, true);
        SetPanelActive(optionsPanel,  false);
        SetPanelActive(creditsPanel,  false);
        SetPanelActive(loadPanel,     false);
    }

    private void ShowOptionsPanel()
    {
        SetPanelActive(mainMenuPanel, false);
        SetPanelActive(optionsPanel,  true);
        SetPanelActive(creditsPanel,  false);
        SetPanelActive(loadPanel,     false);
    }

    private void ShowCreditsPanel()
    {
        SetPanelActive(mainMenuPanel, false);
        SetPanelActive(optionsPanel,  false);
        SetPanelActive(creditsPanel,  true);
        SetPanelActive(loadPanel,     false);
    }

    private void ShowLoadPanel()
    {
        SetPanelActive(mainMenuPanel, false);
        SetPanelActive(optionsPanel,  false);
        SetPanelActive(creditsPanel,  false);
        SetPanelActive(loadPanel,     true);
    }

    private static void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null) panel.SetActive(active);
    }
}
