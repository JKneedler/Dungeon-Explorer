using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public enum Direction { 
        LEFT,
        UP,
        RIGHT,
        DOWN,
        MIDDLE
    }
public class DungeonPathTrigger : MonoBehaviour
{
    public Direction direction;
    void OnTriggerEnter2D(Collider2D col) {
        if(col.gameObject.tag == "Player") {
            col.gameObject.GetComponent<Player>().dM.MoveRooms(direction);
        }
    }
}
