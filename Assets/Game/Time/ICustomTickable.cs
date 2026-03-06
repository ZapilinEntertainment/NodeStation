using UnityEngine;

namespace ZE.NodeStation
{
    public interface ICustomTickable
    {
        void Tick();
    
    }

    public interface IFrameTickable : ICustomTickable { }

    public interface IFixedFrameTickable : ICustomTickable { }
    public interface ILateFrameTickable : ICustomTickable { }
}
