using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Jobs;
using System.ComponentModel;
using Unity.Collections;

[SerializeField]
public class AICharacterController : MonoBehaviour
{
    private PlayerCharacterController characterController;
    private CharacterSheet characterSheet;

    public int visionDistance = 30;
    public List<Transform> myTeam = new List<Transform>();
    public List<Transform> knowEnemys = new List<Transform>();
    private Transform targetEnemy;

    public bool canAct = true;
    private bool canEndTurn = true;

    public PlayerCharacterController CharacterController()
    {
        return characterController;
    }

    public void StartAI(PlayerCharacterController controller, CharacterSheet sheet)
    {
        if (this.characterController == null)
        {
            this.characterController = controller;
            this.characterSheet = sheet;
            characterController.IsAi(true);
        }

        Debug.Log($"{characterSheet.name} AI is Active!");
    }

    // Start is called before the first frame update
    void Awake()
    {
        //StartAI(GetComponent<PlayerCharacterController>(), GetComponent<CharacterSheet>());            
    }


    public void UpdateMyknowledge()
    {
        // Update Enemy List
        List<Transform> deadEnemies = new List<Transform>();

        foreach (Transform enemy in knowEnemys)
        {
            if (!enemy.GetComponent<CharacterSheet>().IsAlive())
            {
                deadEnemies.Add(enemy);
            }
        }

        foreach (Transform enemy in deadEnemies)
        {
            knowEnemys.Remove(enemy);
        }
    }

    // Get and Set
    public void CanAct(bool can)
    {
        canAct = can;
    }

    [SerializeField]
    public bool CanAct()
    {
        return canAct;
    }

    public void CanEndTurn(bool can)
    {
        canEndTurn = can;
    }

    public bool CanEndTurn()
    {
        return canEndTurn;
    }

    // AI Basic Turn Actions
    public void PassTurn()
    {
        Global.Match.TurnEnd();
        canEndTurn = false;
    }
}
