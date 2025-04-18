using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GifButton : MonoBehaviour
{
    public TextMeshProUGUI gifText;
    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => {
            //ChatUIManager.Instance.OnGifButtonClicked(gifText.text);
        });
         
    }
}
