using Unity.Jobs;
using UnityEngine;
using UnityEngine.Jobs;

/// <summary>
/// This class is responsible for moving objects using a job system.
/// </summary>
public class MoveObjectsJob : MonoBehaviour
{
    public GameObject[] objectsToMove;
    public TransformAccessArray transforms;
    public Vector3 direction;
    public float speed;

   
    private void Start()
    {
        // Create a new TransformAccessArray with the specified length
        transforms = new TransformAccessArray(objectsToMove.Length);

        // Add the transforms of the objects to the array
        for (int i = 0; i < objectsToMove.Length; i++)
        {
            transforms.Add(objectsToMove[i].transform);
        }
    }

    
    private void Update()
    {
        // Create a new MyMoveJob
        MyMoveJob job = new MyMoveJob
        {
            direction = direction,
            speed = speed,
            deltaTime = Time.deltaTime
        };

        // Schedule the job
        JobHandle handle = job.Schedule(transforms);

        // Wait for the job to complete
        handle.Complete();
    }

    /// <summary>
    /// This function is called when the script is disabled.
    /// </summary>
    private void OnDisable()
    {
        // Dispose of the TransformAccessArray
        transforms.Dispose();
    }
}

/// <summary>
/// This struct represents a move job that can be executed in parallel.
/// </summary>
public struct MyMoveJob : IJobParallelForTransform
{
    public Vector3 direction;
    public float speed;
    public float deltaTime;

    /// <summary>
    /// This function is called for each element in the array.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    /// <param name="transform">The transform of the element.</param>
    public void Execute(int index, TransformAccess transform)
    {
        // Calculate the move distance
        Vector3 moveDistance = direction * speed * deltaTime;

        // Update the position of the transform
        transform.position += moveDistance;
    }
}
