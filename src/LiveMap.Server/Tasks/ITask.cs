using CitizenFX.Core;

namespace LiveMap.Server.Tasks;

public interface ITask
{
    Coroutine Execute();
}