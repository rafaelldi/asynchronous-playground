using System.Collections.Concurrent;
using System.Diagnostics;

namespace synchronization_context_app;

public class AffinitySynchronizationContext : SynchronizationContext
{
    private readonly int _processorId;
    private readonly ConcurrentQueue<MyTask> _queue;
    private readonly Thread _processingThread;
    
    public AffinitySynchronizationContext(int processorId)
    {
        _processorId = processorId;
        _queue = new ConcurrentQueue<MyTask>();
        _processingThread = new Thread(Handle)
        {
            IsBackground = true
        };
        _processingThread.Start();
    }

    private void Handle()
    {
        var threadId = Util.GetCurrentThreadId();
        var process = Process.GetCurrentProcess();
        var thread = process.Threads.OfType<ProcessThread>().Single(it => it.Id == threadId);
        thread.IdealProcessor = _processorId;
        thread.ProcessorAffinity = 1 << _processorId;
        
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