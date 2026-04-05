using UnityEngine;

public class InGameMenuController : MonoBehaviour
{

    [SerializeField] GameObject inGameMenuOverlay;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inGameMenuOverlay.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenInGameMenu()
    {
        inGameMenuOverlay.SetActive(true);
    }

    public void CloseInGameMenu()
    {
        inGameMenuOverlay.SetActive(false);
    }
}
