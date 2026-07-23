using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;

        public Sprite portrait;

        [TextArea(3,5)]
        public string dialogue;
    }

    [Header("UI")]
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    public bool autoStartDialogue = true;

    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    public float typingSpeed = 0.03f;

    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

  void Start()
    {
    if (autoStartDialogue)
    {
        dialoguePanel.SetActive(true);
        currentIndex = 0;
        ShowLine();
    }
    else
    {
        dialoguePanel.SetActive(false);
    }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                FinishTyping();
            }
            else
            {
                NextLine();
            }
        }
    }

    public void StartDialogue(DialogueLine[] newDialogue)
    {
    dialogueLines = newDialogue;
    currentIndex = 0;

    dialoguePanel.SetActive(true);

    ShowLine();
    }

    void ShowLine()
    {
        DialogueLine line = dialogueLines[currentIndex];

        nameText.text = line.speakerName;
        portraitImage.sprite = line.portrait;

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(line.dialogue));
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;

        dialogueText.text = "";

        foreach(char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void FinishTyping()
    {
        StopCoroutine(typingCoroutine);

        dialogueText.text = dialogueLines[currentIndex].dialogue;

        isTyping = false;
    }

    void NextLine()
    {
        currentIndex++;

        if(currentIndex >= dialogueLines.Length)
        {
            dialoguePanel.SetActive(false);
            currentIndex = 0;
            return;
        }

        ShowLine();
    }
}