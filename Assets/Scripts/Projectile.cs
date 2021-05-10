using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public bool friendly;
    public float lifetime;
    public DungeonManager dM;

    // Update is called once per frame
    void Update()
    {
        if(lifetime > 0) {
            lifetime -= Time.deltaTime;
        } else {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(friendly) {
            if(col.gameObject.tag == "Enemy") {
                dM.KillEnemy(col.gameObject.GetComponent<Enemy>());
                Destroy(gameObject);
            }
        } else {
            if(col.gameObject.tag == "Player") {
                col.gameObject.GetComponent<Player>().Damage();
                Destroy(gameObject);
            }
        }
    }
}
