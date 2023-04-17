using System;

public enum ImageSource
{
    Web,
    StreamingAssets,
    Resources
}

[Serializable]
public class ImageEntry
{
    public ImageSource Source;
    public string LeftImagePath;
    public string RightImagePath;
    public float Distance;
    public float Divergence;
    public float ImageWidth;
    public float ImageHeight;
}
