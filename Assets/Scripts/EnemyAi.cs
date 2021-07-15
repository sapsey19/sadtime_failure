using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour {

    public NavMeshAgent agent;
    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    //Patrolling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking 
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    //Enemy projectile
    public GameObject projectile;

    private float health;

    //test floor stuff
    private bool grounded;
    private Vector3 normalVector = Vector3.up;
    private bool cancellingGrounded;

    private void Awake() {
        player = GameObject.Find("Player Container").transform.GetChild(0).transform;
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange && agent.enabled) Patrolling();
        if (playerInSightRange && !playerInAttackRange && agent.enabled) ChasePlayer();
        if (playerInSightRange && playerInAttackRange && agent.enabled) AttackPlayer();

        if(!agent.enabled && grounded) {
            GetComponent<Rigidbody>().isKinematic = true;
            agent.enabled = true;
        }
    }

    private void Patrolling() {
        if (!walkPointSet && agent.enabled)
            SearchWalkPoint();
        else if(agent.enabled)
            agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }

    private void ChasePlayer() {
        if(agent.enabled) agent.SetDestination(player.position);
    }

    private void AttackPlayer() {

        if (agent.enabled) {
            agent.SetDestination(transform.position);
        }

        transform.LookAt(player);

        if(!alreadyAttacked) {
            Rigidbody rb = Instantiate(projectile, transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            rb.AddForce(transform.forward * 32f, ForceMode.Impulse);
            rb.AddForce(transform.up * 3f, ForceMode.Impulse);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack() {
        alreadyAttacked = false;
    }

    private void SearchWalkPoint() {
        if (agent.enabled) {
            float randomZ = Random.Range(-walkPointRange, walkPointRange);
            float randomX = Random.Range(-walkPointRange, walkPointRange);

            walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

            if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround)) {
                walkPointSet = true;
            }
        }
    }

    public void TakeDamage(float damage) {
        health -= damage;

        if (health <= 0) 
            Invoke(nameof(DestroyEnemy), 0.5f);
    }

    public void DisableAi() {
        GetComponent<Rigidbody>().isKinematic = false;
        agent.enabled = false;
    }

    //public void OnTriggerEnter(Collider other) {
    //    if (!agent.enabled && other.CompareTag("Environment")) {
    //        agent.enabled = true;
    //        GetComponent<Rigidbody>().isKinematic = true;
    //        //Debug.Log("enabled");
    //    }
    //}

    //private void IsOnGround() {
    //    if(Physics.Raycast(transform.position, -Vector3.up, out RaycastHit hit, 10f)) {
    //        if(hit.transform.CompareTag("Environment")) {
    //            Debug.DrawLine(transform.position, hit.point, Color.cyan);
    //            Debug.Log("heherh");
    //            agent.enabled = true;
    //            GetComponent<Rigidbody>().isKinematic = true;
    //        }
    //    }
    //}

    private bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < 35;
    }

    private void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return; //???????????????????????????

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    private void StopGrounded() {
        grounded = false;
    }

    private void DestroyEnemy() {
        Destroy(gameObject);
    }
}
