using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Vector2 room;
    public float moveSpeed;
    public bool sameRoom;
    public float attackSpeed;
    private float attackTimer;
    public float attackDistance;
    public bool ranged;
    public float rangedObjectSpeed;
    public GameObject rangedPrefab;
    public GameObject player;
    public DungeonManager dM;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        dM = player.GetComponent<Player>().dM;
    }

    // Update is called once per frame
    void Update()
    {
        if (sameRoom && !dM.paused) {
            if(attackTimer > 0) attackTimer -= Time.deltaTime;
            if(Vector3.Distance(player.transform.position, transform.position) > attackDistance) {
                // Move towards player in order to be able to attack
                Vector3 moveDir = new Vector3(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y, 0);
                transform.Translate(moveDir.normalized * moveSpeed * Time.deltaTime);
            } else {
                Attack();
            }
        }
    }

    void Attack() {
        if(attackTimer <= 0) {
            if(ranged) {
                Vector3 targetDir = (player.transform.position - transform.position).normalized;
                GameObject rangedObj = (GameObject)Instantiate(rangedPrefab, transform.position, Quaternion.LookRotation(Vector3.forward, targetDir));
                rangedObj.GetComponent<Rigidbody2D>().velocity = targetDir * rangedObjectSpeed;
                rangedObj.GetComponent<Projectile>().dM = player.GetComponent<Player>().dM;
            } else {
                player.GetComponent<Player>().Damage();
            }
            attackTimer = attackSpeed;
        }
    }
}
