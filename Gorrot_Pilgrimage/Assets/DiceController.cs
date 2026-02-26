using UnityEngine;

public class DiceController : MonoBehaviour
{

    public GameObject dice;
    public Transform diceDropPosition;

    public GameObject side_1;
    public GameObject side_2;
    public GameObject side_3;
    public GameObject side_4;
    public GameObject side_5;
    public GameObject side_6;

    public Vector3 side_1_Pos;
    public Vector3 side_2_Pos;
    public Vector3 side_3_Pos;
    public Vector3 side_4_Pos;
    public Vector3 side_5_Pos;
    public Vector3 side_6_Pos;

    public float side_1_UP = 0;
    public float side_2_UP = 0;
    public float side_3_UP = 0;
    public float side_4_UP = 0;
    public float side_5_UP = 0;
    public float side_6_UP = 0;

    public int winningValue = 0;

    public Rigidbody rb;

    public bool isRolling = false;

    public float settleLinearThreshold = 0.15f;
    public float settleAngularThreshold = 0.15f;
    public float settleTimeForSfx = 0.10f;

    float settleTimer;
    bool sfxPlayed;

    public System.Action OnSettledSfx;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = dice.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        /*
         * 
        if(isRolling)
        {
            if (rb.IsSleeping())
            {
                CalculateFinalRollValue();
            }
            else
            {
                checkWinningValue();
            }
        }
        */

        if (!isRolling) return;

        // keep tracking current best face (fine)
        checkWinningValue();

        // motion check
        bool lowMotion =
            rb.linearVelocity.magnitude < settleLinearThreshold &&
            rb.angularVelocity.magnitude < settleAngularThreshold;

        if (lowMotion)
        {
            settleTimer += Time.fixedDeltaTime;

            // fire SFX when it "looks settled"
            if (!sfxPlayed && settleTimer >= settleTimeForSfx)
            {
                sfxPlayed = true;
                // call your sound here, or raise an event
                //AudioManager.Instance.PlayDiceRollComplete();

                OnSettledSfx?.Invoke();
            }

            // still wait for true sleep (or a longer settle) to finalize value
            if (rb.IsSleeping())
            {
                CalculateFinalRollValue();
            }
        }
        else
        {
            settleTimer = 0f;
        }


    }

    void checkWinningValue()
    {
        side_1_Pos = side_1.transform.position;
        side_2_Pos = side_2.transform.position;
        side_3_Pos = side_3.transform.position;
        side_4_Pos = side_4.transform.position;
        side_5_Pos = side_5.transform.position;
        side_6_Pos = side_6.transform.position;

        side_1_UP = side_1_Pos.y;
        side_2_UP= side_2_Pos.y;
        side_3_UP = side_3_Pos.y;
        side_4_UP = side_4_Pos.y;
        side_5_UP = side_5_Pos.y;
        side_6_UP = side_6_Pos.y;
    }

    public void RollDice()
    {
        // Reset position
        dice.transform.position = diceDropPosition.transform.position;
        dice.transform.rotation = Random.rotation;   // random orientation is optional but looks nice

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        // Random *kick* force downward-ish or forward-ish � adjust to taste
        Vector3 randomForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(2f, 4f),     // upward kick
            Random.Range(-1f, 1f)
        ) * 3f; // overall power

        // Random torque so it actually *spins*
        Vector3 randomTorque = new Vector3(
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f),
            Random.Range(-10f, 10f)
        );

        rb.AddForce(randomForce, ForceMode.Impulse);
        rb.AddTorque(randomTorque, ForceMode.Impulse);

        isRolling = true;

        settleTimer = 0f;
        sfxPlayed = false;
        isRolling = true;
    }

    void CalculateFinalRollValue()
    {
        isRolling = false;
        

        var sides = new (int value, float y)[]
        {
            (1, side_1_UP),
            (2, side_2_UP),
            (3, side_3_UP),
            (4, side_4_UP),
            (5, side_5_UP),
            (6, side_6_UP)
        };

        int bestValue = 0;
        float bestZ = float.MinValue;

        foreach (var s in sides)
        {
            if (s.y > bestZ)
            {
                bestZ = s.y;
                bestValue = s.value;
            }
        }

        winningValue = bestValue;
        Debug.Log("Dice result = " + winningValue);


    }

    public int getDiceResult()
    {
        return winningValue;
    }
}
 