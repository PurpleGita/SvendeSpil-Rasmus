
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyAttacks : MonoBehaviour
{
    [SerializeField]
    BossAnimationHandler _bossAnimator;

    [SerializeField]
    GameObject _laserObject;

    [SerializeField]
    GameObject _rockObject;

    [SerializeField]
    GameObject _spikeObject;

    [SerializeField]
    private Vector2 laserSpawnOffset = Vector2.zero;

    [SerializeField]
    private List<GameObject> _ChrsAbleToTarget;

    [SerializeField]
    float _offsetLaser;

    [SerializeField]
    Camera _camera;

    [SerializeField]
    List<GameObject> _warningObjects;

    private int framesToLiveLaser = 100;

    [SerializeField]
    Light directionalLight;

    public float cameraSpeed = 2;

    public float rotationSpeed = 15f;

    private Coroutine rotateTo150Coroutine;

    private Coroutine rotateTo30Coroutine;

    private Coroutine adjustCameraCoroutine;

    private Coroutine resetCameraCoroutine;

    private void Start()
    {
        _bossAnimator = GetComponent<BossAnimationHandler>();

        /*
        StartCoroutine(SummonSpike(_ChrsAbleToTarget[0], 2f));
        StartCoroutine(SummonSpike(_ChrsAbleToTarget[1], 2.5f));
        StartCoroutine(SummonSpike(_ChrsAbleToTarget[2], 3f));
        StartCoroutine(SummonSpike(_ChrsAbleToTarget[3], 3.5f));
        StartCoroutine(SummonSpike(_ChrsAbleToTarget[0], 4f));
        StartCoroutine(SummonSpike(_ChrsAbleToTarget[1], 5f));
        StartCoroutine(SummonSpike(_ChrsAbleToTarget[2], 6f));
        StartCoroutine(SummonSpike(_ChrsAbleToTarget[3], 7f));
        */
    }

    void Update()
    {
        //just here to test attacks
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CreateWarning(_ChrsAbleToTarget[1], 0f, 1));
            StartCoroutine(SingleLaser(_ChrsAbleToTarget[1],1));
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            MultiLaser(_ChrsAbleToTarget);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            List<GameObject> testTargets = new List<GameObject>();
            testTargets.Add(_ChrsAbleToTarget[0]);
            testTargets.Add(_ChrsAbleToTarget[2]);
            testTargets.Add(_ChrsAbleToTarget[0]);
            testTargets.Add(_ChrsAbleToTarget[3]);
            LasersInRow(testTargets, 2.5f,0.4f);
        }


        if (Input.GetKeyDown(KeyCode.K))
        {
            //turn Camera
            Vector3 targetPosition = new Vector3(1.26f, 9.64f, -2.22f);
            SmoothTransition(targetPosition, 1f, 1.4f); // 1 second to move, hold for 1.4 seconds


            HitWall();



        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            RockFall(_ChrsAbleToTarget[0],1,300,9f);
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            TentaclesInGround(5f);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(SummonSpike(_ChrsAbleToTarget[0],1f));
            StartCoroutine(SummonSpike(_ChrsAbleToTarget[1], 1.5f));
            StartCoroutine(SummonSpike(_ChrsAbleToTarget[2], 2f));
            StartCoroutine(SummonSpike(_ChrsAbleToTarget[3], 2.5f));
        }
    }

    public IEnumerator SingleLaser(GameObject targetPosition,float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Debug.Log("SingleLaser activated!");
        _bossAnimator.PlayLaserAnimation();

        float waitTimeForSpawningLaser = 1.1f;

        StartCoroutine(SpawnLaser(targetPosition, waitTimeForSpawningLaser, framesToLiveLaser));
    }

    public void MultiLaser(List<GameObject> targets)
    {
        Debug.Log("MultiLaser activated!");
        _bossAnimator.PlayLaserAnimation();

        float waitTime = 1.1f;
        int framesToLive = 100;

        //add charge effect (not implemented)


        foreach (GameObject target in targets) 
        {
            StartCoroutine(SpawnLaser(target, waitTime, framesToLive));
        }

    }

    public void LasersInRow(List<GameObject> targets,float laserWindowTime,float timeBetweenLasers)
    {
        
        float waitTime = 1.1f;

        Debug.Log("LasersInRow activated!");
        _bossAnimator.PlayLasersInRow(laserWindowTime,timeBetweenLasers,targets.Count);
        
        int lasernumber = 0;

        foreach (GameObject target in targets) 
        {
            StartCoroutine(SpawnLaser(target, waitTime+(lasernumber*timeBetweenLasers), framesToLiveLaser));
            lasernumber++;
        }
    }

    public void HitWall()
    {
        //turn Camera
        Vector3 targetPosition = new Vector3(1.26f, 9.64f, -2.22f);
        SmoothTransition(targetPosition, 1f, 1.4f); // 1 second to move, hold for 1.4 seconds
        _bossAnimator.PlayHitWallAnimation();
    }

    public void RockFall(GameObject gameObject, float waitTime, int framesToLive, float height)
    {
        StartCoroutine(SpawnRock(gameObject,waitTime,framesToLive,height));
        Debug.Log("spawning rock");
    }

    public void TentaclesInGround(float holdTime)
    {
        _bossAnimator.PlaySpikesAnimation(holdTime);
    }

    public IEnumerator SummonSpike(GameObject targetObject,float waitTime) 
    {
        Debug.Log("summon spike");
        yield return new WaitForSeconds(waitTime);
        Vector3 spawnPosition = new Vector3(
            targetObject.transform.position.x,
            -0.5f,
            targetObject.transform.position.z
        );
        GameObject spikeSpawned = Instantiate(_spikeObject, spawnPosition, Quaternion.identity);
        spikeSpawned.GetComponent<SpikeBehaviour>().goneup = false;

        yield return new WaitForSeconds(120f / 60f);

        Destroy(spikeSpawned);
    }
    
    public IEnumerator SpawnRock (GameObject targetObject, float waitTime, int framesToLive, float height) 
    {
        //få transformen af targeted
        yield return new WaitForSeconds(waitTime);
        Vector3 spawnPosition = new Vector3(
            targetObject.transform.position.x,
            targetObject.transform.position.y+height,
            targetObject.transform.position.z
           );

        //ændre rotatationen af rock
        Quaternion initialRotation = Quaternion.Euler(0, 0, Random.Range(0f, 360f));
        Debug.Log("Spawned rock with a random Z rotation: " + initialRotation.eulerAngles.z);
        GameObject rockspawned = Instantiate(_rockObject,spawnPosition,initialRotation);
        rockspawned.GetComponent<RockBehaviour>().Fall = true;

        yield return new WaitForSeconds(framesToLive / 60f);

        Destroy(rockspawned);
    }

    private IEnumerator SpawnLaser(GameObject targetObject, float waitTime, int framesToLive)
    {
        yield return new WaitForSeconds(waitTime);

        // Calculate spawn position with offset
        Vector3 spawnPosition = new Vector3(
            transform.position.x + laserSpawnOffset.x,
            transform.position.y + laserSpawnOffset.y,
            transform.position.z-0.2f // Keep the z-axis the same to avoid misalignment but put it closer to the camera then the boss so it is visable
        );

        // Get the target position
        Vector2 targetPosition = new Vector2(targetObject.transform.position.x, targetObject.transform.position.y - (targetObject.transform.position.y /_offsetLaser));

        // Calculate direction and rotation
        Vector2 direction = targetPosition - (Vector2)spawnPosition;

        // Calculate angle based on direction
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Instantiate the laser
        GameObject laserInstance = Instantiate(_laserObject, spawnPosition, Quaternion.identity);

        // Rotate the laser to point towards the target
        laserInstance.transform.rotation = Quaternion.Euler(0, 0, angle);

        // Start the scaling coroutine for the laser
        StartCoroutine(_bossAnimator.AnimateLaserScale(laserInstance.transform, framesToLive));

        yield return new WaitForSeconds(framesToLive / 60f);

        // Destroy the laser after the duration
        Destroy(laserInstance);
    }


    public void SmoothTransition(Vector3 targetPosition, float duration, float holdTime)
    {
        StartCoroutine(SmoothTransitionRoutine(targetPosition, duration, holdTime));
    }

    private IEnumerator SmoothTransitionRoutine(Vector3 targetPosition, float duration, float holdTime)
    {
        Vector3 startPosition = _camera.transform.position;
        float elapsedTime = 0f;

        // Move to the target position
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            _camera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        //_camera.transform.position = targetPosition;

        // Hold at the target position
        yield return new WaitForSeconds(holdTime);

        // Move back to the original position
        elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            _camera.transform.position = Vector3.Lerp(targetPosition, startPosition, t);
            yield return null;
        }

        _camera.transform.position = startPosition;
    }

    public IEnumerator CreateWarning(GameObject targetObject, float waitTime,int intensity) 
    {
        yield return new WaitForSeconds(waitTime);

        // Get the target position
        Vector3 spawnPosition = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y + 1.2f,targetObject.transform.position.z);

        GameObject warningInstance = Instantiate(_warningObjects[intensity], spawnPosition, Quaternion.identity);
        warningInstance.active = true;

        yield return new WaitForSeconds(1f);

        Destroy(warningInstance);
    }

    public void ChangeLightingTo150()
    {
        if (rotateTo30Coroutine != null) StopCoroutine(rotateTo30Coroutine);
        if (rotateTo150Coroutine != null) StopCoroutine(rotateTo150Coroutine);
        Debug.Log("rotating to 150");

        rotateTo150Coroutine = StartCoroutine(RotateLight(150f));
    }

    public void ChangeLightingTo30()
    {
        if (rotateTo150Coroutine != null) StopCoroutine(rotateTo150Coroutine);
        if (rotateTo30Coroutine != null) StopCoroutine(rotateTo30Coroutine);

        rotateTo30Coroutine = StartCoroutine(RotateLight(30f));
    }

    private IEnumerator RotateLight(float targetXRotation)
    {
        Quaternion startRotation = directionalLight.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetXRotation, startRotation.eulerAngles.y, startRotation.eulerAngles.z);

        while (Quaternion.Angle(directionalLight.transform.rotation, targetRotation) > 0.1f)
        {
            directionalLight.transform.rotation = Quaternion.RotateTowards(directionalLight.transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }

    public void AdjustCamera()
    {
        if (adjustCameraCoroutine != null) StopCoroutine(adjustCameraCoroutine);
        adjustCameraCoroutine = StartCoroutine(RotateAndMoveCamera(11f, 7f));
    }

    public void ResetCamera()
    {
        if (adjustCameraCoroutine != null) StopCoroutine(adjustCameraCoroutine);
        if (resetCameraCoroutine != null) StopCoroutine(resetCameraCoroutine);

        resetCameraCoroutine = StartCoroutine(RotateAndMoveCamera(15f, 8.1f));
    }

    private IEnumerator RotateAndMoveCamera(float targetXRotation, float targetYPosition)
    {
        Quaternion startRotation = _camera.transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(targetXRotation, startRotation.eulerAngles.y, startRotation.eulerAngles.z);
        Vector3 startPosition = _camera.transform.position;
        Vector3 targetPosition = new Vector3(startPosition.x, targetYPosition, startPosition.z);

        while (Quaternion.Angle(_camera.transform.rotation, targetRotation) > 0.1f || Mathf.Abs(_camera.transform.position.y - targetYPosition) > 0.1f)
        {
            _camera.transform.rotation = Quaternion.RotateTowards(_camera.transform.rotation, targetRotation, cameraSpeed * Time.deltaTime);
            _camera.transform.position = Vector3.MoveTowards(_camera.transform.position, targetPosition, cameraSpeed * Time.deltaTime);
            yield return null;
        }
    }

}