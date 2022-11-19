using System.Collections.Concurrent;

namespace synchronization_context_app;

public class SimpleSynchronizationContext : SynchronizationContext
{
    private readonly ConcurrentQueue<MyTask> _queue;
    private readonly Thread _processingThread;

    public SimpleSynchronizationContext()
    {
        _queue = new ConcurrentQueue<MyTask>();
        _processingThread = new Thread(Handle);
        _processingThread.Start();
    }

    private void Handle()
    {
        Console.WriteLine($"Start processing tasks on thread: {Environment.CurrentManagedThreadId}");

        var spinWait = new SpinWait();
        while (true)
        {
            if (_queue.TryDequeue(out var myTask))
            {
                myTask.Callback(myTask.State);
            }
            else
            {
                spinWait.SpinOnce();
            }
        }
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        throw new NotImplementedException();
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        var myTask = new MyTask(d, state);
        _queue.Enqueue(myTask);
    }
}