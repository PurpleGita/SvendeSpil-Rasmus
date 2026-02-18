using UnityEngine;

public class FollowPlayerX : MonoBehaviour
{
    public Transform player;
    public Vector2 deadZone = new Vector2(9f, 9f); // Defines movement boundaries before camera moves
    public Vector3 _cameraOffset = new Vector3(0, 8.1f, -0.37f); // Offset of the camera from the player
    public Vector2 smoothTime = new Vector2(3f, 3f); // Time it takes for camera to move to target position
    public Vector3 _camPos;

    void FixedUpdate()
    {
        CalculateCameraPosition();
        transform.position = _camPos;
     }
    
    void CalculateCameraPosition() 
    { 
        _camPos.x = Mathf.Lerp(transform.position.x, 
            player.position.x + _cameraOffset.x, 
            Time.deltaTime * smoothTime.x
            );

        if (_camPos.x - player.position.x > deadZone.x)
            _camPos.x = player.position.x + _cameraOffset.x;
        else if (_camPos.x - player.position.x < -deadZone.x)
            _camPos.x = player.position.x - _cameraOffset.x;

        _camPos.y = _cameraOffset.y;
        _camPos.z = _cameraOffset.z;
    }

}
