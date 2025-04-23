using System.Collections;
using UnityEngine;
using UnityEngine.Networking;


public class GiphyAPI : MonoBehaviour
{
    public string APIKey = "TatL3oZxffwhU66y3BJkzTzKUsG7kECv";
    public string SearchQuery = "happy";

    [SerializeField] private UniGifImage gifDisplay; // Thay RawImage bằng UniGifImage

    void Start()
    {
        StartCoroutine(GetGiphyData());
    }

    IEnumerator GetGiphyData()
    {
        string url = $"https://api.giphy.com/v1/gifs/search?api_key={APIKey}&q={SearchQuery}&limit=1&offset=0&rating=g&lang=en";
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error: {webRequest.error}");
                yield break;
            }

            string jsonResponse = webRequest.downloadHandler.text;
            Debug.Log($"Response: {jsonResponse}");

            GiphyResponse response = JsonUtility.FromJson<GiphyResponse>(jsonResponse);

            if (response.data.Length > 0)
            {
                string gifUrl = response.data[0].images.original.url; // Dùng GIF động
                Debug.Log($"GIF URL: {gifUrl}");
                //StartCoroutine(LoadGif(gifUrl));
            }
            else
            {
                Debug.LogError("No GIFs found in response.");
            }
        }
    }

    /*   IEnumerator LoadGif(string gifUrl)
       {
           yield return gifDisplay.SetGifFromUrl(gifUrl); // UniGif tải và phát GIF
       }
   }*/

    [System.Serializable]
    public class GiphyResponse
    {
        public GiphyData[] data;
    }

    [System.Serializable]
    public class GiphyData
    {
        public GiphyImages images;
    }

    [System.Serializable]
    public class GiphyImages
    {
        public GiphyImage original;
        public GiphyImage original_still;
    }

    [System.Serializable]
    public class GiphyImage
    {
        public string url;
    }
}