using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuController : MonoBehaviour
{

    Coroutine loadProcess;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(string sceneString)
    {

        if(loadProcess != null)
        {
            StopCoroutine(loadProcess);
        }
        loadProcess = StartCoroutine(LoadWithMinimumTime(sceneString, 2f));
    }

    IEnumerator LoadWithMinimumTime(string sceneName, float minimumTime)
    {
        float timer = 0f;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (true)
        {
            timer += Time.deltaTime;

            // Scene is loaded when progress reaches ~0.9
            bool loadReady = op.progress >= 0.9f;
            bool timeReady = timer >= minimumTime;

            // Update UI here:
            // - progress bar
            // - spinner
            // - text
            // You can combine op.progress and timer however you want

            if (loadReady && timeReady)
                break;

            yield return null;
        }

        // Optional: small fade-out here
        op.allowSceneActivation = true;
    }


    

    public void PressedQuitButton()
    {
        StartCoroutine(QuitAfterTime());
    }



    IEnumerator QuitAfterTime()
    {
        yield return new WaitForSeconds(0.5f);

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
