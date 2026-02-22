using System.Collections;
using UnityEngine;

public class SpikeBehaviour : MonoBehaviour
{

    public bool goneup = false;

    Color normalColor;

    void Start()
    {
        normalColor = this.GetComponent<SpriteRenderer>().color;
    }


    // Håndtere spike bevægesle og farve
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
    // Coroutine der venter lidt tid før den ændre farven tilbage.
    private IEnumerator changeColorBack(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);
        transform.GetComponent<SpriteRenderer>().color = normalColor;

    }


}
