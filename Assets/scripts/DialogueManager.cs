using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueChoice
    {
        public string choiceText;
        public bool isCorrect;
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        public Sprite portrait;

        [TextArea(3, 5)]
        public string dialogue;

        [Header("Choices")]
        public bool hasChoices = false;

        public DialogueChoice[] choices;

        [Header("NPC Response When Answer Is Wrong")]
        [TextArea(2, 4)]
        public string wrongAnswerResponse = "I'm sorry.";
    }

    [Header("UI")]
    public GameObject dialoguePanel;
    public Image portraitImage;
    public TMP_Text nameText;
    public TMP_Text dialogueText;

    [Header("Choice UI")]
    public GameObject choicePanel;

    public Button choiceButton1;
    public Button choiceButton2;

    public TMP_Text choiceText1;
    public TMP_Text choiceText2;

    [Header("Dialogue")]
    public DialogueLine[] dialogueLines;

    [Header("Settings")]
    public bool autoStartDialogue = false;
    public float typingSpeed = 0.03f;

    public bool IsDialogueActive { get; private set; } = false;

    private int currentIndex = 0;
    private bool isTyping = false;
    private bool waitingForChoice = false;

    private Coroutine typingCoroutine;


    void Start()
    {
        // Hide the choice UI at the beginning
        choicePanel.SetActive(false);

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
        if (!IsDialogueActive)
            return;

        // Do not allow normal dialogue input while waiting for a choice
        if (waitingForChoice)
            return;

        if (Input.GetKeyDown(KeyCode.Space) ||
            Input.GetMouseButtonDown(0))
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


    // Called by the NPC to start the dialogue
    public void StartDialogue(DialogueLine[] newDialogue)
    {
        IsDialogueActive = true;

        dialogueLines = newDialogue;

        currentIndex = 0;

        dialoguePanel.SetActive(true);

        choicePanel.SetActive(false);
        
        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;

        ShowLine();
    }


    void ShowLine()
    {
        DialogueLine line = dialogueLines[currentIndex];

        nameText.text = line.speakerName;

        portraitImage.sprite = line.portrait;

        // Make sure the choice UI is hidden
        choicePanel.SetActive(false);

        waitingForChoice = false;

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

        // Show choices after the dialogue finishes typing
        ShowChoicesIfNeeded();
    }


    void FinishTyping()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = dialogueLines[currentIndex].dialogue;

        isTyping = false;

        // Show choices immediately if this line has choices
        ShowChoicesIfNeeded();
    }


    void ShowChoicesIfNeeded()
    {
        DialogueLine line = dialogueLines[currentIndex];

        if (!line.hasChoices)
            return;

        if (line.choices == null || line.choices.Length < 2)
        {
            Debug.LogWarning(
                "This DialogueLine has choices enabled, but at least two choices have not been set!"
            );

            return;
        }

        waitingForChoice = true;

        choicePanel.SetActive(true);

        // Set the first button
        choiceText1.text = line.choices[0].choiceText;

        // Set the second button
        choiceText2.text = line.choices[1].choiceText;

        // Remove previous button events
        choiceButton1.onClick.RemoveAllListeners();
        choiceButton2.onClick.RemoveAllListeners();

        // Add new button events
        choiceButton1.onClick.AddListener(() =>
{
    Debug.Log("BUTTON 1");
    ChooseAnswer(0);
});

choiceButton2.onClick.AddListener(() =>
{
    Debug.Log("BUTTON 2");
    ChooseAnswer(1);
});
    }


    // Called when the player selects an answer
    void ChooseAnswer(int choiceIndex)
    {
        if (!waitingForChoice)
            return;

        DialogueLine line = dialogueLines[currentIndex];

        waitingForChoice = false;

        choicePanel.SetActive(false);

        // Check whether the answer is correct
        if (line.choices[choiceIndex].isCorrect)
        {
            Debug.Log("Player chose the correct answer.");

            currentIndex++;

            // If there is another dialogue line
            if (currentIndex < dialogueLines.Length)
            {
                ShowLine();
            }
            else
            {
                EndDialogue();
            }
        }
        else
        {
            Debug.Log("Player chose the wrong answer.");

            StartCoroutine(WrongAnswer());
        }
    }


    IEnumerator WrongAnswer()
{
    DialogueLine line = dialogueLines[currentIndex];

    // Show wrong answer response
    nameText.text = line.speakerName;
    portraitImage.sprite = line.portrait;

    dialogueText.text = "";

    isTyping = true;

    foreach (char c in line.wrongAnswerResponse)
    {
        dialogueText.text += c;

        yield return new WaitForSeconds(typingSpeed);
    }

    isTyping = false;

    // IMPORTANT:
    // Wait until the current mouse click has been released.
    yield return new WaitUntil(() => !Input.GetMouseButton(0));

    // Now wait for a NEW click or Space press.
    while (!Input.GetKeyDown(KeyCode.Space) &&
           !Input.GetMouseButtonDown(0))
    {
        yield return null;
    }

    RestartScene();
}


    void NextLine()
    {
        currentIndex++;

        if (currentIndex >= dialogueLines.Length)
        {
            EndDialogue();

            return;
        }

        ShowLine();
    }


    void EndDialogue()
    {
        dialoguePanel.SetActive(false);

        choicePanel.SetActive(false);

        IsDialogueActive = false;

        currentIndex = 0;
    }


    void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.buildIndex);
    }
}