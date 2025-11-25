using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioClip turnChangeSoundEffect_Player;
    public AudioClip turnChangeSoundEffect_Enemy;
    public AudioSource soundEffectPlayer;

    public AudioClip backgroundMusic;
    public AudioSource backgroundMusicPlayer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playBackgroundMusic(backgroundMusic);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeTurnSound(string turnFor)
    {
        if(turnFor == "player")
        {
            playSoundEffect(turnChangeSoundEffect_Player);
        }
        else if(turnFor == "enemy")
        {
            playSoundEffect(turnChangeSoundEffect_Enemy);
        }
        
    }

    void playBackgroundMusic(AudioClip newMusic)
    {
        backgroundMusicPlayer.PlayOneShot(newMusic);
    }

    void playSoundEffect(AudioClip newClip)
    {
        soundEffectPlayer.PlayOneShot(newClip);
    }
}
