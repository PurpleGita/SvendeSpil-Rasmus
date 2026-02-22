using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{

    [SerializeField]
    AnimationCurve _curve;

    public void Shaking(float duration, float intensity) 
    {
        StartCoroutine(ShakingLocal(duration, intensity));
    }


    // Corutine der ryster kamaret.
    public IEnumerator ShakingLocal(float duration,float intensity) 
    { 
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = _curve.Evaluate(elapsedTime / duration) * intensity;
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
        Debug.Log("shaked");
    }
}
