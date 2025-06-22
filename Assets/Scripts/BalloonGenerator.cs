using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BalloonGenerator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gameOverText;
    [SerializeField] TextMeshProUGUI detailText; 
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject gameOverPanel;

    [Header("Prefabs & Assets")]
    [SerializeField] GameObject balloonPrefab;
    [SerializeField] AudioClip menuMusic;
    [SerializeField] AudioClip gameMusic;
    [SerializeField] public AudioClip defaultPopSfx;
    [SerializeField] List<BalloonType> balloonTypes; 

    [Header("Spawn Ayarlarý")]
    [SerializeField] float initialVelocity = 80f;
    [SerializeField] float initialDelay = 0.5f;

    public bool IsMenu { get; private set; } = true;
    float currDelay, currVelocity;
    int score = 0;
    Dictionary<BalloonType, int> hitCounts = new(); 
    const int WIN_SCORE = 50;
    const int LOSE_SCORE = 0;

    AudioSource musicSrc;

    void Start()
    {
        currDelay = initialDelay;
        currVelocity = initialVelocity;
        musicSrc = GetComponent<AudioSource>();

        UpdateScoreUI();
        PlayMusic(menuMusic);
    }

    public void OnStartButton()
    {
        if (!IsMenu) return;

        menuPanel.SetActive(false);
        IsMenu = false;
        Time.timeScale = 1f;

        PlayMusic(gameMusic);
        StartCoroutine(SpawnRoutine());
    }

    public void OnExitButton() => Application.Quit();

    public void OnRestartButton() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator SpawnRoutine()
    {
        while (!IsMenu)
        {
            SpawnBalloon();
            yield return new WaitForSeconds(currDelay);
            currVelocity *= 1.001f;
            currDelay *= 0.999f;
        }
    }

    void SpawnBalloon()
    {
        var type = balloonTypes[Random.Range(0, balloonTypes.Count)];

        Vector2 screenPos = new Vector2(Screen.width * Random.Range(0.1f, 0.9f), -100f);

        RectTransform canvasRect = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPos, null, out localPoint);

        GameObject obj = Instantiate(balloonPrefab, canvasRect); 
        RectTransform rt = obj.GetComponent<RectTransform>();
        rt.anchoredPosition = localPoint;

        obj.name = $"Balloon_{type.name}";
        var balloon = obj.GetComponent<Balloon>();
        balloon.Init(type, this, currVelocity);
    }

    public void OnBalloonPopped(BalloonType type)
    {
        AddScore(type.score);
        CountHit(type);
    }

    public void OnBalloonReachedTop(BalloonType type)
    {
        if (type.score > 0) AddScore(type.score);
    }

    void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
        CheckGameEnd();
    }

    void CountHit(BalloonType type)
    {
        if (!hitCounts.ContainsKey(type)) hitCounts[type] = 0;
        hitCounts[type]++;
    }

    void UpdateScoreUI() => scoreText.text = $"Skor: {score}";
    void DestroyAllBalloons()
    {
        Balloon[] allBalloons = FindObjectsOfType<Balloon>();
        foreach (var balloon in allBalloons)
        {
            Destroy(balloon.gameObject);
        }
    }

    void CheckGameEnd()
    {
        if (score >= WIN_SCORE || score < LOSE_SCORE)
        {
            StopAllCoroutines();
            IsMenu = true;
            Time.timeScale = 0f;

            int highScore = Mathf.Max(score, PlayerPrefs.GetInt("HighScore", 0));
            PlayerPrefs.SetInt("HighScore", highScore);
            Debug.Log("Kayýtlý yüksek skor: " + PlayerPrefs.GetInt("HighScore", 0));

            DestroyAllBalloons();  

            gameOverText.text = $"Son Skor: {score}\nEn Yüksek: {highScore}";
            detailText.text = BuildDetailText();

            gameOverPanel.SetActive(true);
            PlayMusic(menuMusic);
        }
    }


    string BuildDetailText()
    {
        System.Text.StringBuilder sb = new();
        foreach (var kv in hitCounts)
            sb.AppendLine($"{kv.Key.name}: {kv.Value}");
        return sb.ToString();
    }

    void PlayMusic(AudioClip clip)
    {
        if (!clip) return;
        musicSrc.Stop();
        musicSrc.clip = clip;
        musicSrc.loop = true;
        musicSrc.Play();
    }
}
