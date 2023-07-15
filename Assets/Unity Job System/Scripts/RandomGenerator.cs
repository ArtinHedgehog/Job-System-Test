using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Debug = UnityEngine.Debug;
// We use Mathematics Random 
using Random = Unity.Mathematics.Random;

/// <summary>
/// This class is responsible for generating random numbers using a job system.
/// </summary>
class RandomGenerator : MonoBehaviour
{
    private float[] randomList;
    private bool useJob;
    Stopwatch stopwatch = new ();

   
    void Start() => randomList = new float[1000000];

    /// <summary>
    /// This function toggles the use of the job system.
    /// </summary>
    public void UseJobMethod()
    {
        if (useJob) useJob = false;
        else useJob = true;
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            stopwatch.Restart();
            
            if (useJob)
            {
                MultiThreadGenerate();
            }
            else
            {
                MainThreadGenerate();
            }
            stopwatch.Stop();
            Debug.Log("UseJob : "+ useJob +" === Elapsed time: " + stopwatch.Elapsed.TotalMilliseconds + " ms");
        }
    }

    /// <summary>
    /// This function generates random numbers on the main thread.
    /// </summary>
    void  MainThreadGenerate()
    {
        System.Random  rnd  =  new  System.Random();
        for  (int  i  =  0;  i  <  randomList.Length;  i++)  randomList[i]  =  (float)rnd.NextDouble();
    }

    /// <summary>
    /// This function generates random numbers using a job system.
    /// </summary>
    void MultiThreadGenerate()
    {
        NativeArray<float> randomListNative = new NativeArray<float>(randomList.Length, Allocator.TempJob);
        RandomJob randomJob = new RandomJob
        {
            randomList = randomListNative
        };
        JobHandle jobHandle = randomJob.Schedule(randomList.Length, 64);
        jobHandle.Complete();
        randomListNative.CopyTo(randomList);
        randomListNative.Dispose();
    }
}

/// <summary>
/// This struct represents a random number generation job that can be executed in parallel.
/// </summary>
[BurstCompile]
struct RandomJob : IJobParallelFor
{
    public NativeArray<float> randomList;

    /// <summary>
    /// This function is called for each element in the array.
    /// </summary>
    /// <param name="index">The index of the element.</param>
    public void Execute(int index)
    {
        Random random = new Random((uint)(index * 1000 + 1));
        randomList[index] = random.NextFloat();
    }
}
