using System;
using TMPro;
using UnityEngine;
using System.Linq;

public class CharacterCreationController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI characterNameText;
    [SerializeField] TextMeshProUGUI characterLocationText;
    [SerializeField] TextMeshProUGUI characterHealthText;
    [SerializeField] TextMeshProUGUI characterMoneyText;
    [SerializeField] TextMeshProUGUI characterSufferingText;
    [SerializeField] TextMeshProUGUI characterItemText;

    //[SerializeField] CharacterStatSheet characterStatSheet;

    string characterName;
    StartLocations characterStartLocation;
    int characterStartHealth;
    int characterStartMoney;
    int characterStartSuffering;
    string characterItem;


    [SerializeField] TextAsset firstNamesFile;
    [SerializeField] TextAsset lastNamesFile;

    string[] firstNames;
    string[] lastNames;

    [SerializeField] CharacterSpriteLibrary characterSpriteLibrary;

    

    private void Awake()
    {
        firstNames = ParseLines(firstNamesFile);
        lastNames = ParseLines(lastNamesFile);
    }

    string[] ParseLines(TextAsset file)
    {
        if (file == null) return Array.Empty<string>();

        return file.text
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Where(s => s.Length > 0 && !s.StartsWith("#")) // allow comments
            .ToArray();
    }

    // Start is called once before
    // the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RollNewCharacter();
    }

   


    public void PressRollNewButton()
    {
        RollNewCharacter();
    }

    StartLocations RollRandomStartLocation()
    {
        StartLocations[] values = (StartLocations[])System.Enum.GetValues(typeof(StartLocations));
        int index = UnityEngine.Random.Range(0, values.Length);
        return values[index];
    }

    void RollNewCharacter()
    {
        characterName = RollRandomName();
        characterStartLocation = RollRandomStartLocation();
        characterStartHealth = RandomRoll(0, 20);
        characterStartMoney = RandomRoll(0, 20);
        characterStartSuffering = RandomRoll(0, 10);

        characterSpriteLibrary.BuildNewCharacter();


        UpdateStartStats();
        UpdateTextElements();
    }

    void UpdateStartStats()
    {
        CharacterStatSheet statSheet = CharacterStatSheet.Instance;

        statSheet.SetCharacterName(characterName);
        statSheet.SetCharacterStartLocation(characterStartLocation);
        statSheet.SetStartingHealth(characterStartHealth);
        statSheet.SetStartingMoney(characterStartMoney);
        statSheet.SetStartingSuffering(characterStartSuffering);
    }

    string RollRandomName()
    {
        string first = firstNames[UnityEngine.Random.Range(0, firstNames.Length)];
        string last = lastNames[UnityEngine.Random.Range(0, lastNames.Length)];
        return first + " " + last;
    }

    void UpdateTextElements()
    {
        characterNameText.text = "Name: " + characterName.ToString();
        characterLocationText.text = "From: " + characterStartLocation.ToString();
        characterHealthText.text = "Health: " + characterStartHealth;
        characterMoneyText.text = "Money: " + characterStartMoney;
        characterSufferingText.text = "Suffering: " + characterStartSuffering;
    }

    int RandomRoll(int bottomEndRange , int topEndRange)
    {
        return UnityEngine.Random.Range(bottomEndRange, topEndRange);
    }

  
}
