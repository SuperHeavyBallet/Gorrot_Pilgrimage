using UnityEngine;
using UnityEngine.SceneManagement;
public class CharacterCreationMenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClickStart(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
