using System.Collections.Generic;
using UnityEngine;

namespace BehaviorDesigner.Runtime.Tasks.UltraMare
{
    [TaskCategory("UltraMare")]
    [TaskIcon("Assets/Behavior Designer Tutorials/Tasks/Editor/{SkinColor}CanSeeObjectIcon.png")]
    [SerializeField]
    public class LookForEnemies : Conditional
    {

        [Tooltip("My Character")]
        public PlayerCharacterController defaultController;
        public AICharacterController characterAI;
        public CharacterSheet characterSheet;


        [Tooltip("The object that we are searching for")]
        public SharedGameObject targetObject;
        [Tooltip("The field of view angle of the agent (in degrees)")]
        public SharedFloat fieldOfViewAngle = 90;
        [Tooltip("The distance that the agent can see")]
        public SharedFloat viewDistance = 1000;
        [Tooltip("The object that is within sight")]
        public List<Transform> returnedObject = new List<Transform>();

        public override void OnAwake()
        {
        }

        public override void OnStart()
        {
            characterSheet = transform.GetComponent<CharacterSheet>();
            characterAI = transform.GetComponent<AICharacterController>();
            defaultController = transform.GetComponent<PlayerCharacterController>();

            characterAI.UpdateMyknowledge();
        }

        /// <summary>
        /// Returns success if an object was found otherwise failure
        /// </summary>
        /// <returns></returns>
        public override TaskStatus OnUpdate()
        {
            if (characterAI.knowEnemys.Count > 0)
            {
                return TaskStatus.Success;
            }

            returnedObject = WithinSight(targetObject.Value, fieldOfViewAngle.Value, viewDistance.Value);
            if (returnedObject.Count > 0)
            {
                foreach (Transform e in returnedObject)
                {
                    // Return success if an object was found
                    if (!characterAI.knowEnemys.Contains(e.transform))
                    {
                        characterAI.knowEnemys.Add(e.transform);
                    }
                }

                Debug.Log($"Enemy!");
                return TaskStatus.Success;
            }

            // An object is not within sight so return failure
            return TaskStatus.Failure;
        }

        /// <summary>
        /// Determines if the targetObject is within sight of the transform.
        /// </summary>
        private List<Transform> WithinSight(GameObject targetObject, float fieldOfViewAngle, float viewDistance)
        {
            List<Transform> findEnemies = new List<Transform>();

            var allCharacters = Global.Match.InGameCharacters();

            if (allCharacters == null)
            {
                return null;
            }

            foreach (MatchCharacter enemy in allCharacters)
            {
                var direction = enemy.character.transform.position - this.transform.position;
                direction.y = 0;
                var angle = Vector3.Angle(direction, transform.forward);
                if (direction.magnitude < viewDistance && angle < fieldOfViewAngle * 0.5f)
                {
                    // The hit agent needs to be within view of the current agent
                    if (LineOfSight(enemy.character.gameObject) && enemy.character.GetPlayerId() != characterSheet.GetPlayerId())
                    {
                        if (!characterAI.myTeam.Contains(enemy.character.transform) && !characterAI.knowEnemys.Contains(enemy.character.transform))
                        {
                            findEnemies.Add(enemy.character.transform);
                        }
                    }
                }
            }

            return findEnemies;
        }

        /// <summary>
        /// Returns true if the target object is within the line of sight.
        /// </summary>
        private bool LineOfSight(GameObject targetObject)
        {
            RaycastHit hit;
            if (Physics.Linecast(defaultController.eyes.transform.position, targetObject.transform.position, out hit, defaultController.layer))
            {
                if (hit.transform.IsChildOf(targetObject.transform) || targetObject.transform.IsChildOf(hit.transform))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Draws the line of sight representation
        /// </summary>
        public override void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var oldColor = UnityEditor.Handles.color;
            var color = Color.yellow;
            color.a = 0.1f;
            UnityEditor.Handles.color = color;

            var halfFOV = fieldOfViewAngle.Value * 0.5f;
            var beginDirection = Quaternion.AngleAxis(-halfFOV, Vector3.up) * Owner.transform.forward;
            UnityEditor.Handles.DrawSolidArc(new Vector3(Owner.transform.position.x, Owner.transform.position.y + 1.3f, Owner.transform.position.z), Owner.transform.up, beginDirection, fieldOfViewAngle.Value, viewDistance.Value);

            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}