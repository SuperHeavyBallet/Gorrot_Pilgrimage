using UnityEngine;
using System.Collections;

public class ResizeElementOnClick : MonoBehaviour
{

    [Header("Target")]
    [SerializeField] private Transform target; // Use Transform to avoid GameObject->transform every time

    [Header("Pulse Settings")]
    [Tooltip("Scale multiplier relative to the original scale. Example: 1.1 = +10%")]
    [SerializeField] private float scaleMultiplier = 1.05f;

    [Tooltip("Seconds to scale to the target size.")]
    [SerializeField] private float scaleUpDuration = 0.05f;

    [Tooltip("Seconds to hold at the target size.")]
    [SerializeField] private float holdDuration = 0.05f;

    [Tooltip("Seconds to return to original size.")]
    [SerializeField] private float scaleDownDuration = 0.1f;

    [Header("Behavior")]
    [Tooltip("If true, clicking again restarts the pulse from the current scale.")]
    [SerializeField] private bool restartIfClickedAgain = true;

    private Vector3 _originalScale;
    private Coroutine _pulseRoutine;

    private void Awake()
    {
        if (target == null) target = transform;
        _originalScale = target.localScale;
    }

    /// <summary>
    /// Call this from a UI Button OnClick or any other event.
    /// </summary>
    public void OnClick()
    {
        if (_pulseRoutine != null)
        {
            if (!restartIfClickedAgain) return;
            StopCoroutine(_pulseRoutine);
        }

        _pulseRoutine = StartCoroutine(Pulse());
    }

    private IEnumerator Pulse()
    {
        Vector3 startScale = target.localScale; // restart from current scale (feels snappy)
        Vector3 peakScale = _originalScale * Mathf.Max(0.0001f, scaleMultiplier);

        if (scaleUpDuration <= 0f)
            target.localScale = peakScale;
        else
            yield return ScaleOverTime(startScale, peakScale, scaleUpDuration);

        if (holdDuration > 0f)
            yield return new WaitForSeconds(holdDuration);

        if (scaleDownDuration <= 0f)
            target.localScale = _originalScale;
        else
            yield return ScaleOverTime(target.localScale, _originalScale, scaleDownDuration);

        target.localScale = _originalScale; // hard snap to avoid float drift
        _pulseRoutine = null;
    }

    private IEnumerator ScaleOverTime(Vector3 from, Vector3 to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime; // unscaled feels better for UI; swap to Time.deltaTime for gameplay objects
            float lerp = Mathf.Clamp01(t / duration);
            target.localScale = Vector3.LerpUnclamped(from, to, lerp);
            yield return null;
        }

        target.localScale = to;
    }

    /// <summary>
    /// If you ever change the target's base scale at runtime and want this script to respect it.
    /// </summary>
    public void RebindOriginalScale()
    {
        _originalScale = target.localScale;
    }
}
