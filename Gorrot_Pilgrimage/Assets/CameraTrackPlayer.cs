using UnityEngine;

public class CameraTrackPlayer : MonoBehaviour
{
    public GameObject player;
    [SerializeField] float heightOffset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        trackPlayer();
    }

    void trackPlayer()
    {
        this.transform.position = new Vector3(player.transform.position.x, this.transform.position.y, player.transform.position.z);
    }
}
