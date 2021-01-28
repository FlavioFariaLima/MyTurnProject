using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltraMare
{
    [TaskCategory("UltraMare")]
    [TaskIcon("Assets/Behavior Designer Tutorials/Tasks/Editor/{SkinColor}CanSeeObjectIcon.png")]
    public class PassTurn : Action
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

        public override void OnStart()
        {

            characterAI.PassTurn();
        }


        public override void OnBehaviorComplete()
        {

        }
    }
}