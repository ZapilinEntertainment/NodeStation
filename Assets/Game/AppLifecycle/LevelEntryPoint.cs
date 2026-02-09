using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ZE.NodeStation
{
    public class LevelEntryPoint : IStartable
    {
        private readonly AddSwitchablePointsReceiverCommand _addSwitchablePointsCommand;

        [Inject]
        public LevelEntryPoint(RouteChangeController controller, AddSwitchablePointsReceiverCommand addSwitchablePointsCommand) 
        {
            _addSwitchablePointsCommand = addSwitchablePointsCommand;
        }

        public void Start()
        {
            _addSwitchablePointsCommand.Execute();
        }
    }
}
