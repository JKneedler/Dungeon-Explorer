using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class DungeonManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject player;
    public DungeonGen dG;
    public int curFloor;
    public Vector2 curRoom;
    public Vector2 curRoomBoundsX, curRoomBoundsY;
    public Image fadePanel;
    public Animation panelAnimation;
    public Image[] heartImages;
    public int score;
    public Text scoreText;
    public Text floorText;
    public bool paused;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public Text finalScore;

    // Start is called before the first frame update
    void Start()
    {
        curFloor = 1;
        dG.GenerateDungeon(curFloor);

        curRoom = dG.startingRoom;
        player = (GameObject)Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        SetRoom(curRoom, Direction.MIDDLE);

        player.GetComponent<Player>().dM = this;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            Pause();
        }
    }

    public void SetRoom(Vector2 newRoom, Direction incomingDir) {
        dG.DeactivateEnemies(curRoom);
        curRoom = newRoom;
        Vector3 newRoomPosition = new Vector3(newRoom.x * dG.cellSize.x, newRoom.y * dG.cellSize.y, 0);
        Camera.main.transform.position = new Vector3(newRoomPosition.x, newRoomPosition.y, -10);
        curRoomBoundsX = new Vector2(newRoomPosition.x + (dG.roomSize.x*.5f), newRoomPosition.x - (dG.roomSize.x*.5f));
        curRoomBoundsY = new Vector2(newRoomPosition.y + (dG.roomSize.y*.5f), newRoomPosition.y - (dG.roomSize.y*.45f));

        if(incomingDir == Direction.LEFT) player.transform.position = new Vector3(newRoomPosition.x - (dG.roomSize.x*.4f), newRoomPosition.y, 0);
        if(incomingDir == Direction.UP) player.transform.position = new Vector3(newRoomPosition.x, newRoomPosition.y + (dG.roomSize.y*.4f), 0);
        if(incomingDir == Direction.RIGHT) player.transform.position = new Vector3(newRoomPosition.x + (dG.roomSize.x*.4f), newRoomPosition.y, 0);
        if(incomingDir == Direction.DOWN) player.transform.position = new Vector3(newRoomPosition.x, newRoomPosition.y - (dG.roomSize.y*.4f), 0);
        if(incomingDir == Direction.MIDDLE) player.transform.position = new Vector3(newRoomPosition.x, newRoomPosition.y, 0);

        dG.ActivateEnemies(newRoom);
    }

    public void MoveRooms(Direction direction) {
        StartCoroutine(MoveRoomsCoroutine(direction));
    }

    IEnumerator MoveRoomsCoroutine(Direction direction) {
        player.GetComponent<Player>().canMove = false;
        panelAnimation["fade-out-in"].speed = 1.5f;
        panelAnimation.Play("fade-out-in");

        yield return new WaitForSeconds(.2f);

        if(direction == Direction.LEFT) SetRoom(new Vector2(curRoom.x-1, curRoom.y), Direction.RIGHT);
        if(direction == Direction.UP) SetRoom(new Vector2(curRoom.x, curRoom.y+1), Direction.DOWN);
        if(direction == Direction.RIGHT) SetRoom(new Vector2(curRoom.x+1, curRoom.y), Direction.LEFT);
        if(direction == Direction.DOWN) SetRoom(new Vector2(curRoom.x, curRoom.y-1), Direction.UP);
        player.GetComponent<Player>().canMove = true;
    }

    public void NextFloor() {
        StartCoroutine(NextFloorCoroutine());
    }

    IEnumerator NextFloorCoroutine() {
        player.GetComponent<Player>().canMove = false;
        //panelAnimation["fade-out-hold-in"].speed = .75f;
        panelAnimation.Play("fade-out-hold-in");

        yield return new WaitForSeconds(.33f);

        curFloor++;
        floorText.text = curFloor.ToString();
        score += 1000;
        UpdateScore();
        dG.DestroyDungeon();
        dG.GenerateDungeon(curFloor);

        curRoom = dG.startingRoom;
        SetRoom(curRoom, Direction.MIDDLE);
        player.GetComponent<Player>().canMove = true;
    }

    public void UpdateHearts(int playerHealth, int maxHealth) {
        for(int i = 0; i < maxHealth; i++) {
            if(i < playerHealth) {
                heartImages[i].enabled = true;
            } else {
                heartImages[i].enabled = false;
            }
        }
    }

    public void UpdateScore() {
        scoreText.text = score.ToString();
    }

    public void KillEnemy(Enemy enemy) {
        dG.KillEnemy(enemy);
        score += 500;
        UpdateScore();
    }

    public void ScoreBonus() {
        score += 2000;
        UpdateScore();
    }

    public void Pause() {
        if(paused) {
            Time.timeScale = 1;
            paused = false;

            fadePanel.color = new Color(0, 0, 0, 0);
            pauseMenu.SetActive(false);
        } else {
            Time.timeScale = 0;
            paused = true;

            fadePanel.color = new Color(0, 0, 0, 0.5f);
            pauseMenu.SetActive(true);
        }
    }

    public void GameOver() {
        paused = true;
        Destroy(player);
        fadePanel.color = new Color(0, 0, 0, 0.5f);
        finalScore.text = "Score : " + score.ToString();
        gameOverMenu.SetActive(true);
    }

    public void Retry() {
        SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
    }

    public void Exit() {
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

}
