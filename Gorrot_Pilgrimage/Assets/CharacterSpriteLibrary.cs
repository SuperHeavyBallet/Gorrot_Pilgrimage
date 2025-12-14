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

        headDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetSpriteHead();
        bodyDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetSpriteBody();
        handsDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetSpriteHands();
        legsDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetSpriteLegs();
        feetDisplay.sprite = GetRandomFromArray(characterSpriteSets).GetSpriteFeet();


    }

   CharacterSpriteSet GetRandomFromArray(CharacterSpriteSet[] inputArray)
    {

        int randomNumber = UnityEngine.Random.Range(0, inputArray.Length);

        return inputArray[randomNumber];

    }
}
