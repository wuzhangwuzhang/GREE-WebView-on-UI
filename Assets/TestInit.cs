using UnityEngine;

public class TestInit : MonoBehaviour
{
	private void Start () {
        StartCoroutine(WebView.WritePersistentDataFromStreamingAsset("sample.html"));
    }
}
