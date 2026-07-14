using UnityEngine;

public class MasterKey : MonoBehaviour, IInteractable
{
    [SerializeField] private float healAmount = 25f;
    public string InteractionPrompt => "Ambil Master Key";

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerController player))
        {
            player.Heal(healAmount);
            
            Destroy(gameObject);
        }
    }
}