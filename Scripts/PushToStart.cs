using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PushToStart : MonoBehaviour
{
    public GameObject panel;
    public GameObject pushToContinue;
    public Sprite[] imageList;
    public SFXPlayer sfx;
    private MusicInstance music;
    private Image image;
    private int imageIndex;
    private bool buttonPressed;

    private float timeOfNewPage;
    private float interval = 1f;

    private void Awake()
    {
        music = GameObject.Find("MusicPlayer").GetComponent<MusicInstance>();
        image = panel.GetComponent<Image>();
    }

    void Start()
    {
        imageIndex = 0;
        buttonPressed = false;
        music.Reset();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!buttonPressed)
            {
                sfx.PlayStartSound();
                panel.SetActive(true);
                // pushToContinue.SetActive(true);
                buttonPressed = true;
                image.sprite = imageList[0];
                timeOfNewPage = Time.time;
}
        }

        if (buttonPressed)
        {
            float adjustedInterval = interval;
            if (imageIndex == 0) adjustedInterval = interval * 2.0f; 
            if (Time.time - timeOfNewPage > adjustedInterval)
            {
                imageIndex += 1;
                if (imageIndex == imageList.Length)
                {
                    imageIndex = 0;
                    SceneManager.LoadScene("PaperVersion");
                } else
                {
                    image.sprite = imageList[imageIndex];
                }
                timeOfNewPage = Time.time;
            }    
        }
    }
}
