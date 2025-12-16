using System.Collections;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    bool isMoving;
    [SerializeField] Animator animator;

    [Header("Front Sprites")]
    [SerializeField] Sprite front_Head;
    [SerializeField] Sprite front_Body;
    [SerializeField] Sprite front_Hands;
    [SerializeField] Sprite front_Legs;
    [SerializeField] Sprite front_Feet;
    [Space(10)]
    [Header("Back Sprites")]
    [SerializeField] Sprite back_Head;
    [SerializeField] Sprite back_Body;
    [SerializeField] Sprite back_Hands;
    [SerializeField] Sprite back_Legs;
    [SerializeField] Sprite back_Feet;
    [Space(10)]
    [Header("Back Sprites")]
    [SerializeField] Sprite side_Head;
    [SerializeField] Sprite side_Body;
    [SerializeField] Sprite side_Hands;
    [SerializeField] Sprite side_Legs;
    [SerializeField] Sprite side_Feet;
    [Space(10)]
    [Header("Sprite Renderers")]
    [SerializeField] SpriteRenderer headRenderer;
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] SpriteRenderer handsRenderer;
    [SerializeField] SpriteRenderer legsRenderer;
    [SerializeField] SpriteRenderer feetRenderer;
    [Space(10)]
    [Header("Effect Sprites")]
    [SerializeField] GameObject spinIcon;
    [SerializeField] SpriteRenderer spinIconRenderer;
    [Space(10)]
    [Header("Sprite Container")]
    [SerializeField] GameObject spriteContainer;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spinIcon.SetActive(false);
        spriteContainer.transform.localScale = new Vector3(1, 1, 1);
        headRenderer.sprite = back_Head;
        bodyRenderer.sprite = back_Body;
        handsRenderer.sprite = back_Hands;
        legsRenderer.sprite = back_Legs;
        feetRenderer.sprite = back_Feet;
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

    public void SetBackSprites()
    {
        spriteContainer.transform.localScale = new Vector3(1, 1, 1);
        StopAllCoroutines();
        StartSpinAnimation();
        headRenderer.sprite = back_Head;
        bodyRenderer.sprite = back_Body;
        handsRenderer.sprite = back_Hands;
        legsRenderer.sprite = back_Legs;
        feetRenderer.sprite = back_Feet;
    }

    public void SetFrontSprites()
    {
        spriteContainer.transform.localScale = new Vector3(1, 1, 1);
        StopAllCoroutines();
        StartSpinAnimation();
        headRenderer.sprite = front_Head;
        bodyRenderer.sprite = front_Body;
        handsRenderer.sprite = front_Hands;
        legsRenderer.sprite =front_Legs;
        feetRenderer.sprite=front_Feet;
    }

    public void SetSideSprites(string direction)
    {
        StopAllCoroutines();
        StartSpinAnimation();

        if(direction == "right")
        {
            spriteContainer.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            spriteContainer.transform.localScale = new Vector3(1, 1, 1);
        }

        headRenderer.sprite = side_Head;
        bodyRenderer.sprite = side_Body;
        handsRenderer.sprite=side_Hands;
        legsRenderer.sprite=side_Legs;
        feetRenderer.sprite=side_Feet;
    }

    public void StartSpinAnimation()
    {
        StartCoroutine(SpinAnimation());   
    }
    public IEnumerator SpinAnimation()
    {
       

        spinIcon.SetActive(true);

        yield return new WaitForSeconds(0.1f);

        spinIcon.SetActive(false);
    }
}
