using System.Collections.Generic;
using UnityEngine;

namespace ZE.NodeStation
{
    public interface ITimetabledTrainsList
    {
        IReadOnlyList<TimetabledTrain> Trains { get; }
    
    }
}
