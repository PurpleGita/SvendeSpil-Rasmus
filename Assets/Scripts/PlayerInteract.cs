using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    public GameObject interactButton;
    public GameObject LastInteractableSeen; //set from interactable objects

     public void InteractButtonPressed()
    {
        Debug.Log("Interact button pressed");
        LastInteractableSeen.GetComponent<interactable>().Interact();
    }

    public void EnableInteractButton()
    {
        interactButton.SetActive(true);
        Debug.Log("Interact button enabled");
    }

    public void DisableInteractButton()
    {
        interactButton.SetActive(false);
        Debug.Log("Interact button disabled");
    }
}
