using UnityEngine;

[CreateAssetMenu(fileName = "FateOutcome", menuName = "Scriptable Objects/FateOutcome")]
public class FateOutcome : ScriptableObject
{

    public string fateID;
    public string fateName;
    
    public enum statEffected
    {
        health,
        suffering,
        movement,
        attack,
        fate
    }

    public statEffected statToEffect = statEffected.health;

    public int effectDelta;

    public Vector2Int effectDirection;

    public string GetStatEffectedString()
    {
        return statToEffect.ToString();
    }

    public statEffected GetStatEffected()
    {
        return statToEffect;
    }


    public int GetEffectDelta()
    {
        return effectDelta;
    }

    public Vector2Int GetEffectDirection()
    {
        return effectDirection;
    }

    
}
