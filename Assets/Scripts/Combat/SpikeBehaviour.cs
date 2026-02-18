using System.Collections;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{

    public bool goneup = false;

    Color normalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        normalColor = this.GetComponent<SpriteRenderer>().color;
    }

    private void Awake()
    {
    }

    // FixedUpdate is called at a fixed interval and is used for physics updates
    // Handles the spike's movement and color changes as it rises and falls.
    // - Moves the spike up until it reaches a certain height, then marks it as "gone up" and changes its color to red briefly.
    // - After reaching the top, the spike moves down each frame.
    void FixedUpdate()
    {
        if(goneup == false) { 
            if (transform.position.y < 1.17) 
            {
                transform.position = new Vector3(transform.position.x, transform.position.y+0.2f, transform.position.z);
                transform.GetComponent<SpriteRenderer>().color = normalColor;
            }
            else { goneup = true; transform.GetComponent<SpriteRenderer>().color = Color.red; StartCoroutine(changeColorBack(0.2f)); }


        }else 
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
            
        }

    }
    // Coroutine that waits for a specified time before resetting the spike's color to its original value.
    // <param name="waitTime">Time in seconds to wait before changing the color back.</param>
    private IEnumerator changeColorBack(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);
        transform.GetComponent<SpriteRenderer>().color = normalColor;

    }


}
