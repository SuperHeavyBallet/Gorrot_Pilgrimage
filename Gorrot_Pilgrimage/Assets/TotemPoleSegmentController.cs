using UnityEngine;

public class TotemPoleSegmentController : MonoBehaviour
{
    [SerializeField] Transform attachNextPoint;
    
    public Vector3 AttachNextPoint => attachNextPoint.transform.position;
}
