using UnityEngine;

public class Medkit : MonoBehaviour, IInteractable
{
    [SerializeField] private float healAmount = 25f;
    public string InteractionPrompt => "Ambil Medkit";

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerController player))
        {
            player.Heal(healAmount);
            
            Destroy(gameObject);
        }
    }
}