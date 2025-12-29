using System.Collections;
using TMPro;
using UnityEngine;

public class WinPhaseResolution : MonoBehaviour
{
    [SerializeField] GameObject endGameScreen;

    [SerializeField] TurnOrganiser turnOrganiser;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        endGameScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterWinPhase()
    {
        turnOrganiser.UpdateCurrentPhase(TurnOrganiser.ActivePhase.win);

        endGameScreen.SetActive(true);

        StartCoroutine(QuitAfterTime());

    }

    IEnumerator QuitAfterTime()
    {
        yield return new WaitForSeconds(5);

        QuitGame();
    }

    void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                        Application.Quit();
        #endif
    }
}
