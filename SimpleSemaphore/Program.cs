using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static SemaphoreSlim semaphore;

    static void Main(string[] args)
    {
        Console.Write("Введите количество ресурсов: ");
        int resourceCount = int.Parse(Console.ReadLine());

        semaphore = new SemaphoreSlim(resourceCount, resourceCount);

        Task[] tasks = new Task[6];

        for (int i = 0; i < tasks.Length; i++)
        {
            int taskNumber = i + 1;
            tasks[i] = Task.Run(() => AccessResource(taskNumber));
        }

        Task.WaitAll(tasks);
        semaphore.Dispose();

        Console.WriteLine("Все задачи завершены.");
    }

    static void AccessResource(int taskNumber)
    {
        Console.WriteLine($"Задача {taskNumber} ожидает ресурс.");
        semaphore.Wait();

        try
        {
            Console.WriteLine($"Задача {taskNumber} получила ресурс.");
            Thread.Sleep(1000);
        }
        finally
        {
            Console.WriteLine($"Задача {taskNumber} освободила ресурс.");
            semaphore.Release();
        }
    }
}
