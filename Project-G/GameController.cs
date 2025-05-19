using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private UIController uiController;
    [SerializeField]
    //private	GameObject		pattern01;

    // 플레이어 점수 (죽지않고 버틴 시간)
    public float CurrentScore { private set; get; } = 0;

    public bool IsGamePlay { private set; get; } = false;

    public void GameStart()
    {

    }

    public void GameExit()
    {
#if UNITY_EDITOR
		UnityEditor.EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    public void GameOver()
    {
        uiController.GameOver();
    }
}
