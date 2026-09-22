using UnityEngine;

public class NPCDelivery : MonoBehaviour
{
    public DialogueManager dialogueManager;

    public DialogueManager.DialogueLine[] completionDialogue;

    private bool delivered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (delivered)
            return;

        CollectableFollower collectable =
            other.GetComponent<CollectableFollower>();

        if (collectable != null && collectable.IsCollected)
        {
            delivered = true;

            dialogueManager.StartDialogue(completionDialogue);

            Destroy(other.gameObject);
        }
    }
}