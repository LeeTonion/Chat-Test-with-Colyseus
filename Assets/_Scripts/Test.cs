using UnityEngine;
using UnityEngine.UI;

public class Test : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Button X;
    [SerializeField] private GameObject Bar;
    private WebViewObject webViewObject;
    private bool isWebViewVisible = false; // Trạng thái hiển thị WebView

    void Start()
    {
        button.onClick.AddListener(ToggleWebView);
        X.onClick.AddListener(OnClickX);

#if UNITY_EDITOR
        Debug.Log("WebView không chạy trong Unity Editor. Vui lòng thử trên Android/iOS.");
#else
        // Khởi tạo WebViewObject
        webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
        webViewObject.Init();

        // Load video YouTube với iframe
        string videoUrl = "https://www.youtube.com/embed/6VWxwL-q9O0?start=0&autoplay=1&controls=0&modestbranding=1&rel=0&showinfo=0";
        webViewObject.LoadURL(videoUrl);

        // Giới hạn kích thước WebView
        int left = 0;   
        int top = 0;   
        int right = 0;  
        int bottom = 0; 

        webViewObject.SetMargins(left, top, right, bottom);
        webViewObject.SetVisibility(false); // Ban đầu ẩn WebView
#endif
    }

    public void ToggleWebView()
    {
        if (webViewObject != null)
        {
            isWebViewVisible = !isWebViewVisible; // Đảo trạng thái hiển thị
            Bar.SetActive(isWebViewVisible); // Ẩn/hiện Button
            webViewObject.SetVisibility(isWebViewVisible);
            Debug.Log("WebView " + (isWebViewVisible ? "đã bật" : "đã tắt"));
        }
    }
    public void OnClickX()
    {
        Bar.SetActive(false); // Ẩn Button
        webViewObject.SetVisibility(false);

    }
}
