using System;
using System.Threading;
using System.Threading.Tasks;

class Program
{
    static PrioritySemaphore semaphore = new PrioritySemaphore(1);

    static async Task Main(string[] args)
    {
        // Первая задача занимает единственный ресурс.
        Task[] tasks = new Task[5];
        tasks[0] = AccessResourceAsync(1, false);

        // Эти задачи с низким приоритетом встают в очередь раньше.
        tasks[1] = AccessResourceAsync(2, false);
        tasks[2] = AccessResourceAsync(3, false);

        // Но при освобождении ресурса сначала проходят задачи с высоким приоритетом.
        tasks[3] = AccessResourceAsync(4, true);
        tasks[4] = AccessResourceAsync(5, true);

        await Task.WhenAll(tasks);
        semaphore.Dispose();

        Console.WriteLine("Все задачи завершены.");
    }

    static async Task AccessResourceAsync(int taskNumber, bool highPriority)
    {
        string priority;

        if (highPriority)
        {
            priority = "высокий";
        }
        else
        {
            priority = "низкий";
        }

        Console.WriteLine($"Задача {taskNumber} (приоритет: {priority}) ожидает ресурс.");
        await semaphore.WaitAsync(highPriority);

        try
        {
            Console.WriteLine($"Задача {taskNumber} (приоритет: {priority}) получила ресурс.");
            await Task.Delay(1000);
        }
        finally
        {
            Console.WriteLine($"Задача {taskNumber} освободила ресурс.");
            semaphore.Release();
        }
    }
}
