using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShowColorFR : MonoBehaviour
{
    [SerializeField] RawImage background;

    WebCamTexture webCamTexture;
    void Start()
    {
        //NuitrackManager.onColorUpdate += DrawColor;
        string n = SelectExternalCamera();
        webCamTexture = new WebCamTexture(n);
        background.texture = webCamTexture;
        // Bắt đầu webcam
        webCamTexture.Play();
    }
    public void Stop()
    {
        webCamTexture?.Stop();
    }
    private void OnDisable()
    {
        Debug.Log("Camera Dis");
        webCamTexture?.Stop();
    }
    string SelectExternalCamera()
    {
        foreach (var cam in WebCamTexture.devices)
        {
            if (cam.name.Contains("RealSense") || cam.name.Contains("Orbbec") || cam.name.Contains("USB Camera"))
                return cam.name;
        }
        return WebCamTexture.devices.Length > 0 ? WebCamTexture.devices[0].name : null;
    }
}
