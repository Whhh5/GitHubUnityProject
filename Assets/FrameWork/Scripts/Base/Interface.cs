
namespace B1
{
    interface Log
    {
        void Log<T>(T message);
        void LogWarning<T>(T message);
        void LogError<T>(T message);
    }
}