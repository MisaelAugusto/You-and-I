using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Loading : MonoBehaviour
{
    private bool showReasons;
    private float infinity = float.MaxValue;

    // Start is called before the first frame update
    void Start()
    {
        this.showReasons = true;

        Button skip = GameObject.Find("Skip").GetComponent<Button>();
        TextMeshProUGUI hundred = GameObject.Find("100").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI reasons = GameObject.Find("Reasons").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI message = GameObject.Find("Message").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI youAndI = GameObject.Find("You and I").GetComponent<TextMeshProUGUI>();

        /* Here you set the three time animations to each opening text
         * Each number in each array represents the delay to start an animation
         * First: delay to start RevealText() -> Typewriter effect
         * Second: delay to start ShakeText() -> Jitter text effect
         * thrird: delay to start FadeOutText() -> Fade out effect
         */
        this.ShowText(youAndI, new float[3] {0.0f, 3.0f, 6.0f});
        this.ShowText(hundred, new float[3] {7.5f, 11.5f, 14.5f});
        this.ShowText(reasons, new float[3] {7.5f, 11.5f, 14.5f});
        this.ShowText(message, new float[3] {16.0f, 32.5f, 36.0f});

        // You don't need to change this
        this.ShowText(skip.GetComponentInChildren<TextMeshProUGUI>(), new float[3] {0.0f, 1.0f, 36.0f});

        StartCoroutine(this.SetSkip(skip));
    }

    void Update()
    {
        if (Time.time >= 37.0f && this.showReasons)
        {
            this.showReasons = false;
            ReasonsManager.instance.StartShowReasons();
        }
    }
    
    private IEnumerator SetSkip(Button skip)
    {   
        yield return new WaitForSeconds(1.0f);
        skip.onClick.AddListener(this.Skip);
    }

    private void Skip()
    {
        StopAllCoroutines();

        string[] names = new string[4] {"100", "Reasons", "Message", "You and I"};
        foreach(TextMeshProUGUI text in GameObject.FindObjectsOfType<TextMeshProUGUI>())
        {
            if (names.Contains(text.gameObject.name))
            {
                Destroy(text.gameObject);
            }
        }

        TextMeshProUGUI skip = GameObject.Find("Text").GetComponent<TextMeshProUGUI>();
        StartCoroutine(Shaker.instance.ShakeText(skip, 0.0f));
        StartCoroutine(this.FadeOutText(skip, 0.0f));

        ReasonsManager.instance.StartShowReasons();
    }

    private void ShowText(TextMeshProUGUI text, float[] delays)
    {
        StartCoroutine(this.RevealText(text, delays[0]));
        StartCoroutine(Shaker.instance.ShakeText(text, delays[1]));
        if (delays[2] > 0.0f)
        {
            StartCoroutine(this.FadeOutText(text, delays[2]));
        }
    }

    private IEnumerator RevealText(TextMeshProUGUI text, float delay) {
        yield return new WaitForSeconds(delay);
        string originalText = text.text;
        text.text = "";
        text.enabled = true;
        
        for (int i = 0; i < originalText.Length; i++)
        {
            text.text += originalText[i];
            yield return new WaitForSeconds(0.07f);
        }
	}

    private IEnumerator FadeInImage(Image image, float delay)
    {
        yield return new WaitForSeconds(delay);

        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);

        while (image.color.a < 1.0f)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + (Time.deltaTime * 1.0f));
            yield return null;
        }
    }

    private IEnumerator FadeInText(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);

        text.color = new Color(text.color.r, text.color.g, text.color.b, 0);

        while (text.color.a < 1.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a + (Time.deltaTime * 1.0f));
            yield return null;
        }
    }

    private IEnumerator FadeOutText(TextMeshProUGUI text, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (text.GetComponentInParent<Button>() != null)
        {
            text.GetComponentInParent<Button>().interactable = false;
        }

        text.color = new Color(text.color.r, text.color.g, text.color.b, 1);

        while (text.color.a > 0.0f)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, text.color.a - (Time.deltaTime * 1.0f));
            yield return null;
        }

        Destroy(text.gameObject);
    }
}
