using UnityEngine;

public class WebViewBtn_2 : MonoBehaviour
{
    [SerializeField] private RectTransform rt;

    public void Load(string url)
    {
        WebView.LoadUrlOnUI(url, rt);
    }

    public void Show()
    {
        WebView.Show();
    }

    public void Hide()
    {
        WebView.Hide();
    }
    
    public void Destroy()
    {
        WebView.Destroy();
    }
    
    public void Back()
    {
        WebView.GoBack();
    }

    public void Forward()
    {
        WebView.GoForward();
    }
}
