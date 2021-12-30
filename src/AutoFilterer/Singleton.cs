namespace AutoFilterer
{
    internal static class Singleton<T>
        where T : class, new()
    {
        private static T _instance;
        private static object lockingObj = new object();
        internal static T Instance
        {
            get
            {
                lock (lockingObj)
                {
                    if (_instance is null)
                    {
                        _instance = new T();
                    }
                }

                return _instance;
            }
        }
    }
}
