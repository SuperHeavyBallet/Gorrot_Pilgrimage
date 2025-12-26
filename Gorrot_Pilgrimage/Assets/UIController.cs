using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public GameObject blackScreen;
    public Image blackScreenSprite;

    public TextMeshProUGUI currentMapNameText;
    public TextMeshProUGUI currentMapLocationText;

    public TextMeshProUGUI wildMapMarker;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ActivateBlackScreen(bool value) { blackScreen.SetActive(value); }

    public void StartFadeToBlack() { StartCoroutine(ScreenFadeToBlack()); }

    public void StartFadeFromBlack() { StartCoroutine(ScreenFadeFromBlack()); }

    IEnumerator ScreenFadeFromBlack()
    {
        ActivateBlackScreen(true);
        Color color = blackScreenSprite.color;

        float duration = 1f;
        float t = 0f;
        color.a = 1f;
        blackScreenSprite.color = color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            color.a = Mathf.Lerp(1f, 0f, normalized);
            blackScreenSprite.color = color;

            yield return null;
        }

        color.a = 0f;
        blackScreenSprite.color = color;
        ActivateBlackScreen(false);
    }

    public IEnumerator ScreenFadeToBlack()
    {
        Color c = blackScreenSprite.color;
        c.a = 1f;
        blackScreenSprite.color = c;

        float duration = 1f;
        float t = 0f;

        blackScreen.SetActive(true);

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            // Lerp alpha from 0 > 1
            c.a = Mathf.Lerp(0f, 1f, normalized);
            blackScreenSprite.color = c;

            yield return null;
        }

        c.a = 1f;
        blackScreenSprite.color = c;

    }

    public void UpdateMapDataText(string mapName, string mapLocation)
    {
        Debug.Log($"[UIController] UpdateMapDataText called: {mapName} / {mapLocation}", this);
        currentMapNameText.text = mapName;
        currentMapLocationText.text = mapLocation;
    }

    public void LoadCharacterCreationScene()
    {
        SceneManager.LoadScene("CharacterBuild");
    }

    public void UpdateWildMapMarker(bool value)
    {
        if(value == true)
        {
            wildMapMarker.text = "Wild Map";
        }
        else
        {
            wildMapMarker.text = "Not Wild";
        }

    }
}
