using NetCore;
using UnityEngine;

public class RuntimeInitialization
{
    private static IOContext ioContext;

    private static IOContext.WorkGuard workGuard;

    public static TCP tcp;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnAfterSceneLoaded()
    {
        InitNetworkSystem();
    }

    private static void InitNetworkSystem()
    {
        ioContext = new IOContext();
        {
            workGuard = ioContext.MakeWorkGuard();
            ioContext.Run();
        }

        // tcp = new TCP(ioContext);
    }
}
