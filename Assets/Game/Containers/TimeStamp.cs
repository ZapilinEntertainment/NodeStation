using System;
using UnityEngine;
using TriInspector;

namespace ZE.NodeStation
{
    // need to easy set time points at scriptable objects
    [Serializable]
    [DeclareHorizontalGroup("TimeGroup")]
    public struct TimeStamp
    {
        [Group("TimeGroup")] public byte Day;
        [Group("TimeGroup"), LabelText("Time: ")][Range(0, 23)] public byte Hour;
        [Group("TimeGroup"), HideLabel][Range(0,59)] public byte Minute;

        public TimeSpan ToTimeSpan() => new TimeSpan(days: Day, hours: Hour, minutes: Minute, seconds : 0);
    }
}
