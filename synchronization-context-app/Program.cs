using synchronization_context_app;
using Xunit.Sdk;

//RunWithDefaultContext();
//RunWithXunitContext(5);
//RunWithSimpleContext();
RunWithAffinityContext();

static void RunWithDefaultContext()
{
    Console.WriteLine($"Current thread: {Environment.CurrentManagedThreadId}");

    var syncContext = new SynchronizationContext();
    syncContext.Send(_ => Console.WriteLine($"Thread from the Send method: {Environment.CurrentManagedThreadId}"), null);
    syncContext.Post(_ => Console.WriteLine($"Thread from the Pend method: {Environment.CurrentManagedThreadId}"), null);

    Console.ReadKey();
}

static void RunWithXunitContext(int maximumConcurrencyLevel)
{
    Console.WriteLine($"Current thread: {Environment.CurrentManagedThreadId}");

    var syncContext = new MaxConcurrencySyncContext(maximumConcurrencyLevel);
    for (var i = 0; i < 5; i++)
    {
        syncContext.Post(_ => Console.WriteLine($"Thread id: {Environment.CurrentManagedThreadId}"), null);
    }

    Console.ReadKey();
}

static void RunWithSimpleContext()
{
    Console.WriteLine($"Current thread: {Environment.CurrentManagedThreadId}");

    var syncContext = new SimpleSynchronizationContext();
    for (var i = 0; i < 5; i++)
    {
        syncContext.Post(_ => Console.WriteLine($"Thread id: {Environment.CurrentManagedThreadId}"), null);
    }

    Console.ReadKey();
}

static void RunWithAffinityContext()
{
    Console.WriteLine("Star");

    var syncContext = new AffinitySynchronizationContext(8);
    for (var i = 0; i < 500_000; i++)
    {
        syncContext.Post(
            _ =>
            {
                long t = 0;
                for (var i = 0; i < 20_000; i++)
                {
                    t += i / 42 - i * 3;
                }
            },
            null
        );
    }
    
    Console.WriteLine("Finished");
    Console.ReadKey();
}