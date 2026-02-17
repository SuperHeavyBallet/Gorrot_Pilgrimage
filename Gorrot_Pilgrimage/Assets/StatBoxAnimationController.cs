using UnityEngine;

public class StatBoxAnimationController : MonoBehaviour
{
    [SerializeField] Animator attackIconAnimator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShakeAttackAnimator()
    {
        attackIconAnimator.SetTrigger("Shake");
    }
}
