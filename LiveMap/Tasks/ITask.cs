using CitizenFX.Core;

namespace LiveMap.Tasks;

public interface ITask
{
    Coroutine Execute();
}