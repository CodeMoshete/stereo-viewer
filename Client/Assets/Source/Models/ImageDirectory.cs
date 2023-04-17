using System;

[Serializable]
public class ImageDirectory
{
    public ImageEntry[] Images;
    public ImageDirectory[] SubDirectories;
}
