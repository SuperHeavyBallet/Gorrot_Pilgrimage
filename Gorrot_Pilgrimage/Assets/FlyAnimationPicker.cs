using System.Collections;
using UnityEngine;

public class FlyAnimationPicker : MonoBehaviour
{

    [SerializeField] AnimationClip[] idleAnimations;
    int possibleChoices = 0;
    [SerializeField]  Animator animator;
    int waitingTime;
    int choice;

    Coroutine prepareNextAnimation;

    void Awake()
    {
        if (animator is null)
        {
            Debug.LogError($"{name}: FlyAnimationPicker missing Animator reference.", this);
            enabled = false;
            return;
        }

        if (idleAnimations is null || idleAnimations.Length == 0)
        {
            Debug.LogError($"{name}: No idle animations assigned.", this);
            enabled = false;
            return;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(AnimationLoop());

    }

    IEnumerator AnimationLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(waitingTime);
            PickNextClip();
        }
    }

    void PickNextClip()
    {
        choice = Random.Range(0, idleAnimations.Length);
        waitingTime = Random.Range(0, 3);
        animator.SetInteger("ChosenAnimation", choice);
    }

   
}
