using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSpriteSet", menuName = "Scriptable Objects/CharacterSpriteSet")]
public class CharacterSpriteSet : ScriptableObject
{

    [SerializeField] Sprite spriteHeadFront;
    [SerializeField] Sprite spriteHeadBack;
    [SerializeField] Sprite spriteHeadSide;

    [SerializeField] Sprite spriteBodyFront;
    [SerializeField] Sprite spriteBodyBack;
    [SerializeField] Sprite spriteBodySide;

    [SerializeField] Sprite spriteHandsFront;
    [SerializeField] Sprite spriteHandsBack;
    [SerializeField] Sprite spriteHandsSide;

    [SerializeField] Sprite spriteLegsFront;
    [SerializeField] Sprite spriteLegsBack;
    [SerializeField] Sprite spriteLegsSide;

    [SerializeField] Sprite spriteFeetFront;
    [SerializeField] Sprite spriteFeetBack;
    [SerializeField] Sprite spriteFeetSide;

    public Sprite GetSpriteHead() => spriteHeadFront;

    public Sprite GetSpriteBody() => spriteBodyFront;

    public Sprite GetSpriteHands() => spriteHandsFront;

    public Sprite GetSpriteLegs() => spriteLegsFront;

    public Sprite GetSpriteFeet() => spriteFeetFront;

}
