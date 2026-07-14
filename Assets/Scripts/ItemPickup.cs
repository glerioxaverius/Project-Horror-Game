using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData; 
    
    // 🟥 Tarik objek InteractCanvas (World Space) ke kolom ini di Inspector
    [SerializeField] private GameObject interactUI; 

    public string InteractionPrompt => $"Ambil {itemData.itemName}";

    private void Start()
    {
        // Pastikan UI mati saat pertama kali game berjalan
        if (interactUI != null) interactUI.SetActive(false);
    }

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out InventoryManager inventory))
        {
            inventory.AddItem(itemData);
            Destroy(gameObject);
        }
    }

    // 🟥 FUNGSI BARU: Dipanggil oleh PlayerController saat laser menatap objek ini
    public void ShowUI(bool state)
    {
        if (interactUI != null)
        {
            interactUI.SetActive(state);
        }
    }
}