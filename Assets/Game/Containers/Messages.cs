using UnityEngine;

namespace ZE.NodeStation
{
    public class TrainAnnouncedMessage
    {
        public readonly TimetabledTrain Train;
        public TrainAnnouncedMessage(TimetabledTrain train) => Train = train;
    }
}
