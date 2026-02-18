using UnityEngine;


public class interactable : MonoBehaviour
{

    public GameObject player;
    public int interactActionID;


    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player has entered the trigger");
            player.GetComponent<PlayerInteract>().EnableInteractButton();
            player.GetComponent<PlayerInteract>().LastInteractableSeen = gameObject;
        }
    }


    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("Player has left the trigger");
            player.GetComponent<PlayerInteract>().DisableInteractButton();
        }
    }


    public void Interact()
    {
         GameObject[] gos;
         gos = GameObject.FindGameObjectsWithTag("Unlockable");

         foreach (GameObject go in gos)
         {
            go.GetComponent<Unlockable>().Unlock(1);
         }

    }

}
