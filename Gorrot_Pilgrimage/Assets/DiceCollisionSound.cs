using UnityEngine;

public class DiceCollisionSound : MonoBehaviour
{

    public float minImpactVelocity = 1f;   // ignore tiny bumps
    public float cooldown = 0.02f;           // prevent spam

    public AudioManager audioManager;

    private float lastPlayTime = -999f;

    void OnCollisionEnter(Collision collision)
    {
        // Check cooldown so we don't rattle-play
        if (Time.time - lastPlayTime < cooldown) return;

        // Calculate impact strength
        float impact = collision.relativeVelocity.magnitude;

        if (impact > minImpactVelocity)
        {
            float pitch =  Random.Range(0.9f, 1.1f); // slight pitch variation
            audioManager.playDiceHitSoundEffect(pitch);
            lastPlayTime = Time.time;
        }
    }
}
