using UnityEngine;
using System.IO;
using System.Collections;

public class AdvancedHighResScreenshot : MonoBehaviour
{
    public int width = 3840; // Width of the screenshot
    public int height = 2160; // Height of the screenshot
    public string screenshotFileName = "HighResScreenshot";
    public Camera screenshotCamera;

  /*  void Start()
    {
        // Optionally, find a specific camera or use the main camera
        screenshotCamera = Camera.main;
    }*/

    void Update()
    {
        // Trigger the screenshot capture
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(CaptureScreenshot());
        }
    }

    IEnumerator CaptureScreenshot()
    {
        // Wait for the right moment in the frame to ensure all rendering is done
        yield return new WaitForEndOfFrame();

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        screenshotCamera.targetTexture = renderTexture;
        screenshotCamera.Render();

        // Create a new Texture2D and read the RenderTexture image into it
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        RenderTexture.active = renderTexture;
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();

        // Clean up
        screenshotCamera.targetTexture = null;
        RenderTexture.active = null;
        Destroy(renderTexture);

        // Encode the texture into PNG format
        byte[] bytes = screenshot.EncodeToPNG();
        // Save the encoded image to a file
        string filename = string.Format("{0}_{1}x{2}_{3}.png", screenshotFileName, width, height, System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        File.WriteAllBytes(Path.Combine(Application.persistentDataPath, filename), bytes);

        Debug.Log($"Screenshot saved: {Path.Combine(Application.persistentDataPath, filename)}");
    }
}
