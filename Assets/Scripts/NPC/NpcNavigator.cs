using UnityEngine;
using UnityEngine.AI;

public class NpcNavigator : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        SetRandomDestination();
    }

    private void Update()
    {
        // Check if the NPC has reached its destination
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            // Set a new random destination
            SetRandomDestination();
        }
    }

    private void SetRandomDestination()
    {
        Vector3 randomPosition = GetRandomNavMeshPosition();
        agent.SetDestination(randomPosition);
    }

    private Vector3 GetRandomNavMeshPosition()
    {
        // Generate a random point on the NavMesh within a specified range
        Vector3 randomDirection = Random.insideUnitSphere * 10f;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }
}