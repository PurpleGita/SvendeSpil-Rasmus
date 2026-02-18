using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Schema;
using TMPro;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class ChrsController : MonoBehaviour
{
    //bool der er true hvis det er spillerens tur
    public bool isPlayersTurn = true;

    //liste af karakterene, spefickt deres spille objekter i unity
    public List<GameObject> _chrGameObjects;

    //liste af aktive karakterer i kamp (bruges muligvis til logik adskilt fra gameobjects, da denne værdi er af Chr klasseb)
    public List<Chr> _chrActiveInCombat;

    //startpositionerne som karaktererne skal stå på
    [SerializeField]
    List<Vector3> _startPos;

    //UI objekt som indeholder angrebsvalg knapper
    [SerializeField]
    GameObject _attackOptions;

    //de tre angrebsvalg knapper
    [SerializeField]
    GameObject _attackOption1;

    [SerializeField]
    GameObject _attackOption2;

    [SerializeField]
    GameObject _attackOption3;

    [SerializeField]
    private GameObject _loseObject;

    [SerializeField]
    GameObject _pointObject;

    //boolean der fortæller om karaktererne står på deres startposition
    public bool ChrsInStartPosition;

    //liste over hvilke karakterer der skal flyttes
    List<GameObject> ChrsToMove = new List<GameObject>();

    //liste over hvor karaktererne skal flyttes hen
    List<Vector3> PositionToMove = new List<Vector3>();

    //boolean der aktiverer bevægelse af karakterer
    bool moveChr = false;

    //holder styr på hvilken tur det er (hvilken karakter der skal angribe)
    public int currentTurn = 0;

    //holder styr på antal ture der er gået
    public int turnTimer = 0;

    //holder styr på hvor godt de forkselige karaktere blockere
    public List<double> blockAccrucyList = new List<double>() { 0, 0, 0, 0 };

    private bool won = false;

    //update bliver kaldt hver frame
    void FixedUpdate()
    {
        //hvis det er spillerens tur, og karaktererne ikke er i startposition, så flyt dem dertil
        if (isPlayersTurn)
        {
            if (!ChrsInStartPosition)
            {
                moveChrsToStartPosition();
            }
        }

        //hvis der er karakterer der skal flyttes, så flyt dem
        if (moveChr)
        {
            moveChrsToPosition(ChrsToMove, PositionToMove);
        }

        for (int i = 0; i < blockAccrucyList.Count; i++)
        {
            if (blockAccrucyList[i] > 0)
            {
                blockAccrucyList[i] = blockAccrucyList[i] - 0.07;
            }
            else { blockAccrucyList[i] = 0; }
        }


        

    }

    private void Update()
    {
        HandleTouchInput();
    }

    //metode der flytter en eller flere af karaktererne hen til ønskede positioner
    private void moveChrsToPosition(List<GameObject> chrsToMove, List<Vector3> position)
    {
        for (int i = 0; i < chrsToMove.Count; i++)
        {

            //insert check to see if chr has to move more den 0.5f
            if (chrsToMove[i].transform.position != position[i])
            {

                // vend karakteren korrekt
                if (chrsToMove[i].transform.position.x > position[i].x)
                {
                    chrsToMove[i].gameObject.GetComponent<SpriteRenderer>().flipX = true;
                }
                else if (chrsToMove[i].transform.position.x < position[i].x)
                {
                    chrsToMove[i].gameObject.GetComponent<SpriteRenderer>().flipX = false;
                }

                
                
                //beregn afstand til destination
                float distanceToTarget = Vector3.Distance(chrsToMove[i].transform.position, position[i]);

                //bevæg karakteren hen mod destinationen og ændre deres animation, hvis det kan nås
                if(distanceToTarget > 1f){ chrsToMove[i].GetComponent<AnimationHandler>().Walk(); };
                //Debug.Log(distanceToTarget);

                chrsToMove[i].transform.position = Vector3.MoveTowards(chrsToMove[i].transform.position, position[i], 0.08f);
            }
            //hvis denne karaakter er stoppet med at bevæge sig så skift animation til idle
            else
            {
                bool _isChrHurt;
                if (chrsToMove[i].GetComponent<CombatChrInfo>()._currentHealth > 0) 
                { _isChrHurt = false; }
                else
                { _isChrHurt=true; }
                chrsToMove[i].GetComponent<AnimationHandler>().Idle(_isChrHurt);

            }
        }

        //hvis alle karakterer er nået frem, ryd lister og stop bevægelse
        if (ArePositionsEqual(chrsToMove, position))
        {
            //sikker at alle er tilbage til idle animation
            for (int i = 0; i < chrsToMove.Count; i++)
            {
                bool _isChrHurt;
                if (chrsToMove[i].GetComponent<CombatChrInfo>()._currentHealth > 0)
                { _isChrHurt = false; }
                else
                { _isChrHurt = true; }
                chrsToMove[i].GetComponent<AnimationHandler>().Idle(_isChrHurt);

                //peg mod modstanderen
                _chrGameObjects[i].gameObject.GetComponent<SpriteRenderer>().flipX = false;
            }

            moveChr = false;
            PositionToMove.Clear();
            chrsToMove.Clear();
        }
    }

    //sæt alle karakterer tilbage på deres startposition (bliver kaldt i starten af spillerens tur)
    private void moveChrsToStartPosition()
    {
        if (!moveChr)
        {
            ChrsToMove.Clear();

            //tjek hver karakter og se om den står forkert
            for (int i = 0; i < _chrGameObjects.Count; i++)
            {
                if (_chrGameObjects[i].transform.position != _startPos[i])
                {
                    ChrsToMove.Add(_chrGameObjects[i]);
                    PositionToMove.Add(_startPos[i]);
                    moveChr = true;
                }
            }
        }

        //hvis ingen karakterer skal flyttes, er alle i position
        if (!moveChr)
        {
            ChrsInStartPosition = true;

            //vis UI med angrebsvalg
            EnableAttackOptionsFirstTime();
        }
    }

    //tjek om alle karakterers position matcher deres målposition
    bool ArePositionsEqual(List<GameObject> players, List<Vector3> PositionsToCheck)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].transform.position != PositionsToCheck[i])
            {
                return false; // hvis én position ikke matcher, return false
            }
        }

        return true; // hvis alle positioner matcher, return true
    }

    //aktiver UI for at vælge angreb første gang på spillerens tur
    private void EnableAttackOptionsFirstTime()
    {
        _attackOptions.SetActive(true);

        //find den karakter der skal angribe og start dens tur
        passTurn(currentTurn);
    }

    //aktiver UI for at vælge angreb
    private void EnableAttackOptions(CombatChrInfo currentTurnsChr) 
    {


        //sæt ikoner for angrebsvalg baseret på den aktuelle karakters angreb og deres AP kost til at stemme over ens
        _attackOptions.SetActive(true);

        GameObject[] attackOptions = { _attackOption1, _attackOption2, _attackOption3 };

        for (int i = 0; i < attackOptions.Length; i++)
        {
            var attack = currentTurnsChr._equipedAttacks[i];
            var option = attackOptions[i];

            // Set image sprite
            option.GetComponent<Image>().sprite = attack._image;

            // Set AP text
            option.GetComponentInChildren<TextMeshProUGUI>().text = $"{attack.APCost}AP";

            // Set color based on AP
            option.GetComponent<Image>().color =
                (attack.APCost > currentTurnsChr._currentAP) ? Color.grey : Color.white;
        }


    }

    //starter tur for specifik karakter i rækken
    private void passTurn(int chrPosition)
    {
        //hvis alle karakterer har haft tur, giv turen til modstanderen
        if (chrPosition >= _chrGameObjects.Count)
        {
            isPlayersTurn = false;
            RemovePoints(1000);

            List<GameObject> Enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

            int enemyAttackToUse = turnTimer;
            if (turnTimer > 3)
            {
                System.Random rand = new System.Random();
                enemyAttackToUse = rand.Next(0, 3);
            }


            foreach (GameObject enemy in Enemies)
            {
                PositionToMove = enemy.GetComponent<EnemyPatternBehaviour>().EnemyTurnStarted(enemyAttackToUse);
            }
            // here bruger vi ToList() for ikke at assign den direkte liste til ChrsToMove da hvis den bliver cleared senere ville _chrGameObjects også blive cleared
            ChrsToMove = _chrGameObjects.ToList();
            moveChr = true;

        }
        else { 

            //skift karakterens animation til klar (mangler implementering og animationer)

            //flyt karakteren frem på skærmen for at gøre klar til at angribe
            ChrsToMove.Add(_chrGameObjects[chrPosition]);
            PositionToMove.Add(new Vector3(_chrGameObjects[chrPosition].transform.position.x, _chrGameObjects[chrPosition].transform.position.y, _chrGameObjects[chrPosition].transform.position.z - 1));

            moveChr = true;

            EnableAttackOptions(_chrGameObjects[chrPosition].GetComponent<CombatChrInfo>());
        }
    }

    //kaldes når spilleren trykker på en angrebs-knap
    public void AttackPressed(int attackPlayed)
    {
        
        Attack attack = _chrGameObjects[currentTurn].GetComponent<CombatChrInfo>()._equipedAttacks[attackPlayed];

        if (attack.APCost <= _chrGameObjects[currentTurn].GetComponent<CombatChrInfo>()._currentAP)
        {

            //sætter attack options til false så man ikke kan klikke flere gange på samme attack
            _attackOptions.SetActive(false);

            //betaler ap cost
            _chrGameObjects[currentTurn].GetComponent<CombatChrInfo>()._currentAP -= attack.APCost;

            //afspil angrebsanimation
            int atttackValue = attack.Value;
            _chrGameObjects[currentTurn].GetComponent<AnimationHandler>().Attack(atttackValue);

            //se hvor langt tid animationen tager
            double duration = attack.Duration;
            Debug.Log(duration);

            StartCoroutine(HandleAttackEffects((float)attack.EffectAt, attack));
            StartCoroutine(HandleEndOfTurn((float)duration));
        }

    }

    public void WaitPressed() 
    {
        _attackOptions.SetActive(false);
        _chrGameObjects[currentTurn].GetComponent<CombatChrInfo>()._currentAP += 2;
        StartCoroutine(HandleEndOfTurn(0));
    }

    //burde være sit eget script i det fulde spil
    private IEnumerator HandleAttackEffects(float waitTime,Attack attack)
    {
        yield return new WaitForSeconds(waitTime);

        //udfør angrebseffekt (mangler implementering)
        List<GameObject> Enemies = GameObject.FindGameObjectsWithTag("Enemy").ToList();

        //hvis angrebet gør skade så gør den mængde skade til modstanderne
        if(attack.DMG > 0) 
        { 
            foreach(GameObject enemy in Enemies) 
            { 
                enemy.GetComponent<EnemyInfo>()._currentHealth -= attack.DMG;
                if (enemy.GetComponent<EnemyInfo>()._currentHealth <= 0) 
                {
                    EnemyPatternBehaviour enemyPattern = enemy.GetComponent<EnemyPatternBehaviour>();
                    enemyPattern.StartCoroutine(enemyPattern.Defeated(2f));

                    _attackOptions.SetActive(false);
                    won = true;
                }
            }
        }

        if(attack.HPGain > 0) 
        { 
            foreach (GameObject chr in _chrGameObjects)
            { 
                chr.GetComponent<CombatChrInfo>()._currentHealth += attack.HPGain;
                if(chr.GetComponent<CombatChrInfo>()._currentHealth > chr.GetComponent<CombatChrInfo>()._maxHealth) 
                {
                    chr.GetComponent<CombatChrInfo>()._currentHealth = chr.GetComponent<CombatChrInfo>()._maxHealth;
                }
            }
        }

        if(attack.APGain > 0) 
        {
            foreach (GameObject chr in _chrGameObjects)
            {
                chr.GetComponent<CombatChrInfo>()._currentAP += attack.APGain;
                if (chr.GetComponent<CombatChrInfo>()._currentAP > chr.GetComponent<CombatChrInfo>()._maxAP)
                {
                    chr.GetComponent<CombatChrInfo>()._currentAP = chr.GetComponent<CombatChrInfo>()._maxAP;
                }
            }
        }



    }


    private IEnumerator HandleEndOfTurn(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        
        //stop attacking
        _chrGameObjects[currentTurn].GetComponent<AnimationHandler>().StopAttack();

        if (!won) {

        //move chr back to start posistion
        ChrsToMove.Add(_chrGameObjects[currentTurn]);
        PositionToMove.Add(_startPos[currentTurn]);
        moveChr = true;

        //forøg tur tælleren og start næste tur
        currentTurn++;
        passTurn(currentTurn);
        }
    }


    public void Attacked(int position,int attackDMG) 
    {
        Debug.Log(position + " is postion attacked out of " + _chrGameObjects.Count);
        if (_chrGameObjects[position].GetComponent<AnimationHandler>().CheckIfBlocking()) 
        {
            int roundedValue = Convert.ToInt32(Math.Round(blockAccrucyList[position]));


            _chrGameObjects[position].GetComponent<CombatChrInfo>()._currentAP += roundedValue/5;
            StartCoroutine(_chrGameObjects[position].GetComponent<AnimationHandler>().ParryAnimation());
            AddPoints(roundedValue*10);
            Debug.Log(roundedValue);
            Debug.Log(blockAccrucyList[position]);
        }
        else 
        {
            //hvis angrebet fjern points og hvis karakteren er under 0 liv tager de andre karaktere også skade.
            if(_chrGameObjects[position].GetComponent<CombatChrInfo>()._currentHealth <= 0) 
            {
                _chrGameObjects[position].GetComponent<AnimationHandler>().Hurt(true);
                for (int i = 0; i < _chrGameObjects.Count; i++)
                {
                    _chrGameObjects[i].GetComponent<CombatChrInfo>()._currentHealth -= attackDMG;
                }
                RemovePoints(attackDMG*100);
            }
            else 
            {
                _chrGameObjects[position].GetComponent<AnimationHandler>().Hurt(false);
                _chrGameObjects[position].GetComponent<CombatChrInfo>()._currentHealth -= attackDMG;
                RemovePoints(attackDMG * 100);
            }

            bool isSomeoneAlive = false;

            foreach (var chr in _chrGameObjects) 
            { 
                if(chr.GetComponent<CombatChrInfo>()._currentHealth > 0) {isSomeoneAlive = true;}
            }

            if (!isSomeoneAlive) 
            {
                Destroy(_attackOptions);
                _loseObject.SetActive(true);
            }

        }


        
    }

    private void tryToBlock(GameObject chrObject) 
    { 
        if (chrObject.GetComponent<AnimationHandler>().Block()) 
        {
            actuallyBlock(chrObject.GetComponent<CombatChrInfo>()._position);
         
        }
    }

    private void actuallyBlock(int chrPosition)
    {
        Debug.Log("actuallyBlocking");
        blockAccrucyList[chrPosition] = 35;

    }

    //skyder ene "laser" der hvor der bliver klikket og hvis den rammer noget der har tagget Chr (som karakterene) så prøver den at få den karakter til at blokke.
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            foreach (Touch touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Began)
                {
                    Ray ray = Camera.main.ScreenPointToRay(touch.position);
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 1.0f); // Visualize the ray
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {
                        if (hit.collider.CompareTag("Chr"))
                        {
                            Debug.Log("Touched: " + hit.collider.gameObject.name);
                            tryToBlock(hit.collider.gameObject);
                        }
                    }
                    else
                    {
                        Debug.Log("Ray did not hit any object.");
                    }
                }
            }
        }
    }

    //tilføjere points og så opdatere point scoreing i scenen
    private void AddPoints(int pointsToAdd)
    {
        int currentPoints = 0;
        if (int.TryParse(_pointObject.GetComponent<TextMeshProUGUI>().text, out currentPoints))
        {
            currentPoints = currentPoints + pointsToAdd;
            _pointObject.GetComponent<TextMeshProUGUI>().text = currentPoints.ToString();
        }
    }

    //fjerner points og så opdatere point scoreing i scenen
    private void RemovePoints(int pointsToRemove) 
    {
        int currentPoints = 0;
        if (int.TryParse(_pointObject.GetComponent<TextMeshProUGUI>().text, out currentPoints))
        {
            currentPoints = currentPoints - pointsToRemove;
            _pointObject.GetComponent<TextMeshProUGUI>().text = currentPoints.ToString();
        }


    }
}
