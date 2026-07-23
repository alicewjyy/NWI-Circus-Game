using TMPro;
using UnityEngine;
using System.Collections;

public class Typewriter : MonoBehaviour
{
    public TMP_Text storyText;

    [TextArea]
    public string story;

    public float speed = 0.05f;

    IEnumerator Start()
    {
        storyText.text = "";

        foreach (char c in story)
        {
            storyText.text += c;
            yield return new WaitForSeconds(speed);
        }
    }
}