using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;
    public GameObject gameManager;

    private void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        remainingTime = gameManager.GetComponent<GameManager>().GetGameDuration();
    }
    public void StartGame()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager");
        remainingTime = gameManager.GetComponent<GameManager>().GetGameDuration();
    }
    void Update()
    {
        remainingTime -= Time.deltaTime;
        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    public void SetRemainingTime(int time)
    {
        remainingTime = time;
    }
    public float GetRemainingTime()
    {
        return remainingTime;
    }
}
