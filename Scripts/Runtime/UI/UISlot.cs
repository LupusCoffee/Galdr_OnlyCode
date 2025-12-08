// made by Isabelle H Heiskanen
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlot : MonoBehaviour {
    
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image itemImage;

    private void Awake() {
        nameText.text = "";
        descriptionText.text = "";
        itemImage.sprite = null;
    }

    public void SetInformation(string name, string description, Sprite sprite) {
        nameText.text = name;
        descriptionText.text = description;
        itemImage.sprite = sprite;
    }
}
