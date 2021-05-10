using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int health;
    public int maxHealth;
    public float moveSpeed;
    public bool canMove;
    public float attackSpeed;
    private float attackTimer;
    public float arrowSpeed;
    public GameObject arrowPrefab;
    public Sprite[] spriteDirections;
    private SpriteRenderer spriteR;
    public DungeonManager dM;

    // Start is called before the first frame update
    void Start()
    {
        spriteR = gameObject.GetComponent<SpriteRenderer>();
        canMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(attackTimer > 0) attackTimer -= Time.deltaTime;

        if(canMove && !dM.paused) {
            if(Input.GetKey(KeyCode.W)) {
                transform.Translate(Vector2.up * moveSpeed * Time.deltaTime);
                spriteR.sprite = spriteDirections[0];
            }
            if(Input.GetKey(KeyCode.A)) {
                transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
                spriteR.sprite = spriteDirections[1];
            }
            if(Input.GetKey(KeyCode.S)) {
                transform.Translate(Vector2.down * moveSpeed * Time.deltaTime);
                spriteR.sprite = spriteDirections[2];
            }
            if(Input.GetKey(KeyCode.D)) {
                transform.Translate(Vector2.right * moveSpeed * Time.deltaTime);
                spriteR.sprite = spriteDirections[3];
                spriteR.flipX = true;
            }

            if(Input.GetKey(KeyCode.UpArrow)) {
                Attack(Vector2.up);
                spriteR.sprite = spriteDirections[0];
            }
            if(Input.GetKey(KeyCode.LeftArrow)) {
                Attack(Vector2.left);
                spriteR.sprite = spriteDirections[1];
            }
            if(Input.GetKey(KeyCode.DownArrow)) {
                Attack(Vector2.down);
                spriteR.sprite = spriteDirections[2];
            }
            if(Input.GetKey(KeyCode.RightArrow)) {
                Attack(Vector2.right);
                spriteR.sprite = spriteDirections[3];
                spriteR.flipX = true;
            }
        }

        // Check Bounds
        if(transform.position.x > dM.curRoomBoundsX.x) transform.position = new Vector3(dM.curRoomBoundsX.x, transform.position.y, 0);
        if(transform.position.x < dM.curRoomBoundsX.y) transform.position = new Vector3(dM.curRoomBoundsX.y, transform.position.y, 0);
        if(transform.position.y > dM.curRoomBoundsY.x) transform.position = new Vector3(transform.position.x, dM.curRoomBoundsY.x, 0);
        if(transform.position.y < dM.curRoomBoundsY.y) transform.position = new Vector3(transform.position.x, dM.curRoomBoundsY.y, 0);
    }

    public void Attack(Vector2 direction) {
        if(attackTimer <= 0) {
            float zRot = (-90 * direction.x) + (direction.y == -1 ? 180 : 0);
            GameObject arrow = (GameObject)Instantiate(arrowPrefab, transform.position, Quaternion.Euler(0, 0, zRot));
            arrow.GetComponent<Rigidbody2D>().velocity = direction * arrowSpeed;
            arrow.GetComponent<Projectile>().dM = dM;

            attackTimer = attackSpeed;
        }
    }

    public void Damage() {
        health--;
        dM.UpdateHearts(health, maxHealth);
        if(health == 0) {
            dM.GameOver();
        }
    }

    public void Heal() {
        if(health < maxHealth-1) {
            health += 2;
        } else {
            health = maxHealth;
        }
        dM.UpdateHearts(health, maxHealth);
    }
}
