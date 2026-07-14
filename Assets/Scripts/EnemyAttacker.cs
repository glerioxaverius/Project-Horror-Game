using System.Collections;
using UnityEngine;
using UnityEngine.AI; 

public class EnemyAttacker : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damageAmount = 20f;
    
    [SerializeField] private float attackInterval = 5f; 
    [SerializeField] private float damageDelay = 1.49f;
    private NavMeshAgent _agent;
    private Animator _animator;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private IEnumerator AttackRoutine()
    {
        if (_agent != null)
        {
            _agent.isStopped = true;       
            _agent.velocity = Vector3.zero; 
        }

        if (_animator != null)
        {
            _animator.SetTrigger("Attack"); 
        }

        yield return new WaitForSeconds(attackInterval);

        if (_agent != null)
        {
            _agent.isStopped = false; 
        }
    }
}