using UnityEngine;

public class Medkit : MonoBehaviour, IInteractable
{
    [SerializeField] private float healAmount = 25f;
    public string InteractionPrompt => "Ambil Medkit";

    public void Interact(GameObject interactor)
    {
        // Cek apakah yang berinteraksi adalah Player
        if (interactor.TryGetComponent(out PlayerController player))
        {
            // Panggil fungsi heal yang akan kita buat di bawah
            player.Heal(healAmount);
            
            // Hancurkan medkit setelah diambil
            Destroy(gameObject);
        }
    }
}