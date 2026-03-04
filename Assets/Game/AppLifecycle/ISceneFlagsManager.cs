using UnityEngine;
using ZE.Flags;

namespace ZE.NodeStation
{
    public interface ISceneFlagsManager : IFlagsManager { }
    public class SceneFlagsManager : FlagsManager, ISceneFlagsManager { }
}
