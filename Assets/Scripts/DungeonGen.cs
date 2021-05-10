using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGen : MonoBehaviour
{
    public Vector2 mapSize;
    public Vector2 cellSize;
    public Vector2 roomSize;
    public float dungeonDensity;
    public float bonusPickupDensity;
    public float enemyRoomDensity;
    public int baseMaxEnemies;
    private DungeonRoom[,] curLevel;
    private List<Vector2> roomPlacements;
    private List<GameObject> roomObjects;
    public GameObject[] roomPrefabs;
    public GameObject[] enemyPrefabs;
    public GameObject stairsPrefab;
    public GameObject[] bonusPickupPrefabs;
    public Vector2 startingRoom;

    public class DungeonRoom {
        // -1 if no room, 0-15 for room types
        public int roomVal;
        public int numEnemies;
        public bool stairs;
        public bool bonusPickup;
        public List<Vector2> navRooms = new List<Vector2>();
        public List<Enemy> enemies;
    }

    public void GenerateDungeon(int level) {
        curLevel = new DungeonRoom[(int)mapSize.x + Mathf.FloorToInt(level/10), (int)mapSize.y + Mathf.FloorToInt(level/10)];
        for(int x = 0; x < mapSize.x; x++) {
            for(int y = 0; y < mapSize.y; y++) {
                curLevel[x,y] = new DungeonRoom();
                if (Random.Range(0.0f, 1.0f) < dungeonDensity) {
                    // Make a room for this cell - will calculate roomVal at the end
                    curLevel[x,y].roomVal = 0;

                    if(Random.Range(0.0f, 1.0f) < enemyRoomDensity+(level*.01f)) {
                        //Enemies in this room - enemyRoomDensity and amount of enemies should increase with level
                        curLevel[x,y].numEnemies = Random.Range(0, (baseMaxEnemies + Mathf.FloorToInt(level/10))+1);
                    }

                    if(Random.Range(0.0f, 1.0f) < bonusPickupDensity+(level*.01f)) {
                        curLevel[x,y].bonusPickup = true;
                    }

                } else {
                    curLevel[x,y].roomVal = -1;
                }
            }
        }

        // Calculate Room Values
        roomPlacements = new List<Vector2>(); 
        for(int x = 0; x < mapSize.x; x++) {
            for(int y = 0; y < mapSize.y; y++) {
                if (curLevel[x,y].roomVal != -1) {
                    //Below
                    if(IsRoom(curLevel, x, y-1)) { 
                        curLevel[x,y].roomVal += 1;
                        curLevel[x,y].navRooms.Add(new Vector2(x,y-1));
                    }
                    //Right
                    if(IsRoom(curLevel, x+1, y)) {
                        curLevel[x,y].roomVal += 2;
                        curLevel[x,y].navRooms.Add(new Vector2(x+1,y));
                    }
                    //Above
                    if(IsRoom(curLevel, x, y+1)) {
                        curLevel[x,y].roomVal += 4;
                        curLevel[x,y].navRooms.Add(new Vector2(x,y+1));
                    }
                    //Left
                    if(IsRoom(curLevel, x-1, y)) {
                        curLevel[x,y].roomVal += 8;
                        curLevel[x,y].navRooms.Add(new Vector2(x-1,y));
                    }

                    roomPlacements.Add(new Vector2(x, y));
                }
            }
        }

        // Keep only the largest navigational group
        bool allGroupsChecked = false;
        List<Vector2> tempPlacements = new List<Vector2>(roomPlacements);
        List<List<Vector2>> navGroups = new List<List<Vector2>>();
        while(!allGroupsChecked) {
            Vector2 navStartRoom = tempPlacements[0];
            tempPlacements.RemoveAt(0);

            List<Vector2> reachableRooms = new List<Vector2>();
            reachableRooms.Add(navStartRoom);
            GetAllReachable(navStartRoom, reachableRooms);

            navGroups.Add(reachableRooms);
            foreach(Vector2 placementToRemove in reachableRooms) {
                tempPlacements.Remove(placementToRemove);
            }

            if(tempPlacements.Count == 0) allGroupsChecked = true;
        }

        List<Vector2> largest = new List<Vector2>();
        foreach(List<Vector2> group in navGroups) {
            if (group.Count > largest.Count) {
                largest = group;
            }
        }

        roomPlacements = largest;

        // Place stairs to next level and set starting room
        Vector2 stairsLocation = roomPlacements[Random.Range(0, roomPlacements.Count)];
        curLevel[(int)stairsLocation.x, (int)stairsLocation.y].stairs = true;

        startingRoom = roomPlacements[Random.Range(0, roomPlacements.Count)];

        CreateDungeon();
    }

    public void GetAllReachable(Vector2 start, List<Vector2> reachable) {
        foreach(Vector2 adjacent in curLevel[(int)start.x, (int)start.y].navRooms) {
            if(!reachable.Contains(adjacent)) {
                reachable.Add(adjacent);
                GetAllReachable(adjacent, reachable);
            }
        }
    }

    public bool IsRoom(DungeonRoom[,] curLevel, int x, int y) {
        if(x < 0 || x >= mapSize.x) return false;
        if(y < 0 || y >= mapSize.y) return false;
        if(curLevel[x,y].roomVal != -1) return true;
        return false;
    }

    public void CreateDungeon() {
        roomObjects = new List<GameObject>();
        foreach(Vector2 roomLoc in roomPlacements) {
            DungeonRoom room = curLevel[(int)roomLoc.x, (int)roomLoc.y];
            Vector3 roomWorldPosition = new Vector3(roomLoc.x * cellSize.x, roomLoc.y * cellSize.y, 0);
            GameObject roomObj = (GameObject)Instantiate(roomPrefabs[room.roomVal], roomWorldPosition, Quaternion.identity);
            roomObj.transform.parent = transform;

            room.enemies = new List<Enemy>();
            if(room.numEnemies > 0) {
                for(int i = 0; i < room.numEnemies; i++) {
                    float enemyX = Random.Range((roomLoc.x * cellSize.x) - (roomSize.x*.45f), (roomLoc.x * cellSize.x) + (roomSize.x*.45f));
                    float enemyY = Random.Range((roomLoc.y * cellSize.y) - (roomSize.y*.35f), (roomLoc.y * cellSize.y) + (roomSize.y*.35f));
                    GameObject enemyObj = (GameObject)Instantiate(enemyPrefabs[Random.Range(0,2)], new Vector3(enemyX, enemyY, 0), Quaternion.identity);
                    enemyObj.transform.parent = roomObj.transform;

                    Enemy enemy = enemyObj.GetComponent<Enemy>();
                    enemy.room = roomLoc;
                    enemy.sameRoom = false;
                    room.enemies.Add(enemy);
                }
            }

            if(room.bonusPickup) {
                float pickupX = Random.Range((roomLoc.x * cellSize.x) - (roomSize.x*.45f), (roomLoc.x * cellSize.x) + (roomSize.x*.45f));
                float pickupY = Random.Range((roomLoc.y * cellSize.y) - (roomSize.y*.35f), (roomLoc.y * cellSize.y) + (roomSize.y*.35f));
                GameObject pickup = (GameObject)Instantiate(bonusPickupPrefabs[Random.Range(0,2)], new Vector3(pickupX, pickupY, 0), Quaternion.identity);
                pickup.transform.parent = roomObj.transform;
            }

            if(room.stairs) {
                float stairsX = Random.Range((roomLoc.x * cellSize.x) - (roomSize.x*.45f), (roomLoc.x * cellSize.x) + (roomSize.x*.45f));
                float stairsY = Random.Range((roomLoc.y * cellSize.y) - (roomSize.y*.35f), (roomLoc.y * cellSize.y) + (roomSize.y*.35f));
                GameObject stairs = (GameObject)Instantiate(stairsPrefab, new Vector3(stairsX, stairsY, 0), Quaternion.identity);
                stairs.transform.parent = roomObj.transform;
            }

            roomObjects.Add(roomObj);
        }
    }

    public void ActivateEnemies(Vector2 roomLoc) {
        DungeonRoom room = curLevel[(int)roomLoc.x, (int)roomLoc.y];
        foreach(Enemy enemy in room.enemies) {
            enemy.sameRoom = true;
        }
    }

    public void DeactivateEnemies(Vector2 roomLoc) {
        DungeonRoom room = curLevel[(int)roomLoc.x, (int)roomLoc.y];
        foreach(Enemy enemy in room.enemies) {
            enemy.sameRoom = false;
        }
    }

    public void KillEnemy(Enemy enemyKilled) {
        DungeonRoom room = curLevel[(int)enemyKilled.room.x, (int)enemyKilled.room.y];
        room.enemies.Remove(enemyKilled);
        
        Destroy(enemyKilled.gameObject);
    }

    public void DestroyDungeon() {
        foreach(GameObject obj in roomObjects) {
            Destroy(obj);
        }
        roomObjects.Clear();
    }
}
