using UnityEngine;
using UnityEngine.UI;

public class CharacterSpriteLibrary : MonoBehaviour
{
   

    public Image headDisplay;
    public Image bodyDisplay;
    public Image handsDisplay;
    public Image legsDisplay;
    public Image feetDisplay;


    [SerializeField] CharacterSpriteSet[] characterSpriteSets;

    public void BuildNewCharacter()
    {

        headDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetMenuSpriteHead();
        bodyDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetMenuSpriteBody();
        handsDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetMenuSpriteHands();
        legsDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetMenuSpriteLegs();
        feetDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetMenuSpriteFeet();


    }

   CharacterSpriteSet GetRandomFromArray(CharacterSpriteSet[] inputArray)
    {

        int randomNumber = UnityEngine.Random.Range(0, inputArray.Length);

        return inputArray[randomNumber];

    }
}
