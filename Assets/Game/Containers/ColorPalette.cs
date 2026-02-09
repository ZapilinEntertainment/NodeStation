using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace ZE.NodeStation
{
    [CreateAssetMenu(fileName = nameof(ColorPalette), menuName = Constants.ScriptableObjectsFolderPath + nameof(ColorPalette))]
    public class ColorPalette : ScriptableObject
    {
        [SerializeField] private SerializedDictionary<ColorKey, Color> _colors;
    
        public Color GetColor(ColorKey key) => _colors.TryGetValue(key, out var color) ? color : Color.black;
    }
}
