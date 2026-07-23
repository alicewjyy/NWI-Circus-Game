using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    public DialogueManager dialogueManager;

    public DialogueManager.DialogueLine[] dialogue;

    public Button talkButton;

    private bool playerNearby = false;

    void Start()
    {
        talkButton.gameObject.SetActive(false);

        talkButton.onClick.AddListener(StartConversation);
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNearby = true;
            talkButton.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            playerNearby = false;
            talkButton.gameObject.SetActive(false);
        }
    }

    void StartConversation()
    {
        if(playerNearby)
        {
            talkButton.gameObject.SetActive(false);
            dialogueManager.StartDialogue(dialogue);
        }
    }
}