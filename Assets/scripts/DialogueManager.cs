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

        [TextArea(3, 5)]
        public string dialogue;
    }

    [Header("UI")]
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    [Header("Settings")]
    public bool autoStartDialogue = true;
    public float typingSpeed = 0.03f;

    // 给其他脚本读取（Movement）
    public bool IsDialogueActive { get; private set; } = false;

    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    void Start()
    {
        if (autoStartDialogue)
        {
            IsDialogueActive = true;

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
        // 没有開啟對話就不要處理輸入
        if (!IsDialogueActive)
            return;

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
        IsDialogueActive = true;

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

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    void FinishTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = dialogueLines[currentIndex].dialogue;
        isTyping = false;
    }

    void NextLine()
    {
        currentIndex++;

        if (currentIndex >= dialogueLines.Length)
        {
            dialoguePanel.SetActive(false);

            IsDialogueActive = false;

            currentIndex = 0;
            return;
        }

        ShowLine();
    }
}