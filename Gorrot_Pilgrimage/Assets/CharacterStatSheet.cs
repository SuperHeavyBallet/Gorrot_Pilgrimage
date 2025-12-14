using System;
using UnityEngine;

public class CharacterStatSheet : MonoBehaviour
{

    public int startingHealth { get; private set; } = 10;
    public int startingMoney { get; private set; } = 5;
    public int startingSuffering { get; private set; } = 0;
    public string characterName { get; private set; } = "Bob";

    public StartLocations startLocation { get; private set; } = StartLocations.Fetsmeld;


    public static CharacterStatSheet Instance { get; private set; }
    void Awake()
    {

        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }

    // Health
    public void setStartingHealth(int health) {  startingHealth = health; }
    public int getStartingHealth() => startingHealth;

    // Money
    public void setStartingMoney(int money) {  startingMoney = money; }
    public int getStartingMoney() => startingMoney;

    // Suffering
    public void setStartingSuffering(int suffering) {  startingSuffering = suffering;}
    public int getStartingSuffering() => startingSuffering;

    // Name
    public void setCharacterName(string name) { characterName = name; }
    public string getCharacterName() => characterName;

    // Location
    public void setCharacterStartLocation(int newStartLocation)
    {
        if (Enum.IsDefined(typeof(StartLocations), startLocation))
        {
            startLocation = (StartLocations)newStartLocation;
        }
        else
        {
            Debug.LogError("Invalid start location index");
        }
    }
    public StartLocations getCharacterStartLocation() => startLocation;


}
