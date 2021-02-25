using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerCharacterController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Character States
    private MyParameters.TurnState turn;
    public CharacterSheet character;
    private Inventory inventory;
    private CharacterHotbar hotbar;
    private bool selectState;
    private GameObject selectedEffect;

    // Movement and NavMesh
    public float movementSofar;
    private bool checkDetination;
    private NavMeshAgent characterAgent;
    private Vector3 destination;
    private Animator animator;
    public Animator CharacterAnimator
    {
        get { return animator; }
    }

    private DropedItem destinationItem;
    private CraftStation destinationStation;
    private CharacterSheet destinationCreature;
    MyParameters.ObjectCategory destinationCategory;

    public Vector3 characterPosition;
    public float arriveDistance = 1.5f;

    // AI
    private bool isAI;
    private AICharacterController AI = null;

    [Header("ProjectilePath")]
    public Rigidbody projectile;
    public GameObject cursor;
    public Transform shootPoint;
    public LayerMask layer;
    public LineRenderer lineVisual;
    public int lineSegment = 10;
    public float flightTime = 10;
    private Camera cam;
    private bool canShoot = true;
    public GameObject eyes;

    // Actions
    private bool canAct;
    private bool canMove;
    private float hasMoved = 0;
    private bool canAttack;

    // Mouse Interactions
    private bool mouseIsOver;
    [SerializeField] private TextMeshPro infoName;

    public bool CanAct
    {
        get { return canAct; }
    }

    public bool CanMove
    {
        get { return canMove; }
    }

    public float HasMoved
    {
        get { return hasMoved; }
    }

    public NavMeshAgent CharacterAgent
    {
        get { return characterAgent; }
    }

    // Start is called before the first frame update
    void Start()
    {
        AI = transform.GetComponent<AICharacterController>();
        cam = Camera.main;

        // Character
        character = GetComponent<CharacterSheet>();
        inventory = GetComponent<Inventory>();
        hotbar = GetComponent<CharacterHotbar>();
        character.controller = this;
        gameObject.name = character.GetName();

        selectedEffect = transform.Find("_InteractionEffects").GetChild(0).gameObject;
        characterAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        animator.SetBool("isAlive", true);

        // Info Interface
        infoName.text = character.GetName();
        infoName.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // Update Character Position
        characterPosition = this.transform.position;

        // If has moved all possibel, then cannot move anymore
        if (hasMoved >= character.GetMovement())
            canMove = false;

        AnimatorManager();

        if (Global.Commands.playerIsAttacking)
        {
            if (Global.Commands.attackType == Actions.AttackType.range)
            {
            }
        }
    }

    // Selected State
    public bool GetSelectState()
    {
        return selectState;
    }

    public void SetSelectState(bool selected)
    {
        selectState = selected;

        if (selected)
        {
            Global.Commands.ClearSelectedCharacters(); // First we clear the list of selected characters
            Global.Commands.AddSelectedCharacter(this); // Then we add this character to the list

            Global.UI.selectedCharacterObject = this.gameObject;
            Global.UI.characterSheet = character;
            Global.UI.CharacterInventory = inventory;
            Global.UI.CharacterHotbar = hotbar;

            selectedEffect.SetActive(true);
        }
        else
        {
            selectedEffect.SetActive(false);
        }
    }

    // Animator and Animations
    private void AnimatorManager()
    {
        Vector3 s = characterAgent.transform.InverseTransformDirection(characterAgent.velocity).normalized;
        float turn = s.x;
        animator.SetFloat("speed", characterAgent.velocity.sqrMagnitude);
        animator.SetFloat("rotation", turn);
    }

    public void SetAnimatorDead()
    {
        animator.SetBool("isAlive", false);
    }

    public NavMeshAgent GetAgent()
    {
        return characterAgent;
    }

    public AICharacterController GetPlayerAI()
    {
        return AI;
    }

    public void FootL()
    {

    }

    public void FootR()
    {

    }

    // AI Control
    public void CharacterIsAI(bool ai)
    {
        isAI = ai;       
    }

    public bool IsAi()
    {
        return isAI;
    }

    public IEnumerator StartAI()
    {
        if (isAI)
        {
            if (AI == null)
            {
                AI = this.gameObject.GetComponent<AICharacterController>();
                AI.StartAITurn(this, character);
            }

            while (character.isMyTurn)
            {
                
                yield return null;
            }
        }
    }

    public bool DestinationReached()
    {
        // The path hasn't been computed yet if the path is pending.
        float remainingDistance;
        if (characterAgent.pathPending)
        {
            remainingDistance = float.PositiveInfinity;
        }
        else
        {
            remainingDistance = characterAgent.remainingDistance;
        }

        return remainingDistance <= arriveDistance;
    }

    // UI
    private IEnumerator InfoUI()
    {
        while (mouseIsOver)
        {
            infoName.gameObject.SetActive(true);
            FaceTextMeshToCamera(infoName.transform);

            yield return null;
        }

        infoName.gameObject.SetActive(false);
    }

    private IEnumerator PopFloatText(Transform parent, string atkText, string dmgText)
    {
        string popText = $"{atkText}";

        if (dmgText != string.Empty)
        {
            popText += $"({dmgText})";
        }

        float timeOfTravel = 1f; //time to object reach a target place 
        float currentTime = 0; // actual floting time 
        float normalizedValue;

        Color color_i = new Color(0.5f, 0, 0, 1);
        Color color_f = new Color(1, 1, 1, 1);

        Vector3 initialOffset = new Vector3(parent.position.x, 1, parent.position.z);
        Vector3 finalOffset = new Vector3(parent.position.x, 2.5f, parent.position.z);

        GameObject floatText = Instantiate(Global.UI.popupTextPrefab, new Vector3(parent.position.x, 1, parent.position.z), parent.rotation, parent);
        floatText.GetComponent<TextMeshPro>().text = popText;

        while (currentTime <= timeOfTravel)
        {
            currentTime += Time.deltaTime;
            normalizedValue = currentTime / timeOfTravel; // we normalize our time 

            //lerp factor is from 0 to 1, so we use (FadeExitTime-Time.time)/fadeDuration
            floatText.transform.position = Vector3.Lerp(initialOffset, finalOffset, normalizedValue);
            floatText.GetComponent<TextMeshPro>().color = Color.Lerp(color_i, color_f, normalizedValue);

            FaceTextMeshToCamera(floatText.transform);

            yield return null;
        }

        Destroy(floatText);
    }

    private void FaceTextMeshToCamera(Transform obj)
    {
        Vector3 origRot = obj.eulerAngles;

        obj.LookAt(obj.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);

        origRot.y = obj.eulerAngles.y;
        obj.eulerAngles = origRot;
    }

    // Mouse Methods
    private void OnMouseUp()
    {
        //if (isAI)
        //{
        //    selectedEnemy = this.character;
        //    StartCoroutine(Global.UI.LerpMoveObject(Global.UI.enemyInfo, this.character, MovePanelType.leftRight, true));

        //}

        if (!Global.Commands.playerIsAttacking && Global.Commands.GetSelectedCharacters()[0] != this)
        {
            if (Global.Match.playerTurn == Global.Match.MatchPlayers[this.character.GetPlayerId()]
                && Global.Match.MatchPlayers[this.character.GetPlayerId()].GetName != "DM")
            {
                SetSelectState(true);
            }

            // Show Character Information
            Global.UI.ShowCharacterInfo(character);
            Global.UI.SelectCharacterForUI(this.character.GetId());
        }
        else
        {
            if (Global.Commands.GetSelectedCharacters()[0].transform != this.character.transform)
            {
                Global.Commands.GetSelectedCharacters()[0].AttackEnemy(this.character, Global.Commands.attackType);
                Global.Commands.playerIsAttacking = false;

            }
            else
            {
                Global.UI.SelectCharacterForUI(this.character.GetId());
            }

            Global.UI.characterSheet.controller.canAttack = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsOver = true;
        StartCoroutine(InfoUI());
        transform.GetComponentInChildren<Renderer>().material.EnableKeyword("_EMISSION");

        if (Global.Commands.playerIsAttacking && !isAI && !selectState)
        {
            float distance = Vector3.Distance(Global.Commands.GetSelectedCharacters()[0].transform.position, this.transform.position);
            Global.UI.distanceInfo.text = $"{Math.Round(distance, 1)}";
            Global.UI.distanceInfo.rectTransform.position = Input.mousePosition;
            Global.UI.distanceInfo.gameObject.SetActive(true);
            canShoot = true;

            Debug.Log("Mouse Over Enemy!");
        }
        else
        {
            canShoot = false;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        mouseIsOver = false;
        StartCoroutine(InfoUI());
        transform.GetComponentInChildren<Renderer>().material.DisableKeyword("_EMISSION");

        if (Global.Commands.playerIsAttacking)
        {
            Global.UI.distanceInfo.gameObject.SetActive(false);
            canShoot = true;
        }
    }

    public IEnumerator LookAtMouseDirection()
    {
        while (canAttack)
        {
            Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            LayerMask myLayer = LayerMask.GetMask("Terrain", "Character", "Default");

            if (Physics.Raycast(camRay, out hit, 100f, myLayer))
            {
                Vector3 target = hit.point;
                target.y = 0;

                transform.LookAt(target, Vector3.up);

            }

            yield return null;
        }
    }

    // Locomotion Methods
    public IEnumerator MoveToObject(Transform dest, MyParameters.ObjectCategory category)
    {
        if (canMove)
        {
            destinationStation = null;
            destinationItem = null;
            destinationCreature = null;
            checkDetination = true;

            destination = dest.position;

            Global.UI.mainCamera.FollowCharacter(this.transform);

            if (category == MyParameters.ObjectCategory.Station)
            {
                destinationStation = dest.GetComponent<CraftStation>();
            }
            else if (category == MyParameters.ObjectCategory.Item)
            {

                destinationItem = dest.GetComponent<DropedItem>();
            }
            else if (category == MyParameters.ObjectCategory.Creature)
            {
                destinationCreature = dest.GetComponent<CharacterSheet>();
            }

            StartCoroutine(MoveToPosition(dest.position));

            while (characterAgent.hasPath)
            {
                yield return null;
            }

            if (checkDetination)
            {
                Debug.Log("test2");
                if (DestinationReached())
                {
                    if (destinationStation != null)
                    {
                        destinationStation.UsingStation(true);
                    }
                    else if (destinationItem != null)
                    {
                        destinationItem.TakeItem(Global.UI.CharacterInventory);
                    }
                    else if (destinationCreature != null)
                    {
                        //MoveToAttack(destinationCreature.transform.position, dest.GetComponent<CharacterSheet>(), Actions.AttackType.melee);
                    }
                }
            }
        }
    }

    public IEnumerator MoveToPosition(Vector3 dest)
    {
        if (character.isMyTurn && canMove)
        {
            movementSofar = hasMoved;
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(this.transform.position, dest, NavMesh.AllAreas, path);
            characterAgent.stoppingDistance = 0.2f;

            for (int i = 0; i < path.corners.Length - 1; i++) // Leave room to add 1
            {
                float segmentDistance = (path.corners[i + 1] - path.corners[i]).magnitude;

                if (movementSofar + segmentDistance <= character.GetMovement())
                {
                    movementSofar += segmentDistance;
                }
                else // Path length exceeds maxDist
                {
                    Vector3 finalPoint = path.corners[i] + ((path.corners[i + 1] - path.corners[i]).normalized * (character.GetMovement() - movementSofar));
                    NavMesh.CalculatePath(transform.position, finalPoint, NavMesh.AllAreas, path);
                    break;
                }
            }

            float distance = Vector3.Distance(characterAgent.transform.position, path.corners[path.corners.Length - 1]);

            if (hasMoved + distance <= character.GetMovement())
            {
                characterAgent.SetPath(path);
                hasMoved += distance;
                //Global.UI.mainCamera.FollowCharacter(this.transform);
            }

            Debug.Log($"Move to: {dest}");

            while (!DestinationReached())
            {
               
                yield return null;
            }
        }
    }

    public IEnumerator LookAtPosition(Transform target)
    {
        float duration = 1f;
        float elapsed = 0;
        Vector3 direction = target.position - transform.position;
        Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
        toRotation.y = 0f;

        float angle = Vector3.Angle((target.position - transform.position), transform.forward);

        if (angle > 15)
        {
            while (elapsed < duration)
            {
                //angle = Vector3.Angle((target.position - transform.position), transform.forward);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, elapsed / duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
        }
    }

    // Combat
    public IEnumerator MoveToAttack(Vector3 dest, CharacterSheet enemy, Actions.AttackType type)
    {
        if (true)
        {
            Debug.Log($"Move to Attack!");

            AllowToMove(true);
            canMove = true;
            StartCoroutine(MoveToPosition(dest));
            characterAgent.stoppingDistance = 1.5f;
            //float angle = 10;

            while (!DestinationReached())
            {
                if (isAI)
                {
                    transform.LookAt(enemy.transform);
                }

                yield return null;
            }

            Debug.Log("Chegou!");
            float dist = Vector3.Distance(this.transform.position, enemy.transform.position);
            Debug.Log($"{dist}");

            if (type == Actions.AttackType.melee && dist <= character.GetMeleeDistance())
            {
                AttackEnemy(enemy, type);
                Debug.Log("Melee Attack!");
            }
            else if (type == Actions.AttackType.range && dist <= character.GetRangeDistance())
            {
                AttackEnemy(enemy, type);
                Debug.Log("Range Attack!");
            }

            AllowToMove(false);
            canMove = false;
        }
    }

    public void AttackEnemy(CharacterSheet enemy, Actions.AttackType type)
    {
        if (canAct)
        {
            // Attack Dices
            int[] attackResult = new int[2];
            int[] dmgResult = new int[2];

            string bonusSymbolAttack = "+";
            string bonusSymbolDmg = "+";
            string attackConclusion = "Missed Attack";
            string hasDamage = "";

            StartCoroutine(LookAtPosition(enemy.transform));

            if (type == Actions.AttackType.melee)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) >= character.GetMeleeDistance())
                {
                    StartCoroutine(MoveToAttack(enemy.transform.position, enemy, type));
                    return;
                }

                attackResult = Dices.RollDices(20, character.GetMeleeAttack());
                dmgResult = Dices.RollDices(character.GetMeleeDamage(), character.GetAbilities().strength[1]);
            }
            else if (type == Actions.AttackType.range)
            {
                if (Vector3.Distance(transform.position, enemy.transform.position) >= character.GetRangeDistance())
                {
                    StartCoroutine(MoveToAttack(enemy.transform.position, enemy, type));
                    return;
                }

                attackResult = Dices.RollDices(20, character.GetRangeAttack());
                dmgResult = Dices.RollDices(character.GetRangeDamage(), character.GetAbilities().dexterity[1]);
            }

            if (attackResult[3] < 0)
                bonusSymbolAttack = "-";

            if (dmgResult[3] < 0)
                bonusSymbolDmg = "-";

            if (enemy.GetArmour() <= attackResult[1])
            {
                enemy.TakeDamage(dmgResult[1], character);
                attackConclusion = "Hit Attack";
                hasDamage = $"Damage Roll: 1d{dmgResult[2]} ({dmgResult[0]} {bonusSymbolDmg} {dmgResult[3]}) = {dmgResult[1]}";

                // Float Text
                StartCoroutine(PopFloatText(enemy.transform, $"{attackResult[1]}", $"{dmgResult[1]}"));

                //Debug.Log($"Pass AC!");
            }
            else
            {
                // Float Text
                StartCoroutine(PopFloatText(enemy.transform, $"{attackResult[1]}", string.Empty));
            }

            // Enemy Now Know Your Position
            if (enemy.controller.IsAi())
            {
                if (!enemy.controller.GetPlayerAI().knowEnemys.Contains(character.transform))
                {
                    enemy.controller.GetPlayerAI().knowEnemys.Add(character.transform);
                }
            }

            // Update Actions Log
            StopCoroutine(Global.UI.AddPlayerLogInput($"<color=green>{character.GetName()} <color=#ffffff>{attackConclusion} <color=red>{enemy.GetName()}(<color=#ffffff>AC {enemy.GetArmour()})",
                                                       $"<color=lightblue>Attack Roll: 1d20({attackResult[0]} {bonusSymbolAttack} {attackResult[3]}) = {attackResult[1]}",
                                                       $"<color=red>{hasDamage}"));
            StartCoroutine(Global.UI.AddPlayerLogInput($"<color=green>{character.GetName()} <color=#ffffff>{attackConclusion} <color=red>{enemy.GetName()}(<color=#ffffff>AC {enemy.GetArmour()})",
                                                       $"<color=lightblue>Attack Roll: 1d20({attackResult[0]} {bonusSymbolAttack} {attackResult[3]}) = {attackResult[1]}",
                                                       $"<color=red>{hasDamage}"));

            canAct = false;
        }
    }

    public IEnumerator ShowProjectilePath()
    {
        lineVisual.positionCount = lineSegment + 1;

        Gradient restartAlpha = new Gradient();
        restartAlpha.SetKeys
        (
            lineVisual.colorGradient.colorKeys,
            new GradientAlphaKey[] { new GradientAlphaKey(1, 1f) }
        );
        lineVisual.colorGradient = restartAlpha;

        while (Global.Commands.attackType == Actions.AttackType.range && Global.Commands.playerIsAttacking && !isAI)
        {
            LaunchProjectile();
            RotateCharacter();

            Debug.Log("Projectile...");
            yield return null;
        }

        IEnumerator HideLine()
        {
            Gradient lineRendererGradient = new Gradient();
            float fadeSpeed = 0.5f;
            float timeElapsed = 0f;
            float alpha = 1f;

            Global.UI.SetCursor(Global.UI.cursorDefault, false);

            while (timeElapsed < fadeSpeed)
            {
                alpha = Mathf.Lerp(1f, 0f, timeElapsed / fadeSpeed);

                lineRendererGradient.SetKeys
                (
                    lineVisual.colorGradient.colorKeys,
                    new GradientAlphaKey[] { new GradientAlphaKey(alpha, 1f) }
                );
                lineVisual.colorGradient = lineRendererGradient;

                timeElapsed += Time.deltaTime;
                yield return null;
            }

            HasAction(false);
            lineVisual.gameObject.SetActive(false);
            cursor.gameObject.SetActive(false);
        }

        void LaunchProjectile()
        {
            Ray camRay = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(camRay, out hit, 100f, layer))
            {
                //cursor.SetActive(true);
                cursor.transform.position = hit.point + Vector3.up * 0.1f;

                Vector3 vo = CalculateVelocty(hit.point, shootPoint.position, flightTime);

                Visualize(vo, cursor.transform.position); //we include the cursor position as the final nodes for the line visual position

                shootPoint.transform.rotation = Quaternion.LookRotation(vo);

                if (Input.GetMouseButtonDown(0) && canShoot && canAct && !Global.CanvasManager.MouseIsOver)
                {
                    Rigidbody obj = Instantiate(projectile, shootPoint.position, transform.rotation);
                    obj.velocity = vo;

                    Vector3 v = obj.velocity;
                    float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                    obj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    StartCoroutine(HideLine());
                }
                else if (Input.GetMouseButtonDown(0) && canShoot && canAct && Global.CanvasManager.MouseIsOver)
                {
                    StartCoroutine(HideLine());
                }
            }
        }

        //added final position argument to draw the last line node to the actual target
        void Visualize(Vector3 vo, Vector3 finalPos)
        {
            for (int i = 0; i < lineSegment; i++)
            {
                Vector3 pos = CalculatePosInTime(vo, (i / (float)lineSegment) * flightTime);
                lineVisual.SetPosition(i, pos);
            }

            lineVisual.SetPosition(lineSegment, finalPos);
        }

        Vector3 CalculateVelocty(Vector3 target, Vector3 origin, float time)
        {
            Vector3 distance = target - origin;
            Vector3 distanceXz = distance;
            distanceXz.y = 0f;

            float sY = distance.y;
            float sXz = distanceXz.magnitude;

            float Vxz = sXz / time;
            float Vy = (sY / time) + (0.5f * Mathf.Abs(Physics.gravity.y) * time);

            Vector3 result = distanceXz.normalized;
            result *= Vxz;
            result.y = Vy;

            return result;
        }

        Vector3 CalculatePosInTime(Vector3 vo, float time)
        {
            Vector3 Vxz = vo;
            Vxz.y = 0f;

            Vector3 result = shootPoint.position + vo * time;
            float sY = (-0.5f * Mathf.Abs(Physics.gravity.y) * (time * time)) + (vo.y * time) + shootPoint.position.y;

            result.y = sY;

            return result;
        }
    }

    public void RotateCharacter()
    {
        Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float rayLength;

        if (groundPlane.Raycast(cameraRay, out rayLength))
        {
            Vector3 pointToLook = cameraRay.GetPoint(rayLength);
            Debug.DrawLine(cameraRay.origin, pointToLook, Color.cyan);

            transform.LookAt(new Vector3(pointToLook.x, transform.position.y, pointToLook.z));
        }
    }

    // Actions Menu
    public void HasAction(bool has)
    {
        if (has)
        {
            canAct = true;
        }
        else
        {
            canAct = false;
            canShoot = false;
        }
    }

    public bool AllowToMove(bool allow)
    {
        canMove = allow;
        Global.Commands.playerIsAttacking = false;

        if (allow && !isAI)
        {
            Global.UI.SetCursor(Global.UI.cursorMove, false);
        }

        return true;
    }

    public bool AttackMelee()
    {
        canMove = false;
        canAttack = true;
        StartCoroutine(LookAtMouseDirection());

        if (canAct)
        {
            Global.Commands.playerIsAttacking = true;
            Global.Commands.attackType = Actions.AttackType.melee;

            if(!isAI)
            {
                Global.UI.SetCursor(Global.UI.cursorMelee, false);
            }
        }

        return true;
    }

    public bool AttackRange()
    {
        if (canAct)
        {
            canMove = false;
            canShoot = true;

            Global.Commands.playerIsAttacking = true;
            Global.Commands.attackType = Actions.AttackType.range;
            StartCoroutine(ShowProjectilePath());

            cursor.gameObject.SetActive(true);
            lineVisual.gameObject.SetActive(true);

            if (!isAI)
            {
                Global.UI.SetCursor(Global.UI.cursorRange, false);
            }
        }
        return true;
    }

    public bool Other()
    {
        Global.Commands.playerIsAttacking = false;
        return true;
    }

    public void ResetActions()
    {
        canMove = false;
        hasMoved = 0;
        canAct = true;

        if (isAI && AI != null)
        {
            AI.CanAct(true);
        }
    }
}
