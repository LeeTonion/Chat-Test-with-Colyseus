using NUnit.Framework;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class EmojiManager : MonoBehaviour
{
    [SerializeField] private Transform contentEmoji;
    [SerializeField] private Transform contentGif;
    [SerializeField] private GameObject emojiPrefab;
    [SerializeField] private GameObject gifPrefab;
    [SerializeField] private new List<Button> ButtonItemList = new List<Button>();
    [SerializeField] private GameObject IconAndGif;
    private string EmojiPath = "Icon"; 
    private string GifPath = "Gif"; 
    private TMP_SpriteAsset[] spriteAssetsEmoji; 
    private TMP_SpriteAsset[] spriteAssetsGif; 
    public static EmojiManager Instance { get; private set; }


    private void Start()
    {
        Instance = this;
        spriteAssetsEmoji = Resources.LoadAll<TMP_SpriteAsset>(EmojiPath);
        spriteAssetsGif = Resources.LoadAll<TMP_SpriteAsset>(GifPath);

        AddEmoji();
        AddGif();
    }
    private void Update()
    {

    }

    public void AddEmoji()
    {
        for (int i = 0; i < spriteAssetsEmoji.Length; i++)
        {
            GameObject newEmojiObj = Instantiate(emojiPrefab, contentEmoji);
            TextMeshProUGUI emojiText = newEmojiObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            int spriteCount = spriteAssetsEmoji[i].spriteCharacterTable.Count;
            if (spriteCount > 1)
            {
                emojiText.text = $"<sprite=\"{spriteAssetsEmoji[i].name}\" anim=\"0,{spriteCount - 1},50\">";
            }
            else
            {
                emojiText.text = "<sprite=0>";
            }
        }
    }
    public void AddGif()
    {
        for (int i = 0; i < spriteAssetsGif.Length; i++)
        {
            GameObject newEmojiObj = Instantiate(gifPrefab, contentGif);
            TextMeshProUGUI emojiText = newEmojiObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            int spriteCount = spriteAssetsGif[i].spriteCharacterTable.Count;
            if (spriteCount > 1)
            {
                emojiText.text = $"<sprite=\"{spriteAssetsGif[i].name}\" anim=\"0,{spriteCount - 1},50\">";
            }
            else
            {
                emojiText.text = "<sprite=0>";
            }
        }
    }
    public void EmojiButton()
    {
        InteractableButton(ButtonItemList[0]);
        contentEmoji.transform.parent.parent.gameObject.SetActive(true);
        contentGif.transform.parent.parent.gameObject.SetActive(false);
    }
    public void GifButton()
    {
        InteractableButton(ButtonItemList[1]);
        contentEmoji.transform.parent.parent.gameObject.SetActive(false);
        contentGif.transform.parent.parent.gameObject.SetActive(true);
    }
    public void IconAndGifButton()
    {
        if (IconAndGif.activeSelf)
        {
            HideIconAndGifButton();
        }
        else
        {
            ShowIconAndGifButton();
        }
    }
    public  void ShowIconAndGifButton()
    {
        IconAndGif.SetActive(true);
    }
    public void HideIconAndGifButton()
    {
        IconAndGif.SetActive(false);
    }
    private void InteractableButton(Button Btn)
    {
        foreach (var item in ButtonItemList)
        {
            if (item == Btn)
            {
                item.interactable = false;
            }
            else
            {
                item.interactable = true;
            }

        }
    }
}
