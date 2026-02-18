using UnityEngine;

public class RockBehaviour : MonoBehaviour
{
    public bool Fall = false;
    private float rotationSpeed;

    private void Start()
    {
        // Assign a random rotation speed to each rock to differentiate them
        rotationSpeed = Random.Range(1f, 5f);
    }

    /// <summary>
    /// Called at a fixed interval to update the rock's behavior.
    /// </summary>
    void FixedUpdate()
    {
        if (Fall)
        {
            // Apply rotation relative to the current orientation
            transform.Rotate(0, 0, rotationSpeed, Space.Self);

            // Move down
            transform.position += new Vector3(0, -0.08f, 0);
        }
    }
}
