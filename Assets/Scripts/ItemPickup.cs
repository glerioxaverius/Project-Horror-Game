using UnityEngine;

public class ItemPickup : MonoBehaviour, IInteractable
{
    [SerializeField] private ItemData itemData; 
    
    [SerializeField] private GameObject interactUI; 

    public string InteractionPrompt => $"Ambil {itemData.itemName}";

    private void Start()
    {
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

    public void ShowUI(bool state)
    {
        if (interactUI != null)
        {
            interactUI.SetActive(state);
        }
    }
}