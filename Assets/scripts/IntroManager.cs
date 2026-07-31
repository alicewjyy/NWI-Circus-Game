using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text storyText;
    public CanvasGroup blackScreen;

    [Header("Story")]
    [TextArea]
    public string[] storyLines;

    public float characterSpeed = 0.04f;
    public float lineDelay = 1.5f;
    public float fadeTime = 2f;

    [Header("Click Speed Up")]
    public float fastMultiplier = 8f;

    private bool isTyping = false;
    private bool isAccelerating = false;
    private bool skipCurrentLine = false;

    void Start()
    {
        StartCoroutine(PlayIntro());
    }

    IEnumerator PlayIntro()
    {
        storyText.text = "";

        foreach (string line in storyLines)
        {
            yield return StartCoroutine(TypeLine(line));

            
            if (skipCurrentLine)
            {
                skipCurrentLine = false;
                storyText.text = "";
                continue;
            }

            yield return new WaitForSeconds(lineDelay);
            storyText.text = "";
        }

        yield return new WaitForSeconds(1f);

yield return StartCoroutine(FadeToBlack());

SceneManager.LoadScene("Game");
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;
        isAccelerating = false;
        skipCurrentLine = false;
        storyText.text = "";

        foreach (char c in line)
        {
            if (skipCurrentLine)
            {
                storyText.text = line;
                break;
            }

            storyText.text += c;

            float speed = isAccelerating
                ? characterSpeed / fastMultiplier
                : characterSpeed;

            yield return new WaitForSeconds(speed);
        }

        isTyping = false;
    }

    IEnumerator FadeToBlack()
    {
    float t = 0f;

    while (t < fadeTime)
    {
        t += Time.deltaTime;
        blackScreen.alpha = t / fadeTime;
        yield return null;
    }

    blackScreen.alpha = 1f;
    }
    void Update()
    {
        if (!Input.GetMouseButtonDown(0) || !isTyping)
            return;

        if (!isAccelerating)
        {
           
            isAccelerating = true;
        }
        else
        {
        
            skipCurrentLine = true;
        }
    }
}