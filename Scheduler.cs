using FluentScheduler;
using WeirdTool.Services;

namespace WeirdTool
{
    public class Scheduler
    {
        public static void Initialize()
        {
            Registry timer = new();
            timer.Schedule<GloryListenerJob>()
                .ToRunNow()
                .AndEvery(1)
                .Days()
                .At(12, 0);

            JobManager.Initialize(timer);
        }
    }
}
