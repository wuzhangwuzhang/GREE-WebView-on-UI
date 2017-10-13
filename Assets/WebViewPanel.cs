using UnityEngine;
using UnityEngine.UI;

public class WebViewPanel : MonoBehaviour
{
    [SerializeField] private string m_url;
    [SerializeField] private RectTransform m_viewArea;
    private Animator m_animator;
    
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void LoadWebView()
    {
        WebView.LoadUrlOnUI(m_url, m_viewArea);
    }

    public void ClosePanel()
    {
        WebView.Destroy();
        m_animator.SetTrigger("Close");

        var btns = GetComponentsInChildren<Button>();
        foreach (var btn in btns)
        {
            btn.interactable = false;
        }
    }

    public void DestroyObject()
    {
        Destroy(gameObject);
    }
}
