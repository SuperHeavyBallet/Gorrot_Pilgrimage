using UnityEngine;

public class OverheadDecorationController : MonoBehaviour
{

    [SerializeField] Transform spawnNextPosition;

    public Transform GetSpawnNextPosition => spawnNextPosition;
    [SerializeField] GameObject underhangObject;
    [SerializeField] GameObject spanSupportMesh;
    [SerializeField] GameObject spanObjectMesh;
    Quaternion baseRotation;

    MapData thisMap;

    bool isFirstOrLast;

    private void Awake()
    {
        spanSupportMesh.SetActive(false);
        underhangObject.SetActive(false);
        if (spanObjectMesh != null)
        {
            baseRotation = spanObjectMesh.transform.localRotation;
        }

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

       


    }

    public void SetOverheadDecoration()
    {
        if (isFirstOrLast == true)
        {
            DisplaySpanSupportMesh();
        }
        else
        {
            int rng = UnityEngine.Random.Range(0, 2);
            if (rng == 0)
            {
                DisplayUnderHangObject();
            }
        }
    }

    public void SetSagRotation(int index, int totalSpanAmount)
    {
        if (spanObjectMesh == null || totalSpanAmount <= 1)
            return;

        float middle = (totalSpanAmount - 1) * 0.5f;
        float distanceFromMiddle = Mathf.Abs(index - middle);

        float centreStrength = 1f - (distanceFromMiddle / middle);

        float maxSagAngle = Mathf.Lerp(4f, 18f, Mathf.InverseLerp(2f, 10f, totalSpanAmount));

        float direction = index < middle ? -1f : 1f;

        if (Mathf.Approximately(index, middle))
        {
            direction = 0f;
        }

        float zRotation = centreStrength * maxSagAngle * direction;

        spanObjectMesh.transform.localRotation = baseRotation * Quaternion.Euler(0f, 0f, zRotation);
    }


    public void SetIsFirstOrLast(bool value) => isFirstOrLast = value;

    public void SetThisMapData(MapData newMap) => thisMap = newMap;

    void DisplayUnderHangObject()
    {
        underhangObject.SetActive (true);
    }

    void DisplaySpanSupportMesh()
    {
        spanSupportMesh.SetActive (true);
    }
}
