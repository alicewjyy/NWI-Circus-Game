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

    private bool isTyping = false;

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

            yield return new WaitForSeconds(lineDelay);

            storyText.text = "";
        }

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene("Game");
    }

    IEnumerator TypeLine(string line)
    {
        isTyping = true;

        storyText.text = "";

        foreach (char c in line)
        {
            storyText.text += c;
            yield return new WaitForSeconds(characterSpeed);
        }

        isTyping = false;
    }

    IEnumerator FadeOut()
    {
        float t = 0;

        while (t < fadeTime)
        {
            t += Time.deltaTime;

            blackScreen.alpha = 1 - t / fadeTime;

            yield return null;
        }

        blackScreen.alpha = 0;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isTyping)
        {
            StopAllCoroutines();

            StartCoroutine(PlayIntroSkipCurrent());
        }
    }

    IEnumerator PlayIntroSkipCurrent()
    {
        isTyping = false;

        yield return null;
    }
}