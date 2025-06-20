using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class BalloonGenerator : MonoBehaviour
{
    [SerializeField]

    TextMeshProUGUI killedText;

    [SerializeField]

    TextMeshProUGUI leackedText;

    [SerializeField]

    TextMeshProUGUI gameOverText;

    [SerializeField]

    GameObject balloonPrefab;

    [HideInInspector]
    public int killedCount = 0;

    [HideInInspector]
    public int leackedCount = 0;

    float spawnDelay = 0.5f;

    float velocity = 80f;

    bool gameIsOver = false;

    public bool isOnMenu = false;

    int maxHP = 10;

    int curHP;

    Rigidbody2D rb;

    AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        curHP = maxHP;
        StartCoroutine(Spawn());

    }

    public Sprite[] destrSp;

    IEnumerator Spawn()
    {
        SpawnBalloon();
        yield return new WaitForSeconds(spawnDelay);
        StartCoroutine(Spawn());
        yield return null;
    }

    void SpawnBalloon()
    {
        Vector2 spawnPosition = new Vector2(Screen.width * Random.Range(0.1f, 0.9f), -200f);
        GameObject obj = Instantiate(balloonPrefab, spawnPosition, transform.rotation, transform);
        rb = obj.GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0f, velocity);
        velocity *= 1.001f;
        spawnDelay *= 0.999f;
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }

    public void ShowKilledText()
    {
        killedText.text = "Patlatýldý: " + killedCount;
    }

    [SerializeField]

    GameObject menuObj;
    public void ShowLeackedText()
    {
        leackedText.text = "Kaçýrýlan: " + leackedCount;
        audioSource.Play();
        curHP--;
        if(curHP == 0)
        {
            gameIsOver = true;
            menuObj.SetActive(true);
            Time.timeScale = 0.0f;
            isOnMenu = true;
        }
    }

    public void StartMenu()
    {
        if (gameIsOver) return;
        if (!isOnMenu)
        {
            menuObj.SetActive(true);
            isOnMenu = true;
            Time.timeScale = 0.0f;
        }
        else
        {
            menuObj.SetActive(false);
            Time.timeScale = 1.0f;
            isOnMenu= false;
        }
    }

}
