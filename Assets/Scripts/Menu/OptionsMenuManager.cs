using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

/// <summary>
/// Manages the Options sub-panel (audio, graphics, controls).
/// Works both from Main Menu and Pause Menu.
/// Attach to the Options Panel GameObject.
/// </summary>
public class OptionsMenuManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer audioMixer;          // Optional: assign your Audio Mixer asset
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    [Header("Graphics")]
    public Toggle fullscreenToggle;
    public Dropdown qualityDropdown;       // Using Legacy Dropdown (Unity UI)
    public Dropdown resolutionDropdown;

    [Header("Back Button")]
    public Button backButton;

    // PlayerPrefs keys
    private const string KEY_MASTER = "MasterVolume";
    private const string KEY_MUSIC  = "MusicVolume";
    private const string KEY_SFX    = "SFXVolume";

    // Cached resolutions list
    private Resolution[] _resolutions;

    // ── Unity Lifecycle ──────────────────────────────────────────────────────

    private void OnEnable()
    {
        LoadSettings();
    }

    private void Start()
    {
        // Build resolution list
        BuildResolutionDropdown();

        // Wire up listeners
        if (masterVolumeSlider != null) masterVolumeSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicVolumeSlider  != null) musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxVolumeSlider    != null) sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        if (fullscreenToggle   != null) fullscreenToggle.onValueChanged.AddListener(SetFullscreen);
        if (qualityDropdown    != null) qualityDropdown.onValueChanged.AddListener(SetQuality);
        if (resolutionDropdown != null) resolutionDropdown.onValueChanged.AddListener(SetResolution);
        if (backButton         != null) backButton.onClick.AddListener(OnBackClicked);
    }

    // ── Resolution Dropdown ──────────────────────────────────────────────────

    private void BuildResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        _resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        var options = new System.Collections.Generic.List<string>();
        int currentIndex = 0;

        for (int i = 0; i < _resolutions.Length; i++)
        {
            string option = $"{_resolutions[i].width} x {_resolutions[i].height}";
            options.Add(option);

            // Check if this matches the current screen resolution
            if (_resolutions[i].width  == Screen.currentResolution.width &&
                _resolutions[i].height == Screen.currentResolution.height)
            {
                currentIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", currentIndex);
        resolutionDropdown.RefreshShownValue();
    }

    // ── Settings Setters ─────────────────────────────────────────────────────

    public void SetMasterVolume(float value)
    {
        if (audioMixer != null)
            // AudioMixer volume is in dB; convert from 0-1 slider
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(KEY_MASTER, value);
    }

    public void SetMusicVolume(float value)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(KEY_MUSIC, value);
    }

    public void SetSFXVolume(float value)
    {
        if (audioMixer != null)
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(Mathf.Max(value, 0.0001f)) * 20);
        PlayerPrefs.SetFloat(KEY_SFX, value);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayerPrefs.SetInt("Quality", qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        if (_resolutions == null || resolutionIndex >= _resolutions.Length) return;
        Resolution res = _resolutions[resolutionIndex];
        Screen.SetResolution(res.width, res.height, Screen.fullScreen);
        PlayerPrefs.SetInt("Resolution", resolutionIndex);
    }

    // ── Load Saved Settings ──────────────────────────────────────────────────

    private void LoadSettings()
    {
        // Volume
        float master = PlayerPrefs.GetFloat(KEY_MASTER, 0.8f);
        float music  = PlayerPrefs.GetFloat(KEY_MUSIC,  0.7f);
        float sfx    = PlayerPrefs.GetFloat(KEY_SFX,    0.8f);

        if (masterVolumeSlider != null) masterVolumeSlider.value = master;
        if (musicVolumeSlider  != null) musicVolumeSlider.value  = music;
        if (sfxVolumeSlider    != null) sfxVolumeSlider.value    = sfx;

        // Apply volumes right away
        SetMasterVolume(master);
        SetMusicVolume(music);
        SetSFXVolume(sfx);

        // Fullscreen
        bool fs = PlayerPrefs.GetInt("Fullscreen", 1) == 1;
        if (fullscreenToggle != null) fullscreenToggle.isOn = fs;

        // Quality
        int quality = PlayerPrefs.GetInt("Quality", QualitySettings.GetQualityLevel());
        if (qualityDropdown != null)
        {
            qualityDropdown.ClearOptions();
            qualityDropdown.AddOptions(new System.Collections.Generic.List<string>(QualitySettings.names));
            qualityDropdown.value = quality;
            qualityDropdown.RefreshShownValue();
        }
    }

    // ── Back Button ──────────────────────────────────────────────────────────

    private void OnBackClicked()
    {
        // Save all PlayerPrefs to disk
        PlayerPrefs.Save();

        // Find which manager opened this panel and tell it to go back
        MainMenuManager mainMenu = FindObjectOfType<MainMenuManager>();
        if (mainMenu != null)
        {
            mainMenu.ShowMainMenu();
            return;
        }

        PauseMenuManager pauseMenu = FindObjectOfType<PauseMenuManager>();
        if (pauseMenu != null)
        {
            pauseMenu.HideOptionsPanel();
        }
    }
}
