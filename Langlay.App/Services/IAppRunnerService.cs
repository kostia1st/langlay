namespace Product
{
    public interface IAppRunnerService
    {
        bool IsExiting { get; }

        void ReReadAndRunTheConfig();

        void ExitApplication();
    }
}