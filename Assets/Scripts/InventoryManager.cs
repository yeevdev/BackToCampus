using UnityEngine;
using UnityEngine.UI;
public class InventoryManager : MonoBehaviour
{
    public Image[] ItemIcon;
    public void AddItem(int slotIndex,Sprite itemSprite)
    {
        if (slotIndex < 0 || slotIndex >= ItemIcon.Length)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return;
        }
        ItemIcon[slotIndex].sprite = itemSprite;
        ItemIcon[slotIndex].enabled = true; 
    }
    public void RemoveItem(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= ItemIcon.Length)
        {
            Debug.LogError("Invalid slot index: " + slotIndex);
            return;
        }
        ItemIcon[slotIndex].sprite = null; 
        ItemIcon[slotIndex].enabled = false; 
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
