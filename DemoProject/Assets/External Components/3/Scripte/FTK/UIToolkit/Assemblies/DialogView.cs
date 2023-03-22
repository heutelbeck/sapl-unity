using FTK.UIToolkit.Primitives;
using UnityEngine;
using UnityEngine.Events;

public class DialogView : MonoBehaviour
{

    [SerializeField]
    private string title = "title";
    [TextArea]
    [SerializeField]
    private string message = "message";

    private TextPanelView titleTextPanel;
    private TextPanelView messageTextPanel; 

    public string TitelText
    {
        get => title;
        set
        {
            title = value;
            UpdateTitleText();
        }
    }

    public string MessageText
    {
        get => message;
        set
        {
            message = value;
            UpdateMessageText();
        }
    }

    public void DestroyGameObject()
    {
        Destroy(gameObject);
    }

    private void OnValidate()
    {
        UpdateTitleText();
        UpdateMessageText();
    }

    private void UpdateTitleText()
    {
        if (titleTextPanel == null)
        {
            titleTextPanel = transform.Find("VerticalLayout/Title[TextPanel]")?.GetComponent<TextPanelView>();
            if (titleTextPanel == null)
                Debug.LogError("No titleText child with TextPanelView attached!");
        }
        titleTextPanel.Text = title;
    }

    private void UpdateMessageText()
    {
        if (messageTextPanel == null)
        {
            messageTextPanel = transform.Find("VerticalLayout/Message[TextPanel]")?.GetComponent<TextPanelView>();
            if (titleTextPanel == null)
                Debug.LogError("No titleTextPanel child with TextPanelView attached!");
        }
        messageTextPanel.Text = message;
    }

}
