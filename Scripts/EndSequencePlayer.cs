using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndSequencePlayer : MonoBehaviour
{
    public GameObject panel;
    public Sprite[] imageList;
    private Image image;
    private int imageIndex;
    private AudioSource audioSource;
    public AudioSource bgm;
    public Image fadePanel;

    private void Awake()
    {
        audioSource = GameObject.Find("MusicPlayer").GetComponent<AudioSource>();
    }

    void Start()
    {
        image = panel.GetComponent<Image>();
        panel.SetActive(true);
        imageIndex = 0;
        image.sprite = imageList[imageIndex];
        Invoke("StartFade", 3.0f);
    }

    void StartFade()
    {
        StartCoroutine(FadeToStart());
    }

    IEnumerator FadeToStart()
    {
        float startTime = Time.time;
        while (Time.time - startTime < 5.1f)
        {
            float a = Mathf.Lerp(1, 0, (Time.time - startTime) / 5f);
            bgm.volume = a;
            fadePanel.color = new Color(0, 0, 0, 1 - a);
            yield return null;
        }
        audioSource.time = 0;
        SceneManager.LoadScene("StartMenu");
    }
}
