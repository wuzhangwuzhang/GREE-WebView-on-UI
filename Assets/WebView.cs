using System.Collections;
using UnityEngine;

public static class WebView
{
    //------------------------------ Public Properties ------------------------------//
    /// <summary>
    /// Can go back 1 step?
    /// </summary>
    public static bool canGoBack { get { return m_webViewObject != null ? m_webViewObject.CanGoBack() : false; } }
    /// <summary>
    /// Can go forward 1 step?
    /// </summary>
    public static bool canGoForward { get { return m_webViewObject != null ? m_webViewObject.CanGoForward() : false; } }

    //------------------------------ Public Members ------------------------------//
    /// <summary>
    /// Callback receiving "msg" on tag a href with command "unity:msg" and javascript "Unity.call('msg')"
    /// </summary>
    public static System.Action<string> unityCallback = DefaultUnityCallback;
    
    //------------------------------ Private Members ------------------------------//
    private static WebViewObject m_webViewObject;
    private static RectOffset m_margin = new RectOffset(10, Screen.width - 10, 10, Screen.height - 10);

    //------------------------------ Public Methods ------------------------------//
    #region Control
    /// <summary>
    /// Load and show <paramref name="url"/> page.
    /// </summary>
    public static void LoadUrl(string url)
    {
        // Init
        if (m_webViewObject == null)
        {
            m_webViewObject = (new GameObject("WebViewObject")).AddComponent<WebViewObject>();
            m_webViewObject.Init(
                cb: unityCallback, // JS -> "Unity.call('msg'); return false;" / <a href="unity:msg">
                ld: OnLoadedCallback, // Loaded anything including error page.
                err: OnErrorCallback, // On Error only
                transparent: true,
                //ua: "custom user agent string",
                enableWKWebView: true // iOS
            );
        }

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        m_webViewObject.bitmapRefreshCycle = 1;
#endif
        m_webViewObject.SetMargins(m_margin.left, m_margin.top, m_margin.right, m_margin.bottom);
        m_webViewObject.SetVisibility(true);

        // Load page
#if !UNITY_WEBPLAYER
        // Remote
        if (url.StartsWith("http"))
        {
            m_webViewObject.LoadURL(url.Replace(" ", "%20"));
        }
        // Local
        else
        {
            var dst = System.IO.Path.Combine(Application.persistentDataPath, url);
            m_webViewObject.LoadURL("file://" + dst.Replace(" ", "%20"));
        }
#endif
    }

    /// <summary>
    /// Load and show <paramref name="url"/> page in <paramref name="uiRectTransform"/> area.
    /// </summary>
    /// <param name="url">URL</param>
    /// <param name="uiRectTransform">Area to show page.</param>
    public static void LoadUrlOnUI(string url, RectTransform uiRectTransform)
    {
        RectOffset uiBound = uiRectTransform.ToScreenSpace();
        m_margin = ScreenPointsToMargins(uiBound);
        LoadUrl(url);
    }

    /// <summary>
    /// Destroy WebView object.
    /// </summary>
    public static void Destroy()
    {
        if (m_webViewObject != null)
        {
            // Return the z-order value of SurfaceView in your phone.
            m_webViewObject.SetVisibility(false);

            // Destroy object
            Object.Destroy(m_webViewObject.gameObject);

            // Set reference to null
            m_webViewObject = null;
        }
    }

    /// <summary>
    /// Show hidden WebView object.
    /// </summary>
    public static void Show()
    {
        if (m_webViewObject != null)
        { m_webViewObject.SetVisibility(true); }
    }

    /// <summary>
    /// Hide WebView object.
    /// </summary>
    public static void Hide()
    {
        if (m_webViewObject != null)
        { m_webViewObject.SetVisibility(false); }
    }

    /// <summary>
    /// Back = history.go(-1) / history.back()
    /// </summary>
    public static void GoBack()
    {
        if (canGoBack)
        { m_webViewObject.GoBack(); }
    }

    /// <summary>
    /// Forward = history.go(1)
    /// </summary>
    public static void GoForward()
    {
        if (canGoForward)
        { m_webViewObject.GoForward(); }
    }
    #endregion

    #region Setting
    public static void SetMargins(RectOffset margin)
    {
        m_margin = margin;
        if (m_webViewObject != null)
        { m_webViewObject.SetMargins(margin.left, margin.top, margin.right, margin.bottom); }
    }

    public static void SetMargins(int left, int top, int right, int bottom)
    {
        m_margin = new RectOffset(left, right, top, bottom);
        if (m_webViewObject != null)
        { m_webViewObject.SetMargins(left, top, right, bottom); }
    }

    public static void RestoreDefaultUnityCallback()
    {
        unityCallback = DefaultUnityCallback;
    }
    #endregion

    #region Utility
    public static RectOffset ScreenPointsToMargins(RectOffset screen)
    {
        return ScreenPointsToMargins(screen.left, screen.top, screen.right, screen.bottom);
    }

    public static RectOffset ScreenPointsToMargins(Rect screen)
    {
        return ScreenPointsToMargins((int)screen.xMin, (int)screen.yMax, (int)screen.xMax, (int)screen.yMin);
    }

    public static RectOffset ScreenPointsToMargins(int left, int top, int right, int bottom)
    {
        return new RectOffset(left, Screen.width - right, Screen.height - top, bottom);
    }

    /// <summary>
    /// Write data in "StreamingAssets" folder to Application.persistentDataPath.
    /// *By default, this function MUST be called by StartCoroutine().
    /// </summary>
    public static IEnumerator WritePersistentDataFromStreamingAsset(string localUrl)
    {
        string src = System.IO.Path.Combine(Application.streamingAssetsPath, localUrl);
        string dst = System.IO.Path.Combine(Application.persistentDataPath, localUrl);
        byte[] result = null;
        // For Android
        if (src.Contains("://"))
        {
            var www = new WWW(src);
            yield return www;
            result = www.bytes;
        }
        else
        {
            result = System.IO.File.ReadAllBytes(src);
        }
        System.IO.File.WriteAllBytes(dst, result);
        yield break;
    }

    /// <summary>
    /// Write data in "StreamingAssets" folder to Application.persistentDataPath.
    /// *By default, this function MUST be called by StartCoroutine().
    /// </summary>
    public static IEnumerator WritePersistentDataFromStreamingAsset(string[] localUrls)
    {
        foreach (string url in localUrls)
        {
            string src = System.IO.Path.Combine(Application.streamingAssetsPath, url);
            string dst = System.IO.Path.Combine(Application.persistentDataPath, url);
            byte[] result = null;
            // For Android
            if (src.Contains("://"))
            {
                var www = new WWW(src);
                yield return www;
                result = www.bytes;
            }
            else
            {
                result = System.IO.File.ReadAllBytes(src);
            }
            System.IO.File.WriteAllBytes(dst, result);
        }
        yield break;
    }
    #endregion

    //------------------------------ Private Methods ------------------------------//
    #region (Default) Callback
    private static void OnLoadedCallback(string msg)
    {
        // Init JS function "window.Unity.call"
#if !UNITY_ANDROID
        // Overwrite js function to prevent NS error on iOS.

        // NOTE: depending on the situation, you might prefer
        // the 'iframe' approach.
        // cf. https://github.com/gree/unity-webview/issues/189
#if true
        m_webViewObject.EvaluateJS(@"
              window.Unity = {
                call: function(msg) {
                  window.location = 'unity:' + msg;
                }
              }
            ");
#else
        m_webViewObject.EvaluateJS(@"
              window.Unity = {
                call: function(msg) {
                  var iframe = document.createElement('IFRAME');
                  iframe.setAttribute('src', 'unity:' + msg);
                  document.documentElement.appendChild(iframe);
                  iframe.parentNode.removeChild(iframe);
                  iframe = null;
                }
              }
            ");
#endif
#endif
        // Show UA
        //m_webViewObject.EvaluateJS(@"Unity.call('ua=' + navigator.userAgent)");
    }

    private static void OnErrorCallback(string msg)
    {
        Debug.Log("OnError: " + msg);
    }

    private static void DefaultUnityCallback(string msg)
    {
        if (msg.StartsWith("url="))
        {
            Application.OpenURL(msg.Substring(4).Replace(" ", "%20"));
        }
    }
    #endregion
}
