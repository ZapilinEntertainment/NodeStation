using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace ZE.NodeStation
{
    [CreateAssetMenu(fileName = nameof(WorldSpaceMarkerViewsPack), menuName = Constants.ScriptableObjectsFolderPath + nameof(WorldSpaceMarkerViewsPack))]
    public class WorldSpaceMarkerViewsPack : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<WorldSpaceMarkerViewKey, WorldSpaceMarkerUiView> _views;
        
        public bool TryGetPrefab(WorldSpaceMarkerViewKey key, out WorldSpaceMarkerUiView view) => _views.TryGetValue(key, out view);
    }

    public enum WorldSpaceMarkerViewKey : byte
    {
        Undefined,
        TrainDestination
    }
}
