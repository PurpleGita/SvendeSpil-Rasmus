using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyPatternBehaviour : MonoBehaviour
{
    EnemyAttacks enemyAttacks;
    Vector3 startPosition;

    [SerializeField]
    private ChrsController _chrsController;

    [SerializeField]
    private List<GameObject> _chrsAbleToTarget;

    [SerializeField]
    private GameObject _victoryObject;

    [SerializeField]
    private MusicController musicController;

    private int nextAttackPattern = 0;


    // Susbscriber til music contollerens quedtrack event
    void Start()
    {
        enemyAttacks = GetComponent<EnemyAttacks>();
        startPosition = transform.position;

        if (musicController != null)
        {
            musicController.OnQueuedTrackStarted += OnQueuedTrackStarted;
        }
    }

    // Unsubscriber fra musicontrolleren hvis dette object bliver slettet
    void OnDestroy()
    {
        if (musicController != null)
        {
            musicController.OnQueuedTrackStarted -= OnQueuedTrackStarted;
        }
    }

    // Called when the queued music track starts playing.
    // Begins the next attack pattern coroutine.
    private void OnQueuedTrackStarted()
    {
        Debug.Log("Queued track started playing.");
        StartCoroutine(RunAttackPattern(nextAttackPattern, 0f));
        Debug.Log("attack Pattern " + nextAttackPattern);
    }

    // Called when the enemy's turn starts. Returns a list of target positions for the current attack pattern.
    // Also triggers the next music track and sets the next attack pattern.
    // <param name="attackPatternID">The ID of the attack pattern to execute.</param>
    // <returns>List of Vector3 positions to target.</returns>
    public List<Vector3> EnemyTurnStarted(int attackPatternID)
    {
        List<Vector3> vector3sToReturn = new List<Vector3>();

        switch (attackPatternID)
        {
            case 0:
                vector3sToReturn.Add(new Vector3(0f, 2.3f, 15f));
                vector3sToReturn.Add(new Vector3(-2f, 2.3f, 19f));
                vector3sToReturn.Add(new Vector3(-4.5f, 2.3f, 19f));
                vector3sToReturn.Add(new Vector3(-6.5f, 2.3f, 15f));      
                nextAttackPattern = 0;

                if (musicController != null)
                {
                    musicController.nextAudioToPlay = 1;
                    musicController.TriggerNextAudio();
                }
                break;

            case 1:
                vector3sToReturn.Add(new Vector3(0f, 2.3f, 16f));
                vector3sToReturn.Add(new Vector3(-2f, 2.3f, 16f));
                vector3sToReturn.Add(new Vector3(-4f, 2.3f, 16f));
                vector3sToReturn.Add(new Vector3(-6f, 2.3f, 16f));

                nextAttackPattern = 1;

                if (musicController != null)
                {
                    musicController.nextAudioToPlay = 2;
                    musicController.TriggerNextAudio();
                }

                break;


            case 2:
                vector3sToReturn.Add(new Vector3(0f, 2.3f, 16f));
                vector3sToReturn.Add(new Vector3(-2f, 2.3f, 16f));
                vector3sToReturn.Add(new Vector3(-4f, 2.3f, 16f));
                vector3sToReturn.Add(new Vector3(-6f, 2.3f, 16f));

                nextAttackPattern = 2;

                if (musicController != null)
                {
                    musicController.nextAudioToPlay = 3;
                    musicController.TriggerNextAudio();
                }


                break;

            case 3:
                vector3sToReturn.Add(new Vector3(0f, 2.3f, 15f));
                vector3sToReturn.Add(new Vector3(-2f, 2.3f, 19f));
                vector3sToReturn.Add(new Vector3(-4.5f, 2.3f, 19f));
                vector3sToReturn.Add(new Vector3(-6.5f, 2.3f, 15f));

                nextAttackPattern = 3;
                if (musicController != null)
                {
                    musicController.nextAudioToPlay = 4;
                    musicController.TriggerNextAudio();
                }
                break;
        }

        return vector3sToReturn;
    }

    // Coroutine that runs the specified attack pattern after an optional wait time.
    // <param name="attackPatternID">The ID of the attack pattern to execute.</param>
    // <param name="waitTime">Time in seconds to wait before starting the pattern.</param>
    private IEnumerator RunAttackPattern(int attackPatternID, float waitTime)
    {
        float startTime = Time.unscaledTime;

        // Initial delay
        while (Time.unscaledTime < startTime + waitTime)
        {
            yield return null;
        }

        switch (attackPatternID)
        {
            case 0:

                yield return StartCoroutine(ExecuteAttackPattern0());
                break;


            case 1:

                Debug.Log("case 1 called");

                yield return StartCoroutine(ExecuteAttackPattern1());
                break;

            case 2:

                yield return StartCoroutine(ExecuteAttackPattern2());
                break;

            case 3:
                yield return StartCoroutine(ExecuteAttackPattern3());
                break;

            default:
                StartCoroutine(enemyTurnEnded(1f));
                break;
        }
    }



    // Coroutine that ends the enemy's turn after a delay, updates the player controller, and logs the event.
    // <param name="waitTime">Time in seconds to wait before ending the turn.</param>
    private IEnumerator enemyTurnEnded(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _chrsController.turnTimer++;
        _chrsController.ChrsInStartPosition = false;
        _chrsController.currentTurn = 0;
        _chrsController.isPlayersTurn = true;

        Debug.Log("enemy Turn ended");
    }


    // deaktiverer modstanderen og aktiver vicotry
    public IEnumerator Defeated(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _victoryObject.SetActive(true);
        gameObject.SetActive(false);
    }

    // starter det 1 angrebs sekvens
    private IEnumerator ExecuteAttackPattern0()
    {
        Vector3 attackPosition1 = new Vector3(startPosition.x, startPosition.y, 19f);
        Vector3 attackPosition2 = new Vector3(startPosition.x, startPosition.y, 15f);

        // ryk modstanderen til attack positionen
        float moveStartTime = Time.unscaledTime;
        float moveDuration = 1f;

        while (Time.unscaledTime < moveStartTime + moveDuration)
        {
            float t = (Time.unscaledTime - moveStartTime) / moveDuration;
            transform.position = Vector3.Lerp(startPosition, attackPosition1, t);
            yield return null;
        }

        transform.position = attackPosition1;

        // Multiple attacks with warnings and lasers, each with their own timing


        // Attack 1: 1 seconds after start, will spawn at 2.1 seconds
        float attack1StartTime = Time.unscaledTime;
        float attack1Duration = 0.4f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[1], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[1], 0f));

        while (Time.unscaledTime < attack1StartTime + attack1Duration)
        {
            yield return null;
        }

        // Attack 2: 1.4 seconds after start, will spawn at 2.5 seconds
        float attack2StartTime = Time.unscaledTime;
        float attack2Duration = 0.5f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[2], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[2], 0f));

        while (Time.unscaledTime < attack2StartTime + attack2Duration)
        {
            yield return null;
        }

        // Attack 3: 1.9 seconds after start, will spawn at 3 seconds
        float attack3StartTime = Time.unscaledTime;
        float attack3Duration = 0.4f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[1], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[1], 0f));

        while (Time.unscaledTime < attack3StartTime + attack3Duration)
        {
            yield return null;
        }

        // Attack 4: 2.3 seconds after start, will spawn at 3.4 seconds
        float attack4StartTime = Time.unscaledTime;
        float attack4Duration = 0.5f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[1], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[1], 0f));

        while (Time.unscaledTime < attack4StartTime + attack4Duration)
        {
            yield return null;
        }


        // Attack 5: 2.7 seconds after start, will spawn at 3.8 seconds
        float attack5StartTime = Time.unscaledTime;
        float attack5Duration = 1.1f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[2], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[2], 0f));

        while (Time.unscaledTime < attack5StartTime + attack5Duration)
        {
            yield return null;
        }


        //Move to other attack position. time here will be 3.8 when the moving starts
        float returnStartTime = Time.unscaledTime;
        float returnDuration = 1f;

        while (Time.unscaledTime < returnStartTime + returnDuration)
        {
            float t = (Time.unscaledTime - returnStartTime) / returnDuration;
            transform.position = Vector3.Lerp(attackPosition1, attackPosition2, t);
            yield return null;
        }

        transform.position = attackPosition2;

        //Attack 6: 4.8 seconds after start, will spawn at 5.3 
        // Warning and spikespawns
        float attack6StartTime = Time.unscaledTime;
        float attack6Duration = 3.3f;

        enemyAttacks.TentaclesInGround(4);
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 0f, 1));
        StartCoroutine(enemyAttacks.SummonSpike(_chrsAbleToTarget[0], 0.5f));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[3], 0.5f, 1));
        StartCoroutine(enemyAttacks.SummonSpike(_chrsAbleToTarget[3], 1f));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 1f, 1));
        StartCoroutine(enemyAttacks.SummonSpike(_chrsAbleToTarget[0], 1.5f));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[3], 1.5f, 1));
        StartCoroutine(enemyAttacks.SummonSpike(_chrsAbleToTarget[3], 2f));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[1], 2f, 1));
        StartCoroutine(enemyAttacks.SummonSpike(_chrsAbleToTarget[1], 2.5f));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[2], 2.5f, 1));
        StartCoroutine(enemyAttacks.SummonSpike(_chrsAbleToTarget[2], 3f));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 3f, 1));
        StartCoroutine(enemyAttacks.SummonSpike(_chrsAbleToTarget[0], 3.5f));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[3], 3.5f, 1));
        StartCoroutine(enemyAttacks.SummonSpike(_chrsAbleToTarget[3], 4f));

        while (Time.unscaledTime < attack6StartTime + attack6Duration)
        {
            yield return null;
        }

        // Attack 7: 8.3 seconds after start, will spawn at 9.4 seconds from here we repeat the first part
        float attack7StartTime = Time.unscaledTime;
        float attack7Duration = 0.4f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[0], 0f));

        while (Time.unscaledTime < attack7StartTime + attack7Duration)
        {
            yield return null;
        }

        // Attack 8:
        float attack8StartTime = Time.unscaledTime;
        float attack8Duration = 0.5f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[3], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[3], 0f));

        while (Time.unscaledTime < attack8StartTime + attack8Duration)
        {
            yield return null;
        }

        // Attack 9:
        float attack9StartTime = Time.unscaledTime;
        float attack9Duration = 0.4f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[0], 0f));

        while (Time.unscaledTime < attack9StartTime + attack9Duration)
        {
            yield return null;
        }

        // Attack 10:
        float attack10StartTime = Time.unscaledTime;
        float attack10Duration = 0.5f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[0], 0f));

        while (Time.unscaledTime < attack10StartTime + attack10Duration)
        {
            yield return null;
        }


        // Attack 11:
        float attack11StartTime = Time.unscaledTime;
        float attack11Duration = 1.4f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[3], 0.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[3], 0f));

        while (Time.unscaledTime < attack11StartTime + attack11Duration)
        {
            yield return null;
        }


        // End Turn
        StartCoroutine(enemyTurnEnded(0f));
    }

    // starter det 2 angrebs sekvens
    private IEnumerator ExecuteAttackPattern1()
    {
        Debug.Log("case 1 called");

        Vector3 attackPosition1 = new Vector3(startPosition.x-1.2f, startPosition.y+1f, 16f);

        // ryk modstanderen til attack positionen
        float moveStartTime = Time.unscaledTime;
        float moveDuration = 1f;

        while (Time.unscaledTime < moveStartTime + moveDuration)
        {
            float t = (Time.unscaledTime - moveStartTime) / moveDuration;
            transform.position = Vector3.Lerp(startPosition, attackPosition1, t);
            yield return null;
        }

        transform.position = attackPosition1;

        // Attack 1: trying to use the laser in row attack
        float attack1StartTime = Time.unscaledTime;
        float attack1Duration = 3.6f;

        //list of who is getting attacked in which order also warnings here for a easier time writing the code
        List<GameObject> newTargets = new List<GameObject>();
        
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[1],0.2f,1));
        newTargets.Add(_chrsAbleToTarget[1]);

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 0.6f, 1));
        newTargets.Add(_chrsAbleToTarget[0]);

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[2], 1f, 1));
        newTargets.Add(_chrsAbleToTarget[2]);

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 1.4f, 1));
        newTargets.Add(_chrsAbleToTarget[0]);

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[3], 1.8f, 1));
        newTargets.Add(_chrsAbleToTarget[3]);

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 2.2f, 1));
        newTargets.Add(_chrsAbleToTarget[0]);

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[1], 2.6f, 1));
        newTargets.Add(_chrsAbleToTarget[2]);

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 3f, 1));
        newTargets.Add(_chrsAbleToTarget[0]);

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 3.4f, 1));
        newTargets.Add(_chrsAbleToTarget[1]);

        enemyAttacks.LasersInRow(newTargets, 3.2f,0.4f);
        while (Time.unscaledTime < attack1StartTime + attack1Duration)
        {
            yield return null;
        }
        // Attack 2: the multiLaser
        float attack2StartTime = Time.unscaledTime;
        float attack2Duration = 1.2f;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 0f, 2));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[1], 0f, 2));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[2], 0f, 2));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[3], 0f, 2));
        enemyAttacks.MultiLaser(_chrsAbleToTarget);

        while (Time.unscaledTime < attack2StartTime + attack2Duration)
        {
            yield return null;
        }


        // Move to start position
        float moveBackTime = Time.unscaledTime;
        float moveBackDuration = 1f;

        while (Time.unscaledTime < moveBackTime + moveBackDuration)
        {
            float t = (Time.unscaledTime - moveBackTime) / moveBackDuration;  // Corrected
            transform.position = Vector3.Lerp(attackPosition1, startPosition, t);
            yield return null;
        }

        transform.position = startPosition;


        // End Turn
        StartCoroutine(enemyTurnEnded(0f));
    }

    // starter det 3 angrebs sekvens
    private IEnumerator ExecuteAttackPattern2()
    {
        Debug.Log("case 2 called");

        //should allready have this position but just to be sure
        Vector3 attackPosition1 = new Vector3(startPosition.x, startPosition.y, startPosition.z);

        // ryk modstanderen til attack positionen
        float moveStartTime = Time.unscaledTime;
        float moveDuration = 1f;

        while (Time.unscaledTime < moveStartTime + moveDuration)
        {
            float t = (Time.unscaledTime - moveStartTime) / moveDuration;
            transform.position = Vector3.Lerp(startPosition, attackPosition1, t);
            yield return null;
        }
        transform.position = attackPosition1;

        // Attack 1: Camera pans to see the enemy hit the wall
        float attack1StartTime = Time.unscaledTime;
        float attack1Duration = 3.8f;
        enemyAttacks.HitWall();

        while (Time.unscaledTime < attack1StartTime + attack1Duration)
        {
            yield return null;
        }
        // Attack 2: Rocks come falling down
        float attack2StartTime = Time.unscaledTime;
        float attack2Duration = 12f;

        enemyAttacks.RockFall(_chrsAbleToTarget[2],0f,260,8);
        enemyAttacks.RockFall(_chrsAbleToTarget[0], 0.5f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[1], 1f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[2], 1.5f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[0], 2f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[3], 2.5f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[1], 3f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[0], 3.5f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[0], 4f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[1], 5f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[3], 5f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[0], 6f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[0], 7f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[2], 8f, 260, 8);
        enemyAttacks.RockFall(_chrsAbleToTarget[2], 8.5f, 260, 8);

        while (Time.unscaledTime < attack2StartTime + attack2Duration)
        {
            yield return null;
        }
        // End Turn
        StartCoroutine(enemyTurnEnded(0f));
    }


    // starter det 4 angrebs sekvens
    private IEnumerator ExecuteAttackPattern3()
    {
        Debug.Log("case 3 called");

        //should allready have this position but just to be sure
        Vector3 attackPosition1 = new Vector3(startPosition.x, startPosition.y, startPosition.z);

        // ryk modstanderen til attack positionen ændre hvor lyset kommer fra i scenen ved at ændre directional light
        float moveStartTime = Time.unscaledTime;
        float moveDuration = 1f;
        enemyAttacks.ChangeLightingTo150();
        

        while (Time.unscaledTime < moveStartTime + moveDuration)
        {
            float t = (Time.unscaledTime - moveStartTime) / moveDuration;
            transform.position = Vector3.Lerp(startPosition, attackPosition1, t);
            yield return null; 
        }

        transform.position = attackPosition1;


        // Attack 1: play the animation to hit the wall
        float attack1StartTime = Time.unscaledTime;
        float attack1Duration = 5;
        enemyAttacks.HitWall();

        while (Time.unscaledTime < attack1StartTime + attack1Duration)
        {
            yield return null;
        }


        // Attack 2: also change camera angle
        float attack2StartTime = Time.unscaledTime;
        float attack2Duration = 1.7f;
        enemyAttacks.AdjustCamera();
        enemyAttacks.RockFall(_chrsAbleToTarget[0],0f,120,8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[1],0.5f,120,8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[2], 1f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[3], 1.5f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[2], 2f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[1], 2.5f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[0], 3f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[1], 2.5f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[2], 3f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[1], 3.5f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[0], 4f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[1], 4.5f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[2], 5f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[3], 5.5f, 120, 8f);
        enemyAttacks.RockFall(_chrsAbleToTarget[2], 6f, 120, 8f);
        while (Time.unscaledTime < attack2StartTime + attack2Duration)
        {
            yield return null;
        }

        // Attack 3: Summon Laser
        float attack3StartTime = Time.unscaledTime;
        float attack3Duration = 7;

        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 0f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[0],0f));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 3f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[0], 3f));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[0], 6.2f, 2));
        StartCoroutine(enemyAttacks.CreateWarning(_chrsAbleToTarget[3], 6.2f, 2));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[0], 6.2f));
        StartCoroutine(enemyAttacks.SingleLaser(_chrsAbleToTarget[3], 6.2f));

        while (Time.unscaledTime < attack3StartTime + attack3Duration)
        {
            yield return null;
        }

        // Attack 4: 
        float attack4StartTime = Time.unscaledTime;
        float attack4Duration = 2;
        enemyAttacks.AdjustCamera();

        while (Time.unscaledTime < attack4StartTime + attack4Duration)
        {
            yield return null;
        }
        enemyAttacks.ResetCamera();
        enemyAttacks.ChangeLightingTo150();

        // End Turn
        StartCoroutine(enemyTurnEnded(0f));
    }

}
