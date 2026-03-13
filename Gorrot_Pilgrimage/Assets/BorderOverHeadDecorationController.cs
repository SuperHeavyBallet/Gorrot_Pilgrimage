using UnityEngine;
using GorrotGame;

public class BorderOverHeadDecorationController : MonoBehaviour
{
    [SerializeField] GameObject overHeadDecoration;
    [SerializeField] Transform overHeadStartPos;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnChainOverhead(int spanAmount, OrthogonalPositions startEdge)
    {

        Vector2 spanDirection = Vector2.zero;
        Transform spawnNextPosition = overHeadStartPos;
        switch (startEdge)
        {
            case OrthogonalPositions.North:
                spanDirection = Vector2.down;
                spawnNextPosition.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;

            case OrthogonalPositions.South:
                spanDirection = Vector2.up;
                spawnNextPosition.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;

            case OrthogonalPositions.East:
                spanDirection = Vector2.left;
                spawnNextPosition.rotation = Quaternion.Euler(0f, 0f, 180f);
                break;

            case OrthogonalPositions.West:
                spanDirection = Vector2.right;
                spawnNextPosition.rotation = Quaternion.Euler(0f, 0f, 0f);
                break;

            default:
                spanDirection = Vector2.zero;
                break;
        }






        if (overHeadDecoration != null)
        {
            for(int i = 0; i < spanAmount; i++)
            {
                

                GameObject newOverhead = Instantiate(overHeadDecoration, spawnNextPosition);
                OverheadDecorationController controller = newOverhead.GetComponent<OverheadDecorationController>();
                if(controller != null )
                {
                    spawnNextPosition = controller.GetSpawnNextPosition;
                }
            }
        }
    }
}
