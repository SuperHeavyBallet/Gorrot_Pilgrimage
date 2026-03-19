using UnityEngine;

public class BillboardUI : MonoBehaviour
{
    Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (cam == null) return;

        transform.forward = cam.transform.forward;
    }
}
