using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSpriteSet", menuName = "Scriptable Objects/CharacterSpriteSet")]
public class CharacterSpriteSet : ScriptableObject
{
    // In Game Sprites
    
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

    [SerializeField] Sprite menuHead;
    [SerializeField] Sprite menuBody;
    [SerializeField] Sprite menuHands;
    [SerializeField] Sprite menuLegs;
    [SerializeField] Sprite menuFeet;

    public Sprite GetMenuSpriteHead() => menuHead;

    public Sprite GetMenuSpriteBody() => menuBody;

    public Sprite GetMenuSpriteHands() => menuHands;

    public Sprite GetMenuSpriteLegs() => menuLegs;

    public Sprite GetMenuSpriteFeet() => menuFeet;

}
