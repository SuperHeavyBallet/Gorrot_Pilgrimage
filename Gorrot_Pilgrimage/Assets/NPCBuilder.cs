using UnityEngine;
using GorrotGame;

public class NPCBuilder : MonoBehaviour
{
    [SerializeField] BattlefieldBuilder battlefieldBuilder;

    [SerializeField] GameObject hasMerchantIcon;
    [SerializeField] MerchantShopController merchantShopController;

    [SerializeField] TurnOrganiser turnOrganiser;

    [SerializeField] GameObject fly;

    [SerializeField] GameObject pottardReference;



    public void CheckNPCRequired()
    {
        CheckMerchantNeeded();
        CheckGateKeeperNeeded();

        CheckFlies();
        CheckPottardPlacement();
    }

    void CheckMerchantNeeded()
    {
        if (battlefieldBuilder.ThisMap.GetHasMerchant())
        {
            hasMerchantIcon.SetActive(true);
            PlaceMerchant();
            merchantShopController.SetCurrentMap(battlefieldBuilder.ThisMap);
        }
        else
        {

            hasMerchantIcon.SetActive(false);
        }
    }


    void PlaceMerchant()
    {
        if (battlefieldBuilder.FreeSquares.Count == 0)
        {
            Debug.LogWarning("No free squares left to place Merchant.");
            return;
        }

        int index = UnityEngine.Random.Range(0, battlefieldBuilder.FreeSquares.Count);
        Vector2Int merchantPosition = battlefieldBuilder.FreeSquares[index];
        battlefieldBuilder.FreeSquares.RemoveAt(index); // good idea to reserve it

        SquareController merchantSquareController = battlefieldBuilder.AllSquares[merchantPosition.x, merchantPosition.y].GetComponent<SquareController>();

        if (merchantSquareController != null) { merchantSquareController.MakeSquare(SquareType.Merchant, battlefieldBuilder.ThisMap); }
        else { Debug.LogError("No Merchant Square Controller"); }


    }

    void CheckGateKeeperNeeded()
    {
        PlayerStatsController playerStatController = battlefieldBuilder.Player.GetComponent<PlayerStatsController>();

        if (playerStatController != null)
        {
            int currentMoney = playerStatController.GetPlayerCurrentMoney();
            int currentHealth = playerStatController.GetPlayerCurrentHealth();

            if (currentMoney > 20 || currentHealth > 20)
            {
                Debug.Log("GateKeeper Should Appear...");
            }
        }
    }

    void CheckFlies()
    {
        turnOrganiser.ClearFlies();
        if (battlefieldBuilder.ThisMap.GetHasFlies())
        {
            PlaceFly(battlefieldBuilder.ThisMap.GetMapSize());
        }
    }

    void PlaceFly(int size)
    {
        turnOrganiser.ClearFlies();
        Vector2Int[] freeSqArray = battlefieldBuilder.FreeSquares.ToArray();

        int max = (int)(size * 0.8f); // size 30 => 24
        float t = UnityEngine.Random.value;
        t = t * t; // bias toward 0
        int randomFlyCount = (size / 6) + Mathf.RoundToInt(t * (max - (size / 6)));

        for (int i = 0; i < randomFlyCount; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, battlefieldBuilder.FreeSquares.Count);



            Vector3 newPos = new Vector3(freeSqArray[randomIndex].x, freeSqArray[randomIndex].y, 1);
            GameObject newFly = Instantiate(fly, transform);
            newFly.transform.position = newPos;

            FlyMovementController flyController = newFly.GetComponent<FlyMovementController>();

            if (flyController != null)
            {
                int testX = freeSqArray[randomIndex].x;
                int testY = freeSqArray[randomIndex].y;

                flyController.SetBattleFieldSize(size, battlefieldBuilder.AllSquares);
                flyController.SetPlayerStartSquare(testX, testY);
            }
            else
            {
                Debug.LogError("No Controller on Fly Available", this);
            }

            turnOrganiser.ReceiveFly(newFly);

        }





    }

    void CheckPottardPlacement()
    {

        if (battlefieldBuilder.ThisMap.CanHavePottard && RollPottardPresentRNG())
        {
            PlacePottard();
        }


    }

    bool RollPottardPresentRNG()
    {
        bool finalChoice = false;

        int randomNumber = UnityEngine.Random.Range(0, 2);

        if (randomNumber == 0)
        {
            finalChoice = true;
        }


        return finalChoice;
    }

    void PlacePottard()
    {
        Vector2Int[] freeSqArray = battlefieldBuilder.FreeSquares.ToArray();

        int randomInt = UnityEngine.Random.Range(0, freeSqArray.Length);

        GameObject newPottard = Instantiate(pottardReference, transform);

        PottardController pottardController = newPottard.GetComponent<PottardController>();
        Vector2 startPos = new Vector2(freeSqArray[randomInt].x, freeSqArray[randomInt].y);

        if (pottardController != null)
        {
            pottardController.GetCurrentBattlefield(freeSqArray, battlefieldBuilder.AllSquares);
            pottardController.SetStartPosition(startPos);
        }



    }
}
