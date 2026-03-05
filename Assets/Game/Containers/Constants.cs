using UnityEngine;

namespace ZE.NodeStation
{
    public static class Constants
    {
        public const float MAX_INPUT_RAYCAST_LENGTH = 1000f;
        public const string ScriptableObjectsFolderPath = "Scriptable Objects/";

        public const int NO_EXIT_PATH_CODE = -1;
        public const int MAX_TRAIN_ROUTES = 8;
        
        public const float SEMAPHORE_IGNITE_DISTANCE = 250f;
        public const float SEMAPHORE_EXTINGUISH_DISTANCE = 10f;

        public const int NO_VIEW_ID = -1;

        public const int MAX_ROUTE_BUTTONS = 5;

        public static readonly int HIGHLIGHT_COLOURED_LAYER = LayerMask.NameToLayer("HighlightColoured");
        public static readonly int GRAYSCALE_IGNORE_LAYER = LayerMask.NameToLayer("IgnoreGrayscale");
    }
}
