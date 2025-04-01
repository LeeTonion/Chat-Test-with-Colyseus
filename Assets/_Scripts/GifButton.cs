using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GifButton : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Button>().onClick.AddListener(() => {
            ChatManager.Instance.OnGifButtonClicked(gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
        });
         ;
    }
}
