using UnityEngine;

public class WebViewBtn : MonoBehaviour
{
    [SerializeField] private GameObject m_panelPrefab;
    [SerializeField] private Transform m_panelParent;
    private GameObject m_panel_instance;
    
    public void ShowPanel()
    {
        if (m_panel_instance == null)
        {
            m_panel_instance = Instantiate(m_panelPrefab, m_panelParent, false);
        }
    }
}
