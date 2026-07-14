using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject visualPanel; 

    private List<InventorySlot> _uiSlots = new List<InventorySlot>();
    private bool _isOpen = false;

    public bool IsOpen => _isOpen;

    private void Start()
    {
        if (gridParent != null)
        {
            foreach (Transform child in gridParent)
            {
                InventorySlot slot = child.GetComponent<InventorySlot>();
                if (slot != null) _uiSlots.Add(slot);
            }
        }

        _isOpen = false;
        if (visualPanel != null) visualPanel.SetActive(false);
    }

    public void ToggleInventory()
    {
        _isOpen = !_isOpen;
        
        if (visualPanel != null)
        {
            visualPanel.SetActive(_isOpen);
        }

        if (_isOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void UpdateUI(List<ItemData> currentItems)
    {
        for (int i = 0; i < _uiSlots.Count; i++) _uiSlots[i].ClearSlot();

        for (int i = 0; i < currentItems.Count; i++)
        {
            if (i < _uiSlots.Count) _uiSlots[i].DisplayItem(currentItems[i]);
        }
    }
}