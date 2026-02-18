using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class BossAnimationHandler : MonoBehaviour
{

    [SerializeField]
    List<GameObject> _BossBodyParts;
    //0 = Body, 1 = Legs, 2 = Arms, 3 = Head

    [SerializeField]
    Camera _camera;



    void Start()
    {
        //check alle krops dele igennem og ryk dem til at pege mod karakterne (til venstre)
        foreach (GameObject part in _BossBodyParts) 
        {
            foreach (Transform childPart in part.transform)
            {
                childPart.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                
                //for hver kropsdel prøv at få animatoren og sæt attack til 0 (idle animation)
                if (childPart.gameObject.GetComponent<Animator>()) 
                {
                    childPart.gameObject.GetComponent<Animator>().speed = ((float)0.15);
                    childPart.gameObject.GetComponent<Animator>().SetFloat("Attack", 0);

                }
            }
        }
    }

    public void PlayLaserAnimation() 
    {

        foreach (GameObject part in _BossBodyParts)
        {
            if(part.name == "BossBody" || part.name == "BossHead" )
            foreach (Transform childPart in part.transform)
            {  
                if (childPart.gameObject.GetComponent<Animator>())
                {
                    childPart.gameObject.GetComponent<Animator>().speed = ((float)0.15);
                    childPart.gameObject.GetComponent<Animator>().Play("Attack1");
                }


            }
        }
        StartCoroutine(ScreenShake(0.4f, 0.4f, 1.1f));
    }

    public void PlayLasersInRow(float laserWindowTime, float timeBetweenLasers,int amount) 
    {
        List<string> bodyPartNames = new List<string>
        {
            "BossBody",
            "BossHead"
        };

        foreach (GameObject part in _BossBodyParts)
        {
            if (bodyPartNames.Contains(part.name))
                foreach (Transform childPart in part.transform)
                {
                    if (childPart.gameObject.GetComponent<Animator>())
                    {
                        childPart.gameObject.GetComponent<Animator>().speed = ((float)0.15);
                        childPart.gameObject.GetComponent<Animator>().Play("Attack1");
                    }


                }
        }

        StartCoroutine(PauseAnimation(bodyPartNames,1.1f));
        StartCoroutine(ResumeAnimation(bodyPartNames, laserWindowTime));
        for (int i = 0; i < amount; i++) 
        {
            StartCoroutine(ScreenShake(0.4f, 0.4f, 1.1f+(i*timeBetweenLasers)));
        }

    }

    public void PlayHitWallAnimation() 
    {

        foreach (GameObject part in _BossBodyParts)
        {
            if (part.name == "BossArms" || part.name == "BossLegs")
                foreach (Transform childPart in part.transform)
                {
                    if (childPart.gameObject.GetComponent<Animator>())
                    {
                        AnimatorStateInfo stateInfo = childPart.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0);
                        if (!stateInfo.IsName("Attack2"))
                        {
                            childPart.gameObject.GetComponent<Animator>().speed = ((float)0.15);
                            childPart.gameObject.GetComponent<Animator>().Play("Attack3");
                        }


                    }


                }
        }

        StartCoroutine(ScreenShake(1f,2f,1f));
    }

    public void PlaySpikesAnimation(float holdTime) 
    {
        List<string> bodyPartNames = new List<string>
        {
            "BossLegs",
        };

        foreach (GameObject part in _BossBodyParts)
        {
            if (bodyPartNames.Contains(part.name))
                foreach (Transform childPart in part.transform)
                {
                    if (childPart.gameObject.GetComponent<Animator>())
                    {
                        childPart.gameObject.GetComponent<Animator>().speed = ((float)0.15);
                        childPart.gameObject.GetComponent<Animator>().Play("Attack2");
                    }


                }
        }
        StartCoroutine(PauseAnimation(bodyPartNames, 1.2f));
        StartCoroutine(ResumeAnimation(bodyPartNames, holdTime));

    }

    public IEnumerator AnimateLaserScale(Transform laserTransform, int framesToLive)
    {
        float scaleUpDuration = 0.15f;
        float holdDuration = 0.5f;
        float scaleDownDuration = 0.15f;
        float targetScaleY = 2f;

        float initialScaleX = 40f;

        // scale laser up
        float elapsedTime = 0f;
        while (elapsedTime < scaleUpDuration)
        {
            float t = elapsedTime / scaleUpDuration;
            float scaleY = Mathf.Lerp(0, targetScaleY, t);

            laserTransform.localScale = new Vector3(initialScaleX, scaleY, 1);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        laserTransform.localScale = new Vector3(initialScaleX, targetScaleY, 1);

        // hold laser i en størrelse
        yield return new WaitForSeconds(holdDuration);

        // scale laser down
        elapsedTime = 0f;
        while (elapsedTime < scaleDownDuration)
        {
            float t = elapsedTime / scaleDownDuration;
            float scaleY = Mathf.Lerp(targetScaleY, 0, t);

            laserTransform.localScale = new Vector3(initialScaleX, scaleY, 1);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        laserTransform.localScale = Vector3.zero;
    }

    private IEnumerator PauseAnimation(List<string> bodyPartsToAnimateNames, float waitTime) 
    {
        yield return new WaitForSeconds(waitTime);

        Debug.Log("whaaaaaa");
        foreach (GameObject part in _BossBodyParts)
        {
            if (bodyPartsToAnimateNames.Contains( part.name))
                foreach (Transform childPart in part.transform)
                {
                    if (childPart.gameObject.GetComponent<Animator>())
                    {
                        childPart.gameObject.GetComponent<Animator>().enabled = false;
                        Debug.Log("disabled stuff");
                    }


                }
        }
    }

    private IEnumerator ResumeAnimation(List<string> bodyPartsToAnimateNames, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Debug.Log("whaaaaaa");
        foreach (GameObject part in _BossBodyParts)
        {
            if (bodyPartsToAnimateNames.Contains(part.name))
                foreach (Transform childPart in part.transform)
                {
                    if (childPart.gameObject.GetComponent<Animator>())
                    {
                        childPart.gameObject.GetComponent<Animator>().enabled = true;
                        Debug.Log("enabled stuff");
                    }


                }
        }
    }

    private IEnumerator ScreenShake(float intensity,float duration, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _camera.GetComponent<Shake>().Shaking(duration,intensity);

    }
}




