using UnityEngine;
using TMPro;
using NUnit.Framework;
using System.Collections.Generic;
using System;

public class SquareController : MonoBehaviour
{
    public bool hasBeenVisited;

    public GameObject visitedSprite;
    public Transform squareCentre;

    public bool isGoalSquare;
    public bool isTreasureSquare;
    public bool isEnemySquare;
    public bool isTerrainSquare;
    public bool isEmptySquare;
    public bool isHealthSquare;
    public bool isItemSquare;

    public GameObject goalSquareSprite;
    public GameObject treasureSquareSprite;
    public GameObject enemySquareSprite;
    public GameObject terrainSquareSprite;
    public GameObject emptySquareSprite;
    public GameObject healthSquareSprite;
    public GameObject itemSquareSprite;


    public int squareX = 0;
    public int squareY = 0;

    public enum squareQuantity { small, medium, large };
    public squareQuantity square = squareQuantity.medium;
    public string squareQuantityString;

    float spriteScale = 1;

    string squareContentsID = "";

    public TextMeshProUGUI squareValue;

    List<InventoryItemTemplate> allItemsList = new List<InventoryItemTemplate>();

    InventoryItemTemplate[] allItems;

    private void OnEnable()
    {
        decideSquareQuantity();
    }


    void ActivateGameObject(GameObject targetGO)
    {
        goalSquareSprite.SetActive(false);
        enemySquareSprite.SetActive(false);
        treasureSquareSprite.SetActive(false);
        terrainSquareSprite.SetActive(false);
        emptySquareSprite.SetActive(false);
        healthSquareSprite.SetActive(false);
        itemSquareSprite.SetActive(false);

        
        targetGO.SetActive(true);

        if(targetGO != emptySquareSprite && targetGO != goalSquareSprite && targetGO != terrainSquareSprite)
        {
            targetGO.transform.localScale = new Vector3(spriteScale, spriteScale, 1);
        }
        
    }

    void decideSquareQuantity()
    {
        int randomChance = UnityEngine.Random.Range(0, 3);
        switch (randomChance)
        {
            case 0:
                SetSquare(squareQuantity.small, 0.25f, "small");
                break;
           
            case 1:
                SetSquare(squareQuantity.medium, 0.5f, "medium");
                break;
            case 2:
                SetSquare(squareQuantity.large, 1f, "large");
                break;
        }
    }

    void SetSquare(squareQuantity sq, float scale, string name)
    {
        square = sq;
        spriteScale = scale;
        squareQuantityString = name;
    }

    public string getSquareQuantity()
    {
        return squareQuantityString;
    }

    public void MakeHealthSquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = false;
        isHealthSquare = true;

        squareValue.gameObject.SetActive(true);

        switch(square)
        {
            case squareQuantity.small:
                squareValue.text = "+1H";
                break;
            case squareQuantity.medium:
                squareValue.text = "+3H, -1S";
                break;
            case squareQuantity.large:
                squareValue.text = "5H, -3S";
                break;
            default:
                break;

        }

        ActivateGameObject(healthSquareSprite);
    }

    public string GetContentsID()
    {
        return squareContentsID;
    }
    public void MakeItemSquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = false;
        isHealthSquare = false;
        isItemSquare = true;

        squareValue.gameObject.SetActive(true);

        ItemCatalogue itemCatalogue = ItemCatalogue.Instance;

        if (itemCatalogue != null)
        {
            
                allItemsList = itemCatalogue.GetAllItems();

            InventoryItemTemplate[] itemCatalogueArray = allItemsList.ToArray();

            int randomInt = UnityEngine.Random.Range(0, itemCatalogueArray.Length);
            string randomID = "";

            for(int i = 0; i < itemCatalogueArray.Length; i++)
            {
                if(i == randomInt)
                {
                    randomID = itemCatalogueArray[i].itemID;
                }
            }

            squareContentsID = randomID;


        }

        squareValue.text = squareContentsID;


        /*
        switch(square)
        {
            case squareQuantity.small:
                squareValue.text = "+1H";
                break;
            case squareQuantity.medium:
                squareValue.text = "+3H, -1S";
                break;
            case squareQuantity.large:
                squareValue.text = "5H, -3S";
                break;
            default:
                break;
        }*/

        ActivateGameObject(itemSquareSprite);
    }

    public void MakeGoalSquare()
    {
        isGoalSquare = true;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = false;
        isHealthSquare = true;

        squareValue.gameObject.SetActive(false);

        ActivateGameObject(goalSquareSprite);
    }

    public void MakeTreasureSquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = true;
        isEmptySquare = false;

        squareValue.gameObject.SetActive(true);

        switch (square)
        {
            case squareQuantity.small:
                squareValue.text = "-1S";
                break;
            case squareQuantity.medium:
                squareValue.text = "-3S";
                break;
            case squareQuantity.large:
                squareValue.text = "-5S";
                break;
            default:
                break;

        }

        ActivateGameObject(treasureSquareSprite);

    }

    public void MakeEnemySquare()
    {
        isGoalSquare= false;
        isEnemySquare = true;
        isTreasureSquare = false;
        isEmptySquare = false;

        squareValue.gameObject.SetActive(true);

        switch (square)
        {
            case squareQuantity.small:
                squareValue.text = "-1H, >2";
                break;
            case squareQuantity.medium:
                squareValue.text = "-3H, >3";
                break;
            case squareQuantity.large:
                squareValue.text = "-5H, >4";
                break;
            default:
                break;

        }

        ActivateGameObject(enemySquareSprite);

    }

    public void MakeTerrainSquare()
    {
        isGoalSquare= false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isTerrainSquare = true;
        isEmptySquare = false;

        squareValue.gameObject.SetActive(false);

        ActivateGameObject(terrainSquareSprite);
    }


    public void MakeEmptySquare()
    {
        isGoalSquare = false;
        isEnemySquare = false;
        isTreasureSquare = false;
        isEmptySquare = true;

        squareValue.gameObject.SetActive(false);

        ActivateGameObject(emptySquareSprite);
    }

  

    public Transform GetSquareCentre()
    {
        return squareCentre;
    }

    public void ActivateSquareVisited()
    {
        hasBeenVisited = true;
        visitedSprite.gameObject.SetActive(true);
    }

    public void SetSquarePosition(int x, int y)
    {
        squareX = x; squareY = y;

    }

    public int GetSquareXPosition()
    {
        return squareX;
    }

    public int GetSquareYPosition()
    {
        return squareY;
    }

    public bool isMoveableSquare()
    {
        return !isTerrainSquare;
    }
}
