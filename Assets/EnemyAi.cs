using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask WhatIsGround, WhatIsPlayer;

    //Patrol
    public Vector3 WalkPoint;
    bool WalkPointSet;
    public float WalkPointRange;

    //Attack
    public float TimeBetweenAttack;
    bool alreadyAttacking;
    public GameObject Projectile;   

    //state
    public float sighRange, attackRange;
    public bool PlayerInSightRange, PlayerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Cube").transform;
        agent = GetComponent<NavMeshAgent>();

    }

    private void Update()
    {
        //Check for sight and attack range
        PlayerInSightRange = Physics.CheckSphere(transform.position, sighRange, WhatIsPlayer);
        PlayerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);

        if (!PlayerInSightRange && !PlayerInAttackRange) Patroling();
        if (!PlayerInSightRange && !PlayerInAttackRange) ChasePlayer();
        if (!PlayerInSightRange && !PlayerInAttackRange) AttackPlayer();

    }

    private void Patroling()
    {
        if (!WalkPointSet) SearchWalkPoint();

        if (WalkPointSet)
            agent.SetDestination(WalkPoint);

        Vector3 distanceToWalkPoint = transform.position - WalkPoint;

        //walkpoint reach
        if (distanceToWalkPoint.magnitude < 1f)
            WalkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // calculate random point in range
        float randomz = Random.Range(-WalkPointRange, WalkPointRange);
        float randomx = Random.Range(-WalkPointRange, WalkPointRange);

        WalkPoint = new Vector3(transform.position.x + randomx, transform.position.y, transform.position.z + randomz);

        if (Physics.Raycast(WalkPoint, -transform.up, 2f, WhatIsGround))
            WalkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        // if you want the enemy stay
        // agent.SetDestination(transform.position);

        // chasing player
        agent.SetDestination(player.position);

        transform.LookAt(player);
        
        if (!alreadyAttacking)
        {

            //Attack Code
            Rigidbody rb = Instantiate(Projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();

            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 8f, ForceMode.Impulse);


            alreadyAttacking = true;
            Invoke(nameof(ResetAttack), TimeBetweenAttack);

        }
    }

    private void ResetAttack()
    {
        alreadyAttacking = false;

    }
}
