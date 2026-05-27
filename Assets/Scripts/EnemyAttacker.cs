using System.Collections;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private float attackInterval = 2.0f; 
    [SerializeField] private float damageDelay = 0.5f;    

    [Header("References")]
    [SerializeField] private Animator animator; 

    private float _nextAttackTime = 0f;
    private bool _isAttacking = false;

    
    private void OnTriggerStay(Collider other)
    {
            if (other.TryGetComponent(out PlayerController player))
        {
                if (Time.time >= _nextAttackTime && !_isAttacking)
            {
                StartCoroutine(AttackRoutine(player));
            }
        }
    }

    private IEnumerator AttackRoutine(PlayerController player)
    {
        _isAttacking = true;
        
        _nextAttackTime = Time.time + attackInterval;

        if (animator != null)
        {
            animator.SetTrigger("Attack");
            Debug.Log("[Enemy] Memulai animasi serangan!");
        }

        yield return new WaitForSeconds(damageDelay);

        if (player != null && player.CurrentHealth > 0)
        {
            player.TakeDamage(damageAmount);
            Debug.Log($"[Enemy] Player terkena hit! Damage: {damageAmount}");
        }

        _isAttacking = false;
    }
}