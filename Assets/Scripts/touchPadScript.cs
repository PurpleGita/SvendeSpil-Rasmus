using UnityEngine;

public class touchPadScript : MonoBehaviour
{
    public Vector2 startPos;
    public Vector2 direction;
    public GameObject player;
    public GameObject SmallPad;          // Reference to the SmallPad child
    public RectTransform Background;     // Reference to the Background child (assumed to be a UI element)


    private float backgroundRadius;

    void Start()
    {
        // Assuming Background is square, calculate the radius as half the width
        backgroundRadius = Background.sizeDelta.x * 0.5f;

        // Initially hide the directional pad visuals
        SmallPad.SetActive(false);
        Background.gameObject.SetActive(false);
    }

    void Update()
    { 
        if (Input.touchCount > 0)
        {
            DetectFingerTouch();
            player.GetComponent<MovementScript>().ReciveInput(direction);
        }
    }

    void DetectFingerTouch()
    {
        Touch touch = Input.GetTouch(0);
        Vector3 touchPosition = touch.position;
        touchPosition.z = 0;

        // Use Background's current position as the center
        Vector2 backgroundCenter = Background.position;

        switch (touch.phase)
        {
            case TouchPhase.Began:
                // When the touch starts, enable the visuals
                SmallPad.SetActive(true);
                Background.gameObject.SetActive(true);

                // Position the Background at the touch start position
                Background.position = touchPosition;
                // Set SmallPad at the center of the Background
                SmallPad.transform.position = touchPosition;
                startPos = touchPosition;
                break;

            case TouchPhase.Moved:
                // Recalculate center in case the Background has moved, which it porbaly has
                backgroundCenter = Background.position;
                Vector2 offset = touch.position - backgroundCenter;

                // Clamp the offset within the allowed radius
                if (offset.magnitude > backgroundRadius)
                {
                    offset = offset.normalized * backgroundRadius;
                }

                SmallPad.transform.position = backgroundCenter + offset;
                Vector2 SmallPadPoition2d = new Vector2(SmallPad.transform.position.x,SmallPad.transform.position.y);

                // Calculate the direction from the start position to the current position
                direction = SmallPadPoition2d - startPos;

                break;

            case TouchPhase.Ended:
                // Hide the directional pad visuals when touch ends
                direction = Vector2.zero;
                SmallPad.SetActive(false);
                Background.gameObject.SetActive(false);
                break;

            case TouchPhase.Stationary:
            case TouchPhase.Canceled:
                // Optionally, handle these phases if needed
                break;
        }
    }
}
