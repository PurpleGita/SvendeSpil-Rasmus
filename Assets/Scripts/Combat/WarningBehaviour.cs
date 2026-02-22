using UnityEngine;

public class WarningBehaviour : MonoBehaviour
{
    float yScale = 0.3f;
    float tempYScale = 0f;

    void Awake()
    {
        this.transform.localScale = new Vector3 (2, 0, 1); 
    }


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
