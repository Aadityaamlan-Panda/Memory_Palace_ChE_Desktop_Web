using UnityEngine;

public class UIPanelSlide : MonoBehaviour
{
    public float slideSpeed = 5f;
    public float slideDistance = 0.5f;

    [Header("Floating Settings")]
    public float floatAmplitude = 0.05f;   // how much it moves
    public float floatSpeed = 1.5f;        // how fast it moves

    private Vector3 targetLocalPos;
    private Vector3 startLocalPos;
    private float t = 0f;

    void Start()
    {
        // ✅ Correct target
        targetLocalPos = targetLocalPos - Vector3.up * 0.1f;

        // Start below
        startLocalPos = targetLocalPos - Vector3.up * slideDistance;

        transform.localPosition = startLocalPos;
    }

    void Update()
    {
        // Slide animation
        t += Time.deltaTime * slideSpeed;
        float smoothT = Mathf.SmoothStep(0, 1, t);

        Vector3 basePos = Vector3.Lerp(startLocalPos, targetLocalPos, smoothT);

        // ✅ Floating motion (after slide)
        float floatOffset = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;

        transform.localPosition = basePos + Vector3.up * floatOffset;
    }
}