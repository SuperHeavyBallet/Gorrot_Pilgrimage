using System.Collections;
using UnityEngine;

public class TotemPoleRotationController : MonoBehaviour
{
    float waitTime;
    float minWaitTime = 1f;
    float maxWaitTime = 8f;

    float rotationSpeed;
    float minRotationSpeed = 5f;
    float maxRotationSpeed = 25f;

    float rotationDistance;
    float minRotationDistance = 2f;
    float maxRotationDistance = 15f;

    void Start()
    {
        StartCoroutine(RotateSection());
    }

    IEnumerator RotateSection()
    {
        while (true)
        {
            CalculateNewWaitTime();
            CalculateRotationSpeed();
            CalculateRotationDistance();

            yield return new WaitForSeconds(waitTime);

            if (Random.value < 0.2f)
            {
                float jitter = Random.Range(0.5f, 2f);
                float jitterDir = Random.value > 0.5f ? 1f : -1f;

                float t = 0f;
                while (t < 0.2f)
                {
                    transform.Rotate(0f, jitterDir * jitter * Time.deltaTime, 0f);
                    t += Time.deltaTime;
                    yield return null;
                }
            }

            // Random direction
            float direction = Random.value > 0.5f ? 1f : -1f;

            float rotatedAmount = 0f;
            float targetRotation = rotationDistance;

            while (rotatedAmount < targetRotation)
            {
                float step = rotationSpeed * Time.deltaTime;

                // Clamp so we don't overshoot
                if (rotatedAmount + step > targetRotation)
                    step = targetRotation - rotatedAmount;

                transform.Rotate(0f, step * direction, 0f);

                rotatedAmount += step;

                yield return null;
            }
        }
    }

    void CalculateNewWaitTime()
    {
        waitTime = Random.Range(minWaitTime, maxWaitTime);
    }

    void CalculateRotationSpeed()
    {
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
    }

    void CalculateRotationDistance()
    {
        rotationDistance = Random.Range(minRotationDistance, maxRotationDistance);
    }
}
