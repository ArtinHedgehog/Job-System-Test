using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

/// <summary>
/// This class is responsible for handling texture jobs.
/// </summary>
public class TextureJob : MonoBehaviour
{
    public Texture2D texture;
    private Color32[] originalTextureData;

    
    private void Start()
    {
        // Get the original texture data
        originalTextureData = texture.GetPixels32();
        
        // Get the raw texture data as a NativeArray
        NativeArray<Color32> textureData = texture.GetRawTextureData<Color32>();

        // Create a new MyTextureJob
        MyTextureJob job = new MyTextureJob();
        job.textureData = textureData;

        // Schedule the job
        JobHandle handle = job.Schedule(textureData.Length, 64);

        // Wait for the job to complete
        handle.Complete();

        // Apply the changes to the texture
        texture.Apply();
    }

    /// <summary>
    /// This function is called when the application quits.
    /// </summary>
    public void OnApplicationQuit()
    {
        // Restore the original texture data
        texture.SetPixels32(originalTextureData);
        texture.Apply();
    }
}

/// <summary>
/// This struct represents a texture job that can be executed in parallel.
/// </summary>
public struct MyTextureJob : IJobParallelFor
{
    public NativeArray<Color32> textureData;

    /// <summary>
    /// This function is called for each element in the array.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    public void Execute(int index)
    {
        // Get the color at the current index
        Color32 color = textureData[index];

        // Invert the color channels
        color.r = (byte)(255 - color.r);
        color.g = (byte)(255 - color.g);
        color.b = (byte)(255 - color.b);

        // Update the color in the array
        textureData[index] = color;
    }
}
