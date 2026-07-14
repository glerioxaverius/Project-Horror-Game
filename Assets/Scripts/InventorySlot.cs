using UnityEngine;
using UnityEngine.UI; // Wajib untuk komponen Image

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image iconImage; // Tarik objek anak 'ItemIcon' ke sini

    private void Awake()
    {
        ClearSlot();
    }

    // Fungsi untuk memunculkan gambar item
    public void DisplayItem(ItemData item)
    {
        if (item == null || iconImage == null) return;

        iconImage.sprite = item.itemIcon;
        iconImage.enabled = true; 
    }

    public void ClearSlot()
    {
        if (iconImage == null) return;

        iconImage.sprite = null;
        iconImage.enabled = false;
    }
}