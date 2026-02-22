using System;
using UnityEngine;

public class CharacterStatSheet : MonoBehaviour
{

  
    public int startingHealth { get; private set; } = 99;
    public int startingMoney { get; private set; } = 99;
    public int startingSuffering { get; private set; } = 0;
    public string characterName { get; private set; } = "Bob99";

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
    public void SetStartingHealth(int health) {  startingHealth = health; }
    public int GetStartingHealth() => startingHealth;

    // Money
    public void SetStartingMoney(int money) {  startingMoney = money; }
    public int GetStartingMoney() => startingMoney;

    // Suffering
    public void SetStartingSuffering(int suffering) {  startingSuffering = suffering;}
    public int GetStartingSuffering() => startingSuffering;

    // Name
    public void SetCharacterName(string name) { characterName = name; }
    public string GetCharacterName() => characterName;

    // Location
    public void SetCharacterStartLocation(StartLocations newStartLocation)
    { 
            startLocation = newStartLocation;
        Debug.Log("SET CHARACTER START LOC: " +  startLocation.ToString());
    }
    public StartLocations GetCharacterStartLocation() => startLocation;

    public string GetCharacterHome() => startLocation.ToString();


}
