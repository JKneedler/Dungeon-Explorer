using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MenuGameController : MonoBehaviour
{
    public void PlayGame() {
        SceneManager.LoadScene("Dungeon", LoadSceneMode.Single);
    }

    public void Exit() {
        EditorApplication.isPlaying = false;
    }
}
