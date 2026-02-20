using UnityEngine;

public class PlaySoundOnClick : MonoBehaviour
{
 

    public void PlayClickSoundEffect() => AudioManager.Instance.PlayClickSoundEffect();
    

   public void PlayPayOffChuckleSoundEffect() => AudioManager.Instance.PlayPayOffChuckle();


    public void PlayAddSufferingSoundEffect() => AudioManager.Instance.playAddSufferingSoundEffect();

    public void PlayAddDamageSoundEffect() => AudioManager.Instance.playTakeDamageSoundEffect();

}
