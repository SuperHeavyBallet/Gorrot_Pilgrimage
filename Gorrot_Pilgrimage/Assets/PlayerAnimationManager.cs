using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    bool isMoving;
    [SerializeField] Animator animator;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIsWalking(bool value)
    {
        isMoving = value;
        animator.SetBool("isWalking", value);
    }
}
