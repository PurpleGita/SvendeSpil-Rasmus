using UnityEngine;
using UnityEngine.Audio;

public class AttackObjectBehavior : MonoBehaviour
{
    [SerializeField]
    int _attackDMG;

    [SerializeField]
    GameObject _chrControllerObject;

    [SerializeField]
    public bool _isActiavted;

    ChrsController chrController;
    
    bool doneDamage = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chrController = _chrControllerObject.GetComponent<ChrsController>();
    }


    //bliver kaldt når objektet dette script er på collider med noget som helst andet der har en collider component
    void OnTriggerEnter(Collider collision)
    {
        //skriver ud hvad den colliedet med:
        Debug.Log("Collided with: " + collision.gameObject.name + " Tag: " + collision.gameObject.tag);
        //en bool som bliver sat til hvis den er aktivt til at gøre skade (bliver sat af det script der cloner angrebs objektet)
        if (_isActiavted) { 
            //hvis den ikke allerede har gjort skade.
            if (!doneDamage)
            {
                //hvis det den coliddet med rent faktisk var en karatker
                if (collision.gameObject.tag == "Chr")
                {
                    CombatChrInfo combatChrInfo = collision.gameObject.GetComponent<CombatChrInfo>();

                    if (combatChrInfo != null)
                    {
                        //siger til den karakters controller at den er blevet angrebet og hvor meget skaded den burde tage.
                        int position = combatChrInfo._position;
                        chrController.Attacked(position, _attackDMG);

                        //sikre sig at den ikke gør skade hver frame ved at sætte denne bool
                        doneDamage = true;
                    }
                }
            }
        }
    }

}
