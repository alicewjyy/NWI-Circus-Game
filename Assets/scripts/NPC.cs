using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    public DialogueManager dialogueManager;

    public DialogueManager.DialogueLine[] dialogue;

    public GameObject talkPrompt;

    private bool playerNearby = false;


    void Start()
    {
        talkPrompt.SetActive(false);
    }


    void Update()
    {
        if (playerNearby && Input.GetKeyDown(KeyCode.F))
        {
            // Prevent the player from starting the dialogue multiple times
            if (dialogueManager.IsDialogueActive)
                return;

            talkPrompt.SetActive(false);

            dialogueManager.StartDialogue(dialogue);
        }
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = true;

            talkPrompt.SetActive(true);
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerNearby = false;

            talkPrompt.SetActive(false);
        }
    }
}