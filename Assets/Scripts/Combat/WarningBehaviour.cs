using UnityEngine;

public class WarningBehaviour : MonoBehaviour
{
    float yScale = 0.3f;
    float tempYScale = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        this.transform.localScale = new Vector3 (2, 0, 1); 
    }

    /// <summary>
    /// Called at a fixed interval to gradually increase the GameObject's local Y scale up to a maximum of 2.
    /// Creates a vertical "growing" warning effect by incrementing the Y scale each frame.
    /// </summary>
    void FixedUpdate()
    {
        if (this.transform.localScale.y < 2)
        {
            tempYScale += yScale;
            this.transform.localScale = new Vector3(
                transform.localScale.x
                , tempYScale
                , transform.localScale.z
                );
        }
    }
}
