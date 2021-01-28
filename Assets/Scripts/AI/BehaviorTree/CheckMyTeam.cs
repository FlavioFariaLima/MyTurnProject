using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltraMare
{
    [TaskCategory("UltraMare")]
    [TaskIcon("Assets/Behavior Designer Tutorials/Tasks/Editor/{SkinColor}CanSeeObjectIcon.png")]
    public class CheckMyTeam : Action
    {
        [Tooltip("My Character Sheet")]
        public AICharacterController characterAI;

        [Tooltip("My Character Sheet")]
        public CharacterSheet characterSheet;

        public override void OnAwake()
        {
            characterSheet = GetComponent<CharacterSheet>();
            characterAI = GetComponent<AICharacterController>();

            foreach (Transform child in this.transform.parent.transform)
            {
                if (child != this.transform)
                    characterAI.myTeam.Add(child);
            }
        }
    }
}