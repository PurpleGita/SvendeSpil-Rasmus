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

    ChrsController _chrController;
    bool doneDamage = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _chrController = _chrControllerObject.GetComponent<ChrsController>();
    }


    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.name + " Tag: " + collision.gameObject.tag);
        if (_isActiavted) { 
            if (!doneDamage)
            {
                if (collision.gameObject.tag == "Chr")
                {
                    CombatChrInfo combatChrInfo = collision.gameObject.GetComponent<CombatChrInfo>();

                    if (combatChrInfo != null)
                    {
                        int position = combatChrInfo._position;
                        _chrController.Attacked(position, _attackDMG);
                        doneDamage = true;
                    }
                }
            }
        }
    }

}
