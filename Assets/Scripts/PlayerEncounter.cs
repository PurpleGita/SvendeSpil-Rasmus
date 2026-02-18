using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerEncounter : MonoBehaviour
{
    public static int fightId;


    private void OnCollisionEnter(Collision collision) 
    { 
        if(collision.gameObject.tag == "Enemy") 
        {
            //check fight id of the enemy and set fight room to the id
            fightId = collision.gameObject.GetComponent<ExplorationEnemy>().FightID;

            //go to scene
            SceneManager.LoadScene(2);
         
        }
    }
}
