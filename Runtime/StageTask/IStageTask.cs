using System.Collections;

namespace GameFramework
{
    public interface IStageTask
    {
        float Progress { get; }

        string TaskInfo { get; }

        IEnumerator Run();
    }
}