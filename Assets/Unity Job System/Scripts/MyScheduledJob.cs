
using System;
using System.Diagnostics;
using System.Globalization;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Debug = UnityEngine.Debug;

public class MyScheduledJob : MonoBehaviour
{
    public NativeArray<float> result;
    private Stopwatch _stopwatch = new();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            result = new NativeArray<float>(2, Allocator.TempJob);

            MyJob job1 = new MyJob
            {
                a = 5,
                b = 10,
                result = result
            };

            JobHandle handle1 = job1.Schedule();

            _stopwatch.Restart();
            handle1.Complete();
            _stopwatch.Stop();

            Debug.Log("Elapsed time for job1: " + _stopwatch.Elapsed.TotalMilliseconds + " ms");

            MySecondJob job2 = new MySecondJob
            {
                a = 1,
                b = 2,
                result = result
            };

            JobHandle handle2 = job2.Schedule(handle1);

            _stopwatch.Restart();
            handle2.Complete();
            _stopwatch.Stop();

            Debug.Log("Elapsed time for job2: " + _stopwatch.Elapsed.TotalMilliseconds + " ms");

            result.Dispose();
        }
    }
}

public struct MyJob : IJob
{
    public float a;
    public float b;
    public NativeArray<float> result;

    public void Execute() => result[0] = a + b;
}

public struct MySecondJob : IJob
{
    public float a;
    public float b;
    public NativeArray<float> result;

    public void Execute() => result[1] = a - b;
}


