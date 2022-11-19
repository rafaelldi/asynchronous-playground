namespace synchronization_context_app;

public readonly record struct MyTask(SendOrPostCallback Callback, object? State);