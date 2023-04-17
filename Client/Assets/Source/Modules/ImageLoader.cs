using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ImageLoader
{
    private const float BASE_VERTICAL_SCALE = 6.5f;
    private const string BASE_URL = "http://codemoshete.com/3dImageViewer";
    private const string MANIFEST_NAME = "manifest.json";
    private ImageDirectory manifest;
    private int currentImageIndex;
    private ImageEntry currentImage;

    private UnityWebRequest manifestRequest;
    private UnityWebRequest leftImageRequest;
    private UnityWebRequest rightImageRequest;

    private Material leftMaterial;
    private Texture leftImage;
    private Material rightMaterial;
    private Texture rightImage;
    private Transform leftScreen;
    private Transform rightScreen;

    private bool leftImageReady;
    private bool rightImageReady;

    private DownloadManager downloadManager;

    public ImageLoader(
        Material leftMat,
        Material rightMat,
        Transform leftScreen,
        Transform rightScreen,
        DownloadManager downloadManager,
        TextAsset localManifest = null)
    {
        leftMaterial = leftMat;
        rightMaterial = rightMat;
        this.leftScreen = leftScreen;
        this.rightScreen = rightScreen;

        this.downloadManager = downloadManager;

        if (localManifest != null)
        {
            manifest = JsonUtility.FromJson<ImageDirectory>(localManifest.text);
            EnableEventListenersAndShowFirstImage();
        }
        else
        {
            string manifestUrl = string.Format("{0}/{1}", BASE_URL, MANIFEST_NAME);
            manifestRequest = new UnityWebRequest(manifestUrl);
            downloadManager.GetManifest(OnRemoteManifestRetrieved);
        }
    }

    private void OnRemoteManifestRetrieved(string manifestText)
    {
        manifest = JsonUtility.FromJson<ImageDirectory>(manifestText);
        EnableEventListenersAndShowFirstImage();
    }

    private void EnableEventListenersAndShowFirstImage()
    {
        Service.UpdateManager.AddObserver(OnUpdate);
        Service.EventManager.AddListener(EventId.FastForward, LoadNextImage);
        Service.EventManager.AddListener(EventId.Rewind, LoadPrevImage);

        if (manifest != null && manifest.Images != null && manifest.Images.Length > 0)
        {
            LoadImage(manifest.Images[0]);
        }
    }

    private void LoadImage(ImageEntry image)
    {
        ResetImages();
        currentImage = image;

        switch (image.Source)
        {
            case ImageSource.Resources:
                OnLeftImageLoaded(Resources.Load<Texture>(image.LeftImagePath));
                OnRightImageLoaded(Resources.Load<Texture>(image.RightImagePath));
                break;
            case ImageSource.StreamingAssets:
            case ImageSource.Web:
                downloadManager.GetTexture(image.LeftImagePath, OnLeftImageLoaded);
                downloadManager.GetTexture(image.RightImagePath, OnRightImageLoaded);
                break;
        }
    }

    public bool LoadNextImage(object cookie)
    {
        currentImageIndex = currentImageIndex == manifest.Images.Length - 1 ? 0 : currentImageIndex + 1;
        LoadImage(manifest.Images[currentImageIndex]);
        return true;
    }

    public bool LoadPrevImage(object cookie)
    {
        currentImageIndex = currentImageIndex == 0 ? manifest.Images.Length - 1 : currentImageIndex - 1;
        LoadImage(manifest.Images[currentImageIndex]);
        return true;
    }

    private void OnLeftImageLoaded(object leftImageObj)
    {
        leftImage = (Texture)leftImageObj;
        leftMaterial.mainTexture = leftImage;
        leftMaterial.SetTexture("_EmissionMap", leftImage);

        float aspectRatio = (float)leftImage.width / (float)leftImage.height;
        Vector3 newScreenScale = new Vector3(aspectRatio * BASE_VERTICAL_SCALE, BASE_VERTICAL_SCALE, 1);
        leftScreen.localScale = newScreenScale;
        leftImageReady = true;
        CheckAllLoadingDone();
    }

    private void OnRightImageLoaded(object rightImageObj)
    {
        rightImage = (Texture)rightImageObj;
        rightMaterial.mainTexture = rightImage;
        rightMaterial.SetTexture("_EmissionMap", rightImage);

        float aspectRatio = (float)rightImage.width / (float)rightImage.height;
        Vector3 newScreenScale = new Vector3(aspectRatio * BASE_VERTICAL_SCALE, BASE_VERTICAL_SCALE, 1);
        rightScreen.localScale = newScreenScale;
        rightImageReady = true;
        CheckAllLoadingDone();
    }

    private void CheckAllLoadingDone()
    {
        if (leftImageReady && rightImageReady)
        {
            Service.EventManager.SendEvent(EventId.NewPhotoLoaded, null);
            Service.EventManager.SendEvent(EventId.SetScreenDistance, currentImage.Distance);
            Service.EventManager.SendEvent(EventId.SetScreenDivergence, currentImage.Divergence);
        }
    }

    private void ResetImages()
    {
        leftImageReady = false;
        rightImageReady = false;

        if (rightImage != null)
        {
            Object.DestroyImmediate(rightImage);
        }

        if (leftImage != null)
        {
            Object.DestroyImmediate(leftImage);
        }
    }

    private void OnUpdate(float dt)
    {
        
    }
}
