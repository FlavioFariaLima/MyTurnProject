using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltraMare
{
    [TaskCategory("UltraMare")]
    [TaskIcon("Assets/Behavior Designer Tutorials/Tasks/Editor/{SkinColor}CanSeeObjectIcon.png")]
    public class CheckTurn : Conditional
    {
        [Tooltip("My Character Sheet")]
        public AICharacterController characterAI;

        [Tooltip("My Character Sheet")]
        public CharacterSheet characterSheet;

        public override void OnAwake()
        {
            characterSheet = GetComponent<CharacterSheet>();
            characterAI = GetComponent<AICharacterController>();
        }

        public override TaskStatus OnUpdate()
        {
            if (characterSheet.isMyTurn)
            {
                if (characterAI.CanAct())
                {
                    // Return success if an object was found
                    return TaskStatus.Success;
                }
            }
            // An object is not within sight so return failure
            return TaskStatus.Failure;
        }
    }
}