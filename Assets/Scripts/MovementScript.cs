using UnityEngine;

public class MovementScript : MonoBehaviour
{
    public Rigidbody rb;
    public float speed = 1f;
    private Vector3 movementVector = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Physics.gravity = new Vector3(0, -65.0F, 0);
    }

    // Bliver kaldt fra touchPadScript n�r der er input
    public void ReciveInput(Vector2 direction)
    {
        //checkker om spilleren g�r ud af sk�rmen
        if(transform.position.z < 11) 
        { 
            if (direction.y < 0) 
            { 
                direction.y = 0;
            }
        }

        // Convert 2D input om til bev�gelse.
        movementVector = new Vector3(direction.x/80, 0f, direction.y/80) * speed;

        // Flip sprite til at se bedre ud.
        if (direction.x < 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (direction.x > 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    void FixedUpdate()
    {
        //ændre velocity til at være 0 hvis den er meget lille.
        if(movementVector.x < 0.5 && movementVector.x > -0.5) { movementVector.x = 0; }
        if(movementVector.z < 0.5 && movementVector.z > -0.5) { movementVector.z = 0; }

        //Bevæger spilleren
        rb.linearVelocity = movementVector;
    }


    private void OnCollisionEnter(Collision collision)
    {
        // Lav movement om til 0 hvis der er kollision
        movementVector = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }
}
