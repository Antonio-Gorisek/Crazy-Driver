using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PlayTime : MonoBehaviour
{
    [SerializeField] private bool loadBestScoreOnly = false;
    private TMP_Text timerText;

    private float timeElapsed;
    private bool scoreLoaded = false;
    private const string BestScoreKey = "BestScore";

    private int lastSpeedIncreaseAt = 0;

    private void Start()
    {
        timerText = GetComponent<TMP_Text>();
        if (loadBestScoreOnly && PlayerPrefs.HasKey(BestScoreKey))
        {
            float bestScore = PlayerPrefs.GetFloat(BestScoreKey);
            timerText.text = "Best Score: " + FormatTime(bestScore);
        }
    }

    void Update()
    {
        if (GameManager.Instance.isGameOver() || loadBestScoreOnly)
        {
            UpdateBestScore();
            return;
        }

        int currentTens = GetNumberOfTens(timeElapsed);

        if (currentTens > lastSpeedIncreaseAt)
        {
            GameManager.Instance.IncreaseDifficulty();
            lastSpeedIncreaseAt = currentTens;
        }

        timeElapsed += Time.deltaTime;

        timerText.text = FormatTime(timeElapsed);
    }

    void UpdateBestScore()
    {
        if (scoreLoaded)
            return;

        if (!PlayerPrefs.HasKey(BestScoreKey) || timeElapsed > PlayerPrefs.GetFloat(BestScoreKey))
        {
            PlayerPrefs.SetFloat(BestScoreKey, timeElapsed);
            Debug.Log($"New record saved: {timeElapsed}");
            scoreLoaded = true;
        }
    }

    int GetNumberOfTens(float time)
    {
        return Mathf.FloorToInt(time / 10f);
    }


    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60);
        int centiseconds = Mathf.FloorToInt((time * 100) % 100);

        return string.Format("{0:00}:{1:00}<size=50%>:{2:00}</size>", minutes, seconds, centiseconds);
    }
}