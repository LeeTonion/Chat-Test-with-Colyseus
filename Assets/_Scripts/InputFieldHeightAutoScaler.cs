using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class InputFieldHeightAutoScaler : MonoBehaviour
{
    public TMP_Text tmpText; // Reference to the TMP_Text component
    public RectTransform tmpRectTransform;// Reference to the TMP_Text RectTransform
    public RectTransform RectTransform; // Reference to the parent RectTransform
    public float padding = 10f;
    public float itempadding = 10f;// Padding to adjust the height

    void Start()
    {
        // Check if tmpText has an input field component and add listener if it does
        TMP_InputField inputField = tmpText.GetComponentInParent<TMP_InputField>();
        if (inputField != null)
        {
            inputField.onValueChanged.AddListener(AdjustHeight);
        }
    }
    void Update()
    {
        // Only adjust height in Editor when not playing

      //  if (UnityEditor.EditorApplication.isPlaying) return;
        AdjustHeight(tmpText.text);
    }
    void AdjustHeight(string text)
    {
        // Get the preferred height of the TMP_Text
        Vector2 preferredValues = tmpText.GetPreferredValues();
        tmpRectTransform.sizeDelta = new Vector2(tmpRectTransform.sizeDelta.x, preferredValues.y + padding);
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, preferredValues.y + itempadding);
    }

#if UNITY_EDITOR
   /* void Update()
    {
        // Only adjust height in Editor when not playing
        
        if (UnityEditor.EditorApplication.isPlaying) return;
        AdjustHeight(tmpText.text);
    }*/
#endif


}