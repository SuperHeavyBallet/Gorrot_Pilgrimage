using UnityEngine;
using GorrotGame;

public class BattleFieldGridBuilder : MonoBehaviour
{
    MapData chosenMap = null;
    MapData mapToBuild = null;
    MapData previousMap;


    bool isLost;

    [SerializeField] MapCatalogue mapCatalogue;
    [SerializeField] PlayerStatReceiver playerStatReceiver;
    [SerializeField] GoalPhaseResolution goalPhaseResolution;

    [SerializeField] PlayerMovementController playerMovementController;

    [SerializeField] BattlefieldBuilder battlefieldBuilder;
    [SerializeField] GameObject battleFieldSquare;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created


   public void BuildBattleFieldGrid(int size, GameObject player)
    {


        if (!battlefieldBuilder.ThisMap.IsFinalCorridoor)
        {
            GameObject[,] newSet = new GameObject[size, size];
            battlefieldBuilder.SetAllSquares(newSet);
            battlefieldBuilder.FreeSquares.Clear();


            int randomGoalSquare = UnityEngine.Random.Range(0, size);

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    GameObject newSquare = Instantiate(battleFieldSquare, transform);

                    if (newSquare != null)
                    {
                        newSquare.transform.localPosition = new Vector3(x, 0, y);
                        battlefieldBuilder.AllSquares[x, y] = newSquare;

                        SquareController newSquareController = newSquare.GetComponent<SquareController>();
                        if (newSquareController != null)
                        {
                            newSquareController.SetSquareMapData(battlefieldBuilder.ThisMap);
                            newSquareController.SetupNewSquare(x, y);
                            newSquareController.AssignPlayer(player);
                        }

                        newSquareController.SetIsInShadow(battlefieldBuilder.ThisMap.GetHasShadows());

                        // Border Placement
                        if (x == 0 || x == size - 1 || y == 0 || y == size - 1) { MakeBorderSquare(x, y, size, size, newSquareController); }

                        // Goal placement
                        bool isGoalSpot = (y == size - 1 && x == randomGoalSquare);
                        if (isGoalSpot)
                        {
                            MakeGoalSquare(newSquareController, newSquare, player);
                            battlefieldBuilder.SetGoalSquareCoord(new Vector2Int(x, y));
                        }

                        // Don't add the player start or goal tile to free list either
                        bool isPlayerStart = (x == battlefieldBuilder.PlayerStartingPosition && y == 0);

                        if (!isPlayerStart && !isGoalSpot) { battlefieldBuilder.FreeSquares.Add(new Vector2Int(x, y)); }


                    }
                    else { Debug.LogError("Square prefab missing SquareController.", newSquare); return; }


                }
            }
        }
        else
        {
            Vector2Int corridoorSize = battlefieldBuilder.ThisMap.GetFinalCorrDimensions();

            int width = corridoorSize.x;
            int height = corridoorSize.y;


            GameObject[,] newSet = new GameObject[width, height];
            battlefieldBuilder.SetAllSquares(newSet);
           
            battlefieldBuilder.FreeSquares.Clear();

            // pick a goal column within corridor width
            int goalX = UnityEngine.Random.Range(0, width);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    GameObject newSquare = Instantiate(battleFieldSquare, transform);
                    //newSquare.transform.position = new Vector3(x, y, 0);

                    newSquare.transform.localRotation = Quaternion.identity;
                    newSquare.transform.localPosition = new Vector3(x, 0, y);

                    battlefieldBuilder.AllSquares[x, y] = newSquare;

                    var sc = newSquare.GetComponent<SquareController>();
                    sc.SetSquareMapData(battlefieldBuilder.ThisMap);
                    sc.SetupNewSquare(x, y);

                    sc.SetIsInShadow(battlefieldBuilder.ThisMap.GetHasShadows());

                    // Border placement using corridor bounds
                    if (x == 0 || x == width - 1 || y == 0 || y == height -1)
                        MakeBorderSquare(x, y, width, height, sc); // see note below

                    // Goal placement: top row of corridor
                    bool isGoalSpot = (y == height - 1 && x == goalX);
                    if (isGoalSpot)
                    {
                        MakeGoalSquare(sc, newSquare, player);
                        battlefieldBuilder.SetGoalSquareCoord(new Vector2Int(x, y));
            
                    }

                    bool isPlayerStart = (x == battlefieldBuilder.PlayerStartingPosition && y == 0);
                    if (!isPlayerStart && !isGoalSpot)
                        battlefieldBuilder.FreeSquares.Add(new Vector2Int(x, y));
                }
            }
        }

    }

    void MakeGoalSquare(SquareController newSquareController, GameObject newSquare, GameObject player)
    {
        if (newSquareController != null)
        {
            newSquareController.MakeSquare(SquareType.Goal, battlefieldBuilder.ThisMap);

            if (player != null)
            {
                if (battlefieldBuilder.PlayerCompassController != null) { battlefieldBuilder.PlayerCompassController.SetGoalLocation(newSquare); }
                else { Debug.LogError("No Compass Controller Component Found on Player"); }

                if (battlefieldBuilder.PlayerDistanceController != null) { battlefieldBuilder.PlayerDistanceController.SetGoalLocation(newSquare); }
                else { Debug.LogError("No Distance Controller Component Found on Player"); }
            }
            else { Debug.Log("No Player Object Found"); }

        }
    }

    void MakeBorderSquare(int x, int y, int width, int height, SquareController sc)
    {

        int[] sidesEmpty =
        {
            x == 0          ? 1 : 0, // left
            y == height - 1 ? 1 : 0, // top
            x == width - 1  ? 1 : 0, // right
            y == 0          ? 1 : 0, // bottom
        };

        sc.AddBorderSquare(sidesEmpty, battlefieldBuilder);
    }

}
