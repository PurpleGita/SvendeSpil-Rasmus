using UnityEngine;

public class Billboard : MonoBehaviour
{
    public Transform cam;

    [SerializeField]
    private bool _canRotate = false;

 
    void LateUpdate()
    {
        if (!_canRotate)
        {
            transform.LookAt(transform.position + cam.forward);
        }
        else
        {
            // Only align X and Y axes with the camera, keep Z axis free
            Vector3 cameraForward = cam.forward;
            cameraForward.y = 0f; // Keep the object upright

            Quaternion targetRotation = Quaternion.LookRotation(cameraForward);
            Vector3 euler = targetRotation.eulerAngles;

            // Maintain the Z rotation
            transform.rotation = Quaternion.Euler(euler.x, euler.y, transform.rotation.eulerAngles.z);
        }
    }
}
