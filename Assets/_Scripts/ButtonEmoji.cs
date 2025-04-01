using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonEmoji : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI EmojiText;
    private TMP_InputField EmojiInputText;
    private TextMeshProUGUI EmojiInputSpriteAsset;

    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => {
            AddText();
        });

        EmojiInputText = GameObject.Find("InputField (TMP)").GetComponent<TMP_InputField>();
        EmojiInputSpriteAsset = GameObject.Find("TextInput").GetComponent<TextMeshProUGUI>();
    }
    private void AddText()
    {
        EmojiInputSpriteAsset.spriteAsset = EmojiText.spriteAsset;
        EmojiInputText.text += EmojiText.text;
        
    }
}
