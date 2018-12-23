namespace Product {
    public interface ILifecycled {
        bool IsStarted { get; }
        void Start();
        void Stop();
    }
}
