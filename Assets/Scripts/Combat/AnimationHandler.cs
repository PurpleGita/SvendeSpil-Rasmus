using UnityEngine;
using System;
using System.Collections;
using UnityEditor;
using Unity.Mathematics;

public class AnimationHandler : MonoBehaviour
{
    Animator animator;
    
    float _hurtTime = 0.1f;

    [SerializeField]
    private GameObject _parryLightObj;

    void Start()
    {
        //tager animationeren på objektet siden der ikke er givet andet logik den skal finde
        animator = GetComponent<Animator>();
        animator.SetFloat("Speed",0);
    }

    public void Walk()
    {
        animator.SetFloat("Speed", 1);

    }

    public void Idle(bool isBelowHP)
    {
        //sæætter til normal idle hvis karakteren ikke er såret og ellers til såret version af idle 
        if (isBelowHP) { animator.SetFloat("Speed", -1); }
        else { animator.SetFloat("Speed", 0); }
        
    }

    public void Attack(int AttackValue) 
    {
        animator.SetBool("IsAttacking", true);
        animator.SetFloat("AttackValue", 0);
    }

    public void StopAttack() {
        animator.SetBool("IsAttacking", false);
    }

    public void Hurt(bool isBelowHP) 
    {
        animator.SetBool("Hurt", true);
        if (isBelowHP) { animator.SetFloat("Speed", -1); }
        else { animator.SetFloat("Speed", 0); }

        StartCoroutine(StopHurt(_hurtTime));
    }

    public bool Block()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        // Chekker at karakteren ikke er i gang med at blokke og ikke trætte efter at prøve at blokke. (så man ikke kan spam blok)
        if (!stateInfo.IsName("Block") && !stateInfo.IsName("Tired"))
        {

            animator.Play("Block");
            return true;
        }
        return false;
    }

    private IEnumerator StopHurt(float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);

        animator.SetBool("Hurt", false);
    }

    public bool CheckIfBlocking() 
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Block"))
        {
            return true;
        }
        else { return false; }
           
    }

    //spiller parry animatinonen og skaber et lys hvor det er.
    public IEnumerator ParryAnimation() 
    {
        animator.Play("Parry");
        Vector3 spawnPosition = new Vector3(
        this.transform.position.x,
        this.transform.position.y,
        this.transform.position.z-2f
        );

        GameObject lightSpawned = Instantiate(_parryLightObj, spawnPosition, Quaternion.identity);
        lightSpawned.active = true;

        yield return new WaitForSeconds(0.3f);

        Destroy(lightSpawned );

    }

}
