using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrackScore : MonoBehaviour
{
    public TextMeshProUGUI scoreTextbox;
    public TextMeshProUGUI topScoreTextbox;
    public float timeAtStart;
    public static float timeElapsed = 0;

    private void Awake()
    {
        TrackScore.timeElapsed = 0;
    }

    void Start()
    {
        timeAtStart = Time.time;
        scoreTextbox.text = "";
        topScoreTextbox.text = $"Record: {PlayerPrefs.GetInt("BestTime", 600):N0}";
    }

    void Update()
    {
        timeElapsed = Time.time - timeAtStart;
        scoreTextbox.text = $"{(timeElapsed):N0}";
    }

    public void UpdateHighScore()
    {
        if (timeElapsed < PlayerPrefs.GetInt("BestTime", 600))
        {
            PlayerPrefs.SetInt("BestTime", (int)timeElapsed);
        }
    }
}
