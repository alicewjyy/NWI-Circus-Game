using UnityEngine;
using TMPro;

public class NPCInteractionHint : MonoBehaviour
{
    [SerializeField] private GameObject interactionHint;

    private void Start()
    {
        interactionHint.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionHint.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            interactionHint.SetActive(false);
        }
    }
}