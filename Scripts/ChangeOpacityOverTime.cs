using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ChangeOpacityOverTime : MonoBehaviour
{

    private TextMeshProUGUI textbox;
    public Color startColor;
    public Color endColor;
    private float startTime;

    void Start()
    {
        textbox = GetComponent<TextMeshProUGUI>();
/*        startColor = new Color(1, 1, 1, 0.5f);
        endColor = new Color(1, 1, 1, 0.0f);*/
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // textbox.color = Color.Lerp(startColor, endColor, 0.5f + 0.5f * Mathf.Sin((Time.time - startTime)/ 0.75f));
        textbox.color = Color.Lerp(startColor, endColor, 1 - Mathf.Pow(Mathf.Sin((Time.time - startTime)/ 1.25f), 2f));
    }

    public void ResetTime()
    {
        startTime = Time.time;
    }
}
