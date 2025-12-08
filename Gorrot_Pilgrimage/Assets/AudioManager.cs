using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public AudioClip turnChangeSoundEffect_Player;
    public AudioClip turnChangeSoundEffect_Enemy;
    public AudioClip cannotMoveSoundEffect;
    public AudioSource soundEffectPlayer;

    public AudioSource diceRollSoundEffectPlayer;
    public AudioClip diceHitSoundEffect;

    public AudioClip backgroundMusic;
    public AudioSource backgroundMusicPlayer;

    public bool musicOff;
    public int musicVolume;

    public AudioClip takeDamageSoundEffect;

    public AudioClip combatWinSoundEffect;

    public AudioClip healthBoostSoundEffect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
        playBackgroundMusic(backgroundMusic);
    }

    // Update is called once per frame
    void Update()
    {
        if (musicOff)
        {
            musicVolume = 0;
        }
        else
        {
            musicVolume = 1;
        }
    }

    public void changeTurnSound(string turnFor)
    {
        if (turnFor == "player")
        {
            playSoundEffect(turnChangeSoundEffect_Player);
        }
        else if (turnFor == "enemy")
        {
            playSoundEffect(turnChangeSoundEffect_Enemy);
        }

    }

    void playBackgroundMusic(AudioClip newMusic)
    {
        backgroundMusicPlayer.Play();
    }

    void playSoundEffect(AudioClip newClip)
    {
        soundEffectPlayer.PlayOneShot(newClip);
    }

    public void playCannotMoveSoundEffect()
    {
        playSoundEffect(cannotMoveSoundEffect);
    }

    public void playDiceHitSoundEffect(float pitch)
    {
        diceRollSoundEffectPlayer.pitch = pitch;
        diceRollSoundEffectPlayer.PlayOneShot(diceHitSoundEffect);
    }

    public void playTakeDamageSoundEffect()
    {
        soundEffectPlayer.PlayOneShot(takeDamageSoundEffect);
    }

    public void playCombatWinSoundEffect()
    {
        soundEffectPlayer.PlayOneShot(combatWinSoundEffect);
    }

    public void playHealthBoostSoundEffect()
    {
        soundEffectPlayer.PlayOneShot(healthBoostSoundEffect);
    }


}
