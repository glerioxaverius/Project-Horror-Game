using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;
    
    [Header("Movement Settings")]
    [SerializeField] private float chaseRange = 15f;
    [SerializeField] private float attackRange = 2f; 

    [Header("Attack Settings")]
    [SerializeField] private float attackInterval = 2.5f;

    private NavMeshAgent _agent;
    private Animator _animator;
    private bool _isAttacking = false;

    private void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        
        if (playerTransform == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) playerTransform = p.transform;
        }
    }

private void Update()
    {
        if (playerTransform == null) 
        {
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        if (_isAttacking)
        {
           
            return;
        }


        if (distanceToPlayer <= attackRange)
        {
            StartCoroutine(AttackRoutine());
        }
        else if (distanceToPlayer <= chaseRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopChasing();
        }
    }

    private void MoveTowardsPlayer()
    {
        if (_agent == null) return;

        _agent.isStopped = false; 
        _agent.SetDestination(playerTransform.position);

        if (_animator != null)
        {
            _animator.SetBool("IsMoving", true);
        }
    }

    private void StopChasing()
    {
        if (_agent == null) return;

        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        if (_animator != null)
        {
            _animator.SetBool("IsMoving", false); 
        }
    }

    private IEnumerator AttackRoutine()
    {
        _isAttacking = true;

        if (_agent != null)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }

        if (_animator != null)
        {
            _animator.SetBool("IsMoving", false);
            _animator.SetTrigger("Attack"); 
        }

        Debug.Log("[Enemy] Melancarkan serangan dan mengunci posisi!");

        yield return new WaitForSeconds(attackInterval);

        _isAttacking = false;
    }
}