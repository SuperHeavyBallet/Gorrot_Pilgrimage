using UnityEngine;

public class TotemPoleCreator : MonoBehaviour
{

    [SerializeField] GameObject[] totemSections;
    int maxHeight = 5;
    int minHeight = 1;

    [SerializeField]    GameObject totemBase;
    [SerializeField] Transform totemBaseAttachPoint;

    Vector3 sectionAttachPoint;

public    int[] sectionIndexes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sectionAttachPoint = totemBaseAttachPoint.transform.position;

        sectionIndexes = new int[] {0, 0, 0, 0, 0};

        BuildTotemPole();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void BuildTotemPole()
    {
        int thisTotemHeight = UnityEngine.Random.Range(maxHeight, minHeight);

        for (int i = 0; i < thisTotemHeight; i++)
        {
           
            int randomSection = UnityEngine.Random.Range(0, totemSections.Length);

            sectionIndexes[i] = randomSection;

           GameObject totemSection = GameObject.Instantiate(totemSections[randomSection], sectionAttachPoint, Quaternion.identity, this.transform);

            TotemPoleSegmentController sectionController = totemSection.GetComponent<TotemPoleSegmentController>();

            if (sectionController == null)
            {
                Debug.LogError("Missing Totem Section Controller");
                return;
            }

            sectionAttachPoint = sectionController.AttachNextPoint;
        


        }

    }
}
