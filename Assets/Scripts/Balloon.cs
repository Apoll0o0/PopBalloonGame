using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Balloon : MonoBehaviour
{
    AudioSource audioSource;

    Image image;

    BalloonGenerator bl;

    bool isTouched = false;

    int spriteNumber = 0;

    private void Start()
    {
        bl = transform.root.GetComponent<BalloonGenerator>();
        audioSource = transform.GetComponent<AudioSource>();
        image = GetComponent<Image>();
        Color col = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        col.a = Random.Range(0.6f, 1f);
        image.color = col;
    }

    public void OnTouch()
    {
        if (isTouched || bl.isOnMenu) return;
        isTouched = true;
        bl.killedCount++;
        bl.ShowKilledText();
        audioSource.Play();
        StartCoroutine(ShowAnim());
        Destroy(gameObject,0.8f);
    }
    IEnumerator ShowAnim()
    {
        image.sprite = bl.destrSp[spriteNumber];
        spriteNumber++;
        if (spriteNumber == bl.destrSp.Length)
        {
            image.enabled = false;
            yield break;
        }
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(ShowAnim());
        yield return null;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        bl.leackedCount++;
        bl.ShowLeackedText();
        Destroy(gameObject);
    }


}
