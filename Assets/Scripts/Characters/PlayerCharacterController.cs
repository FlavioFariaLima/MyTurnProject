using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PlayerCharacterController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Character States")]
    public CharacterSheet character;
    private Inventory inventory;
    private CharacterHotbar hotbar;
    [SerializeField] private CharacterEquipment equipment;
    private GameObject selectedEffect;

    [Header("Movement and NavMesh")]
    private bool checkDetination;
    private NavMeshAgent characterAgent;
    private Vector3 destination;
    private Animator animator;
    private DropedItem destinationItem;
    private CraftStation destinationStation;
    private CharacterSheet destinationCreature;

    public float movementSofar;
    public Vector3 characterPosition;
    public float arriveDistance = 1.5f;

    public Animator CharacterAnimator
    {
        get { return animator; }
    }

    [Header("AI")]
    [SerializeField] private bool isAI;
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

    [Header("Actions")]
    [SerializeField] private bool selectState;
    private bool canAct;
    private bool canMove;
    private float hasMoved = 0;
    private bool canAttack;

    [Header("Mouse Interactions")]
    [SerializeField] private TextMeshPro infoName;
    private bool mouseIsOver;
    private bool isSelectedForUI = false;

    public CharacterEquipment Equipment()
    {
        if (equipment == null)
            equipment = GetComponent<CharacterEquipment>();

        return equipment;
    }

    public Inventory Inventory()
    {
        return inventory;
    }

    public CharacterHotbar Hotbar()
    {
        return hotbar;
    }

    public bool IsSelectedForUI()
    {
        return isSelectedForUI;
    }

    public void SelectedForUI(bool value)
    {
        isSelectedForUI = value;

        if (isSelectedForUI)
        {
            Global.UI.SelectCharacterForUI(this.character.GetId());

            foreach(CharacterSheet c in Global.Match.InGameCharacters())
            {
                if (c != character)
                {
                    c.controller.SelectedForUI(false);
                }
            }
        }
    }

    public bool IsSelectedForAct
    {
        get { return selectState; }
    }

    public void SelectedForAct(bool selected)
    {
        selectState = selected;

        if (selected)
        {
            Global.Commands.ClearSelectedCharacters(); // First we clear the list of selected characters
            Global.Commands.AddSelectedCharacter(this); // Then we add this character to the list
            Global.UI.selectedCharacterObject = this.gameObject;

            selectedEffect.SetActive(true);
            StartCoroutine(ShowReadyToActStuff());
        }
        else
        {
            selectedEffect.SetActive(false);
        }
    }

    public bool CanAct
    {
        get { return canAct; }
    }

    public bool CanMove
    {
        get { return canMove; }
    }

    public bool CheckMovement()
    {
        if (!Global.UI.characterSelectedToUI)
            Global.UI.characterSelectedToUI = this.character;

        Global.UI.UpdateCurrentCharacterInfo();

        if (hasMoved < character.GetMovement())
        {
            return true;
        }
        else
        {
            Global.UI.DisableActionBtn("Jump");
            return false;
        }
    }

    public float HasMoved
    {
        get { return hasMoved; }
    }

    public bool CheckAttacks()
    {
        if (character.NumberOfAttack < character.TotalAttacks)
        {
            canMove = true;
            canAttack = true;
            canShoot = true;
            return true;
        }
        else
        {
            canAttack = false;
            canShoot = false;
            Global.UI.DisableActionBtn("Melee");
            Global.UI.DisableActionBtn("Range");
            Global.Canvas.SetCursor(Global.Canvas.cursorDefault, false);
            return false;
        }
    }

    public NavMeshAgent CharacterAgent
    {
        get { return characterAgent; }
    }

    private void Awake()
    {
        AI = transform.GetComponent<AICharacterController>();
        cam = Camera.main;

        // Character
        character = GetComponent<CharacterSheet>();
        inventory = GetComponent<Inventory>();
        equipment = GetComponent<CharacterEquipment>();
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

    // Start is called before the first frame update
    void Start()
    {
      
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
    public void IsAi(bool ai)
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
                AI.StartAI(this, character);
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

    /// <summary>
    /// UI
    /// </summary>
    /// <returns></returns>

    private IEnumerator InfoUI()
    {
        while (mouseIsOver)
        {
            Global.UI.floatInfoPanel.SetActive(true);
            Global.UI.floatInfoPanel.transform.Find("infoText").GetComponent<TextMeshProUGUI>().text = character.GetName();

            yield return null;
        }

        Global.UI.floatInfoPanel.SetActive(false);
    }

    public IEnumerator ShowReadyToActStuff()
    {
        while (IsSelectedForAct)
        {
            //infoName.gameObject.SetActive(true);
            //infoName.color = Global.UI.actColor;
            //Global.FaceTextMeshToCamera(infoName.transform);

            yield return null;
        }

        //infoName.gameObject.SetActive(false);
        //infoName.color = Global.UI.defaultColor;

    }

    private IEnumerator PopFloatText(Transform parent, string popText, int colorSet)
    {
        if (popText == string.Empty)
            popText = "Miss!";

        float timeOfTravel = 1f; //time to object reach a target place 
        float currentTime = 0; // actual floting time 
        float normalizedValue;
        Color color_i = new Color();
        Color color_f = new Color();

        if (colorSet == 0)
        {
            color_i = Global.UI.dmgColor;
            color_f = Global.UI.dmgColor;
        }
        else if (colorSet == 1)
        {
            color_i = Global.UI.defaultColor;
            color_f = Global.UI.defaultColor;
        }

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

            Global.FaceTextMeshToCamera(floatText.transform);

            yield return null;
        }

        Destroy(floatText);
    }


    /// <summary>
    ///  Mouse Methods
    /// </summary>
    private void OnMouseUp()
    {
        if (!Global.Commands.playerIsAttacking && Global.Commands.GetSelectedCharacters()[0] != this)
        {
            if (Global.Match.playerTurn == Global.Match.MatchPlayers[this.character.GetPlayerId()]
                && Global.Match.MatchPlayers[this.character.GetPlayerId()].GetName != "DM")
            {
                //SetSelectState(true);
            }

            // Show Character Information
            Global.UI.UpdateCharacterInfoPanel(character);
            SelectedForUI(true);
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
                SelectedForUI(true);
            }

            Global.UI.characterSheet.controller.canAttack = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseIsOver = true;
        StartCoroutine(InfoUI());

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
        //StartCoroutine(InfoUI());

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
                        // Attack Creature
                    }
                }
            }
        }
    }

    public IEnumerator MoveToPosition(Vector3 dest)
    {
        if (character.isMyTurn && CheckMovement())
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

        CheckMovement();
    }

    public IEnumerator LookAtPosition(Transform target)
    {
        //float duration = 1f;
        //float elapsed = 0;
        //Vector3 direction = target.position - transform.position;
        //Quaternion toRotation = Quaternion.FromToRotation(transform.forward, direction);
        //toRotation.y = 0f;

        //float angle = Vector3.Angle((target.position - transform.forward), transform.forward);

        //if (angle > 15)
        //{
        //    while (elapsed < duration)
        //    {
        //        //angle = Vector3.Angle((target.position - transform.position), transform.forward);
        //        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, elapsed / duration);
        //        elapsed += Time.deltaTime;
        //        yield return null;
        //    }
        //}
        yield return null;
    }

    /// <summary>
    /// Combat
    /// </summary>
    /// <param name="dest"></param>
    /// <param name="enemy"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public IEnumerator MoveToAttack(Vector3 dest, CharacterSheet enemy, Actions.AttackType type)
    {
        if (true)
        {
            Debug.Log($"Move to Attack!");

            AllowToMove(true);
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
                StartCoroutine(PopFloatText(enemy.transform, $"{dmgResult[1]}", 0));

                //Debug.Log($"Pass AC!");
            }
            else
            {
                // Float Text
                StartCoroutine(PopFloatText(enemy.transform, string.Empty, 1));
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

            character.NumberOfAttack++;
        }

        CheckAttacks();
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

        bool attacked = false;

        while (!attacked && Global.Commands.attackType == Actions.AttackType.range && Global.Commands.playerIsAttacking && !isAI)
        {
            LaunchProjectile();
            RotateCharacter();

            yield return null;
        }

        IEnumerator HideLine()
        {
            Gradient lineRendererGradient = new Gradient();
            float fadeSpeed = 0.5f;
            float timeElapsed = 0f;
            float alpha = 1f;

            Global.Canvas.SetCursor(Global.Canvas.cursorDefault, false);

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

                if (Input.GetMouseButtonDown(0) && canShoot && canAct && !Global.Canvas.MouseIsOver)
                {
                    Rigidbody obj = Instantiate(projectile, shootPoint.position, transform.rotation);
                    obj.velocity = vo;

                    Vector3 v = obj.velocity;
                    float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
                    obj.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                    StartCoroutine(HideLine());
                }
                else if (Input.GetMouseButtonDown(0) && canShoot && canAct && Global.Canvas.MouseIsOver)
                {
                    StartCoroutine(HideLine());
                }
            }
        }

        // added final position argument to draw the last line node to the actual target
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

    /// <summary>
    /// Path Line and Path Calculations
    /// </summary>
    private IEnumerator ShowMovementPath()
    {
        while (CanAct && CheckMovement() && !Global.Commands.GetSelectedCharacters()[0].IsAi())
        {            

            Global.UI.pathLine.gameObject.SetActive(true);
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                Global.UI.distanceInfo.gameObject.SetActive(true);
                Global.UI.pathLine.enabled = true;

                if (Global.Commands.GetSelectedCharacters()[0].HasMoved >= Global.Commands.GetSelectedCharacters()[0].character.GetMovement())
                {
                    yield return null;
                }

                Global.UI.pathLine.startWidth = 0.02f;
                Global.UI.pathLine.endWidth = 0.02f;
                Global.UI.pathLine.colorGradient = Global.UI.gradient;

                Global.Commands.GetSelectedCharacters()[0].movementSofar = Global.Commands.GetSelectedCharacters()[0].HasMoved;
                NavMeshPath path = new NavMeshPath();
                NavMesh.CalculatePath(Global.Commands.GetMainSelectedCharacterTransform().position, hit.point, NavMesh.AllAreas, path);
                float distance = 0;
                float soFar = Global.UI.characterSheet.controller.HasMoved;
                Vector3 finalPoint = new Vector3();

                for (int i = 0; i < path.corners.Length - 1; i++) // Leave room to add 1
                {
                    float segmentDistance = (path.corners[i + 1] - path.corners[i]).magnitude;

                    if (Global.Commands.GetSelectedCharacters()[0].movementSofar + segmentDistance <= Global.Commands.GetSelectedCharacters()[0].character.GetMovement())
                    {
                        Global.Commands.GetSelectedCharacters()[0].movementSofar += segmentDistance;
                    }
                    else // Path length exceeds maxDist
                    {
                        finalPoint = path.corners[i] + ((path.corners[i + 1] - path.corners[i]).normalized * (Global.Commands.GetSelectedCharacters()[0].character.GetMovement() - Global.Commands.GetSelectedCharacters()[0].movementSofar));
                        distance = Vector3.Distance(Global.Commands.GetSelectedCharacters()[0].transform.position, finalPoint);
                        NavMesh.CalculatePath(Global.Commands.GetSelectedCharacters()[0].transform.position, finalPoint, NavMesh.AllAreas, path);
                        break;
                    }
                }

                if (path.corners.Length > 0)
                {
                    distance = Vector3.Distance(Global.Commands.GetSelectedCharacters()[0].transform.position, path.corners[path.corners.Length - 1]);

                    Global.UI.pathLine.positionCount = path.corners.Length;

                    for (int i = 0; i < path.corners.Length; i++)
                    {
                        Global.UI.pathLine.SetPosition(i, path.corners[i]);

                        float dist = Vector3.Distance(Global.UI.pathLine.GetPosition(0), Global.UI.pathLine.GetPosition(i));
                        if (dist > 9)
                        {
                            Global.UI.pathLine.SetPosition(i, Vector3.MoveTowards(Global.UI.pathLine.GetPosition(0), Global.UI.pathLine.GetPosition(i),
                                            Global.Commands.GetMainSelectedCharacterTransform().GetComponent<CharacterSheet>().GetMovement()));

                            yield return null;
                        }
                    }

                    if (Math.Round(distance, 2) > 0 && !Global.Commands.GetSelectedCharacters()[0].IsAi())
                        Global.UI.distanceInfo.text = Math.Round(distance, 1).ToString();

                    if (Global.UI.pathLine.positionCount > 0 && !Global.Commands.GetSelectedCharacters()[0].IsAi())
                        Global.UI.distanceInfo.rectTransform.position = Camera.main.WorldToScreenPoint(Global.UI.pathLine.GetPosition(Global.UI.pathLine.positionCount - 1));
                }
                else
                {
                    Global.UI.distanceInfo.text = string.Empty;
                }
            }

            yield return null;
        }

        Global.UI.pathLine.gameObject.SetActive(false);

        if (!Global.Commands.playerIsAttacking)
            Global.UI.distanceInfo.gameObject.SetActive(false);

        Global.UI.pathLine.enabled = false;
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

    /// <summary>
    /// Actions Menu
    /// </summary>
    /// <param name="has"></param>
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
            Global.Canvas.SetCursor(Global.Canvas.cursorMove, false);
            StartCoroutine(ShowMovementPath());
        }

        return true;
    }

    public bool AttackMelee()
    {        

        if (canAct && CheckAttacks())
        {
            StartCoroutine(LookAtMouseDirection());
            Global.Commands.playerIsAttacking = true;
            Global.Commands.attackType = Actions.AttackType.melee;

            if(!isAI)
            {
                Global.Canvas.SetCursor(Global.Canvas.cursorMelee, false);
            }
        }

        return true;
    }

    public bool AttackRange()
    {
       bool att = CheckAttacks();

        if (att && canAct)
        {
            canMove = false;
            Global.Commands.playerIsAttacking = true;
            Global.Commands.attackType = Actions.AttackType.range;
            StartCoroutine(ShowProjectilePath());

            cursor.gameObject.SetActive(true);
            lineVisual.gameObject.SetActive(true);

            if (!isAI)
            {
                Global.Canvas.SetCursor(Global.Canvas.cursorRange, false);
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
        hasMoved = 0;
        canMove = true;
        canAct = true;
        character.RestartStats();

        if (isAI && AI != null)
        {
            AI.CanAct(true);
        }
    }
}
