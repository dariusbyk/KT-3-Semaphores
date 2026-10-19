using System;
using System.Threading;
using System.Threading.Tasks;

class PrioritySemaphore : IDisposable
{
    private int freeResources;
    private int waitingHigh;
    private int waitingLow;

    private SemaphoreSlim highPriorityQueue;
    private SemaphoreSlim lowPriorityQueue;
    private object locker;

    public PrioritySemaphore(int resourceCount)
    {
        freeResources = resourceCount;
        waitingHigh = 0;
        waitingLow = 0;
        highPriorityQueue = new SemaphoreSlim(0);
        lowPriorityQueue = new SemaphoreSlim(0);
        locker = new object();
    }

    public async Task WaitAsync(bool highPriority)
    {
        bool needToWait = false;

        lock (locker)
        {
            if (freeResources > 0 && waitingHigh == 0 && waitingLow == 0)
            {
                freeResources--;
            }
            else
            {
                needToWait = true;

                if (highPriority)
                {
                    waitingHigh++;
                }
                else
                {
                    waitingLow++;
                }
            }
        }

        if (needToWait)
        {
            if (highPriority)
            {
                await highPriorityQueue.WaitAsync();
            }
            else
            {
                await lowPriorityQueue.WaitAsync();
            }
        }
    }

    public void Release()
    {
        lock (locker)
        {
            if (waitingHigh > 0)
            {
                waitingHigh--;
                highPriorityQueue.Release();
            }
            else if (waitingLow > 0)
            {
                waitingLow--;
                lowPriorityQueue.Release();
            }
            else
            {
                freeResources++;
            }
        }
    }

    public void Dispose()
    {
        highPriorityQueue.Dispose();
        lowPriorityQueue.Dispose();
    }
}
