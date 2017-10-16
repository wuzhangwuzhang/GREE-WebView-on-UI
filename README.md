
# GREE WebView on UI

Showing [GREE's Unity WebView] in specific UI area.

\*It's look-like WebView is showing on UI but NOT!!

Tested on Unity 5.5.1f1 & Unity 2017.1.0f3

## Requirement

- [GREE's Unity WebView]

## Install

1. Install [GREE's Unity WebView] package.
2. Copy everything in this repository `Assets` folder to your project `Assets` folder.

\*Files in this repository alone can't run or build the project.

## Usage

**Load local URL**

Writing _EVERY_ file from `StreamingAssets` folder to `Application.persistentDataPath` is _REQUIRED_.

So, do something like this in initial state.

```csharp
foreach (string file in filePaths)
{
  StartCoroutine(WebView.WritePersistentDataFromStreamingAsset(file));
}
```

**Load remote URL**

Just pass `URL` to parameter is fine.

```csharp
// Using default margin
WebView.LoadUrl(@"http://www.abc.def");
```

```csharp
RectTransform myUiRectTransform; // Object Reference
void DoSomething()
{
  // Show within UI area
  WebView.LoadUrlOnUI(@"http://www.abc.def", myUiRectTransform);
}
```

> Well, open scene `Sample.unity` and see maybe the fastest way.

## License

MIT License

[GREE's Unity WebView]: https://github.com/gree/unity-webview
