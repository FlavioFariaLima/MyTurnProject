using UnityEngine;
using UnityEngine.AI;

namespace BehaviorDesigner.Runtime.Tasks.UltraMare
{
    [TaskCategory("UltraMare")]
    [TaskIcon("Assets/Behavior Designer Tutorials/Tasks/Editor/{SkinColor}CanSeeObjectIcon.png")]
    public class Engage : Action
    {
        protected PlayerCharacterController defaultController;
        protected AICharacterController characterAI;
        protected CharacterSheet characterSheet;
        protected NavMeshAgent navMeshAgent;

        public override void OnAwake()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            characterSheet = GetComponent<CharacterSheet>();
            characterAI = GetComponent<AICharacterController>();
            defaultController = GetComponent<PlayerCharacterController>();

            navMeshAgent.stoppingDistance = 0.5f;
        }

        /// <summary>
        /// Seek the destination. Return success once the agent has reached the destination.
        /// Return running if the agent hasn't reached the destination yet</summary>
        /// <returns></returns>
        public override void OnStart()
        {
            //navMeshAgent.speed = speed.Value;
            //navMeshAgent.angularSpeed = angularSpeed.Value;
            navMeshAgent.isStopped = false;

            if (characterAI != null)
            {
                defaultController.AllowToMove(true);

                var enemy = ClosestEnemyAlive();

                StartCoroutine(defaultController.MoveToAttack(enemy.transform.position, enemy, Actions.AttackType.melee));
            }
        }

        private CharacterSheet ClosestEnemyAlive()
        {
            CharacterSheet enemy = characterAI.knowEnemys[0].GetComponent<CharacterSheet>();
            float distance = Vector3.Distance(this.transform.position, enemy.transform.position);

            foreach (Transform e in characterAI.knowEnemys)
            {
                if (e.GetComponent<CharacterSheet>().IsAlive())
                {
                    if (Vector3.Distance(this.transform.position, e.position) < distance)
                    {
                        enemy = e.GetComponent<CharacterSheet>();
                        distance = Vector3.Distance(this.transform.position, e.position);
                    }
                }
            }

            return enemy;
        }

        // Seek the destination. Return success once the agent has reached the destination.
        // Return running if the agent hasn't reached the destination yet
        public override TaskStatus OnUpdate()
        {
            if (!characterAI.CanAct())
            {
                return TaskStatus.Success;
            }

            if (!defaultController.DestinationReached())
            {
                return TaskStatus.Running;
            }

            return TaskStatus.Success;
        }

        /// <summary>
        /// Stop pathfinding.
        /// </summary>
        private void Stop()
        {
            if (navMeshAgent.hasPath)
            {
                navMeshAgent.isStopped = true;
            }
        }

        /// <summary>
        /// The task has ended. Stop moving.
        /// </summary>
        public override void OnEnd()
        {
            Stop();
        }

        /// <summary>
        /// The behavior tree has ended. Stop moving.
        /// </summary>
        public override void OnBehaviorComplete()
        {
            Stop();
        }
    }
}