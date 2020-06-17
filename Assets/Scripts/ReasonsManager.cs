using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;

public class ReasonsManager : MonoBehaviour
{
    public static ReasonsManager instance;

    private int reasons;
    private string button;
    private Image[] frames;
    private Button[] buttons;
    private bool fixRestart, loaded;
    [SerializeField] private Sprite locked;
    [SerializeField] private Button reason;
    [SerializeField] private Sprite[] locks;
    [SerializeField] private Sprite[] photos;
    [SerializeField] private string[] messages;
    [SerializeField] private Image reasonImage;
    [SerializeField] private TextMeshProUGUI messageText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        SceneManager.sceneLoaded += this.LoadScene;
    }

    private void LoadScene(Scene scene, LoadSceneMode mode)
    {
        this.button = "";
        this.loaded = true;
        this.fixRestart = true;
        this.reasons = this.Load();
        this.buttons = new Button[100];
        this.frames = this.SetFrames();
    }

    private Image[] SetFrames()
    {
        Image[] images = new Image[4];

        for (int i = 0; i < 4; i++)
        {
            images[i] = GameObject.Find("Frame " + (i + 1).ToString()).GetComponent<Image>();
            images[i].enabled = false;
        }

        return images;
    }

    private void Save()
    {
        PlayerPrefs.SetInt("reasons", this.reasons);
    }

    private int Load()
    {
        if (PlayerPrefs.HasKey("reasons"))
        {
            return PlayerPrefs.GetInt("reasons");
        } 
        else
        {
            return 1;
        }
    }

    public void StartShowReasons()
    {
        if (fixRestart)
        {
            StopAllCoroutines();
            StartCoroutine(this.ShowReasons());
        }
    }

    public IEnumerator ShowReasons()
    {
        this.fixRestart = false;

        yield return new WaitForSeconds(1.0f);

        TextMeshProUGUI hundred = GameObject.Find("100 (1)").GetComponent<TextMeshProUGUI>();
        TextMeshProUGUI reasons = GameObject.Find("Reasons (1)").GetComponent<TextMeshProUGUI>();

        hundred.DOFade(1.0f, 1.0f);
        reasons.DOFade(1.0f, 1.0f);
        
        StartCoroutine(Shaker.instance.ShakeText(hundred, 1.0f));
        StartCoroutine(Shaker.instance.ShakeText(reasons, 1.0f));

        yield return new WaitForSeconds(1.0f);

        Image loading = GameObject.Find("Loading Reasons").GetComponent<Image>();
        loading.DOFade(1.0f, 0.0f);
        loading.fillAmount = 0.0f;

        for (int i = 0; i < 100; i++)
        {
            this.buttons[i] = Instantiate(this.reason, Vector3.zero, Quaternion.identity) as Button;
            this.SetButton(this.buttons[i], i);

            if (i < this.reasons)
            {
                this.AnimateButtonHeart(this.buttons[i], i);
            }
            else
            {
                this.AnimateButtonLock(this.buttons[i], i);
            }

            loading.fillAmount += 0.01f;

            yield return new WaitForSeconds(Mathf.Max(0.0f, 0.5f - (0.05f * i)));
        }

        loading.DOFade(0.0f, 0.5f);

        yield return new WaitForSeconds(0.5f);

        for (int i = 0; i < this.reasons; i++)
        {
            Button button = GameObject.Find("Reason " + (i + 1).ToString()).GetComponent<Button>();
            this.buttons[i].onClick.AddListener(() => this.ShowReason(button));
        }
    }

    private void SetButton(Button button, int index)
    {
        float x = 88.0f + ((index % 4) * 165.0f); //-245.5 + 165.0f
        float y = -420.0f - (((int) (index / 4)) * 150.0f); // 1640.0f - 150.0f

        button.enabled = (index < this.reasons);
        button.name = "Reason " + (index + 1).ToString();
        button.GetComponent<RectTransform>().SetParent(GameObject.Find("Content").GetComponent<RectTransform>());
        button.GetComponent<RectTransform>().localScale = new Vector3(0.75f, 0.75f, 0.75f);
        button.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);
        button.GetComponent<RectTransform>().DOAnchorPos(new Vector3(x, y, 0.0f), 0.0f);
    }

    private void AnimateButtonHeart(Button button, int index)
    {
        Image image = button.GetComponent<Image>();
        TextMeshProUGUI text = button.GetComponentInChildren<TextMeshProUGUI>();
        
        text.text = (index + 1).ToString();
        text.DOFade(1.0f, 1.0f);
        image.DOFade(1.0f, 1.0f);
        
        StartCoroutine(Shaker.instance.ShakeText(text, 1.0f));
        StartCoroutine(Shaker.instance.ShakeImage(image));
    }

    private void AnimateButtonLock(Button button, int index)
    {
        Image image = button.GetComponent<Image>();
        image.sprite = this.locked;
        image.DOFade(1.0f, 1.0f);

        string name = ((int.Parse(button.name.Split(' ')[1])) - 1).ToString();
        if (!name.Equals(this.button))
        {
            StartCoroutine(Shaker.instance.SwingImage(image));
        }
    }

    public void ShowReason(Button button)
    {
        this.button = button.name.Split(' ')[1];
        button.enabled = false;
        button.GetComponentInChildren<TextMeshProUGUI>().enabled = false;

        Image image = button.GetComponent<Image>();
        image.rectTransform.DOAnchorPos(new Vector3(350.0f, -400.0f - (27.0f * int.Parse(this.button) - 1), 0.0f), 1.0f);
        image.rectTransform.DOScale(new Vector3(21.0f, 21.0f, 1.0f), 1.0f);

        StopAllCoroutines();

        GameObject.Find("100 (1)").GetComponent<TextMeshProUGUI>().DOFade(0.0f, 0.0f);
        GameObject.Find("Reasons (1)").GetComponent<TextMeshProUGUI>().DOFade(0.0f, 0.0f);

        foreach (Button btn in GameObject.FindObjectsOfType<Button>())
        {
            if (!btn.name.Equals(button.name) && !btn.name.Equals("Back"))
            {
                Destroy(btn.gameObject);
            }
        }

        StartCoroutine(this.HeartAnimation(image));
        StartCoroutine(this.Show(button));
    }

    private IEnumerator HeartAnimation(Image image)
    {
        yield return new WaitForSeconds(1.0f);
        image.rectTransform.DOScale(Vector3.zero, 1.0f);
        image.rectTransform.DOAnchorPos(new Vector2(350.0f, -600.0f - (27.0f * int.Parse(this.button) - 1)), 1.0f);

        yield return new WaitForSeconds(1.0f);
        Destroy(image.gameObject);
    }

    private IEnumerator Show(Button button)
    {
        yield return new WaitForSeconds(2.0f);

        this.messageText.enabled = true;
        this.messageText.text = this.messages[int.Parse(this.button) - 1];
        this.messageText.DOFade(1.0f, 1.0f);
        StartCoroutine(Shaker.instance.ShakeText(this.messageText, 1.0f));

        yield return new WaitForSeconds(1.0f);

        for (int i = 0; i < 4; i++)
        {
            this.frames[i].enabled = true;
            this.frames[i].DOFade(0.8f, 1.0f);
        }

        yield return new WaitForSeconds(0.5f);

        this.reasonImage.enabled = true;
        this.reasonImage.sprite = this.photos[int.Parse(this.button) - 1];
        this.reasonImage.DOFade(1.0f, 1.0f);

        yield return new WaitForSeconds(1.0f);

        Button back = GameObject.Find("Back").GetComponent<Button>();
        back.GetComponentInChildren<TextMeshProUGUI>().DOFade(1.0f, 1.0f);
        StartCoroutine(Shaker.instance.ShakeText(back.GetComponentInChildren<TextMeshProUGUI>(), 1.0f));

        yield return new WaitForSeconds(1.0f);

        back.interactable = true;
        back.onClick.AddListener(() => { StartCoroutine(this.BackToShowReasons()); });
    }

    public IEnumerator BackToShowReasons()
    {
        GameObject.Find("Back").GetComponentInChildren<TextMeshProUGUI>().DOFade(0.0f, 1.0f);
        GameObject.Find("Back").GetComponent<Button>().interactable = false;

        this.messageText.DOFade(0.0f, 1.0f);

        yield return new WaitForSeconds(1.0f);

        this.reasonImage.DOFade(0.0f, 1.0f);
        
        for (int i = 0; i < 4; i++)
        {
            this.frames[i].DOFade(0.0f, 1.0f);
        }

        yield return new WaitForSeconds(1.0f);

        this.messageText.enabled = false;

        for (int i = 0; i < 4; i++)
        {
            this.frames[i].enabled = false;
        }

        this.reasonImage.enabled = false;

        RectTransform content = GameObject.Find("Content").GetComponent<RectTransform>();
        content.offsetMin = new Vector2(content.offsetMin.x, -2760);
        content.offsetMax = new Vector2(content.offsetMax.x, 0.0f);

        this.fixRestart = true;
        this.StartShowReasons();

        if (this.button.Equals((this.reasons).ToString()))
        {
            StartCoroutine(this.LockAnimation());
        }
    }

    private IEnumerator LockAnimation()
    {
        yield return new WaitForSeconds(6.0f + (0.04f * int.Parse(this.button))); // Work on it
        this.Pause();

        Image image = this.buttons[this.reasons].GetComponent<Image>();
        Sprite heart = this.buttons[this.reasons - 1].GetComponent<Image>().sprite;

        for (int i = 0; i < 4; i++)
        {   
            image.DOFade(0.1f, 1.0f);
            yield return new WaitForSeconds(1.0f);
            image.DOFade(1.0f, 1.0f);
            image.sprite = this.locks[i];
            if (i == 2)
            {
                this.GetComponent<AudioSource>().Play();
            }
            yield return new WaitForSeconds(1.25f);
        }
        
        image.DOFade(0.1f, 1.0f);
        yield return new WaitForSeconds(1.0f);
        image.DOFade(1.0f, 1.0f);
        image.sprite = heart;

        TextMeshProUGUI text = this.buttons[this.reasons].GetComponentInChildren<TextMeshProUGUI>();
        text.text = (this.reasons + 1).ToString();
        text.DOFade(1.0f, 1.0f);
        
        yield return new WaitForSeconds(1.0f);

        StartCoroutine(Shaker.instance.ShakeText(text, 0.0f));
        StartCoroutine(Shaker.instance.ShakeImage(image));

        int reason = this.reasons;
        this.buttons[this.reasons].onClick.AddListener(() => this.ShowReason(this.buttons[reason]));
        this.buttons[this.reasons].enabled = true;

        this.reasons = Mathf.Min(100, this.reasons + 1);
        this.Save();

        this.Resume();
    }

    private void Pause()
    {
        for (int i = 0; i < this.reasons; i++)
        {
            Button button = GameObject.Find("Reason " + (i + 1).ToString()).GetComponent<Button>();
            this.buttons[i].onClick.AddListener(() => {});
        }
    }

    private void Resume()
    {
        for (int i = 0; i < this.reasons; i++)
        {
            Button button = GameObject.Find("Reason " + (i + 1).ToString()).GetComponent<Button>();
            this.buttons[i].onClick.AddListener(() => this.ShowReason(button));
        }
    }
}
