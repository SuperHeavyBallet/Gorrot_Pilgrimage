using UnityEngine;
using GorrotGame;

public class InGameMenuController : MonoBehaviour
{

    [SerializeField] GameObject inGameMenuOverlay;

    public bool gameIsPaused;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inGameMenuOverlay.SetActive(false);
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
}
