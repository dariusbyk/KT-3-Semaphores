using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static SemaphoreSlim semaphore;

    static async Task Main(string[] args)
    {
        Console.Write("Введите количество ресурсов: ");
        int resourceCount = int.Parse(Console.ReadLine());

        semaphore = new SemaphoreSlim(resourceCount, resourceCount);
        Task[] tasks = new Task[6];

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < tasks.Length; i++)
        {
            int taskNumber = i + 1;
            tasks[i] = AccessResourceAsync(taskNumber);
        }

        Task allTasks = Task.WhenAll(tasks);

        while (!allTasks.IsCompleted)
        {
            Console.WriteLine("Основная программа продолжает работу.");
            await Task.Delay(500);
        }

        await allTasks;
        stopwatch.Stop();

        semaphore.Dispose();
        Console.WriteLine($"Все задачи завершены за {stopwatch.ElapsedMilliseconds} мс.");
    }

    static async Task AccessResourceAsync(int taskNumber)
    {
        Console.WriteLine($"Задача {taskNumber} ожидает ресурс.");
        await semaphore.WaitAsync();

        try
        {
            Console.WriteLine($"Задача {taskNumber} получила ресурс.");
            await Task.Delay(1000);
        }
        finally
        {
            Console.WriteLine($"Задача {taskNumber} освободила ресурс.");
            semaphore.Release();
        }
    }
}
