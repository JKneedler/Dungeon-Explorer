using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusPickup : MonoBehaviour
{
    public bool health;
    public bool score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Player") {
            if(health) col.gameObject.GetComponent<Player>().Heal();
            if(score) col.gameObject.GetComponent<Player>().dM.ScoreBonus();
            Destroy(gameObject);
        }
    }
}
