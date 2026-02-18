using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{

    [SerializeField]
    AnimationCurve curve;

    public void Shaking(float duration, float intensity) 
    {
        StartCoroutine(ShakingLocal(duration, intensity));
    }

    /// <summary>
    /// Coroutine that applies a shaking effect to the GameObject's position for a given duration and intensity.
    /// The shake strength is modulated over time using the provided AnimationCurve.
    /// At the end of the shake, the GameObject's position is reset to its original value.
    /// </summary>
    /// <param name="duration">How long the shake should last, in seconds.</param>
    /// <param name="intensity">The maximum strength of the shake.</param>
    /// <returns>IEnumerator for coroutine execution.</returns>
    public IEnumerator ShakingLocal(float duration,float intensity) 
    { 
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration) * intensity;
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
        Debug.Log("shaked");
    }
}
