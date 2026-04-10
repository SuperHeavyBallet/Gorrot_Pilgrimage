using UnityEngine;
using GorrotGame;
using TMPro;
using UnityEngine.UI;

public class InGameMenuController : MonoBehaviour
{

    [SerializeField] GameObject inGameMenuOverlay;
    [SerializeField] GameObject controlsScreen;

    public bool gameIsPaused;

    [SerializeField] TextMeshProUGUI musicVolumeText;
    [SerializeField] Slider musicVolumeSlider;
    float musicVolume;

    AudioManager musicManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inGameMenuOverlay.SetActive(false);

        musicVolumeSlider.value = AudioManager.Instance.MusicVolume;
    }

    // Update is called once per frame
    void Update()
    {
        gameIsPaused = GorrotGame.GameFunctions.GameIsPaused();

    }

    public void OpenInGameMenu()
    {
        inGameMenuOverlay.SetActive(true);
     
    }

    public void CloseInGameMenu()
    {
        inGameMenuOverlay.SetActive(false);
       
    }

    public void PauseTime()
    {
        Time.timeScale = 0;
        GorrotGame.GameFunctions.SetGameIsPaused(true);
    }

    public void ReturnNormalTime()
    {
        Time.timeScale = 1;
        GorrotGame.GameFunctions.SetGameIsPaused(false);
    }

    public void OpenControlsScreen()
    {
        controlsScreen.SetActive(true);

    }

    public void CloseControlsScreen()
    {
        controlsScreen.SetActive(false);
    }

    public void UpdateMusicVolume()
    {
        AudioManager.Instance.UpdateMusicVolume(musicVolumeSlider.value);
    }
}
