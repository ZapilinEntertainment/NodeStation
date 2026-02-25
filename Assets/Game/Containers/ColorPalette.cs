using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace ZE.NodeStation
{
    [CreateAssetMenu(fileName = nameof(ColorPalette), menuName = Constants.ScriptableObjectsFolderPath + nameof(ColorPalette))]

    // NOTE: we also can use binding by key, instead of multiple interfaces (VContainer feature)
    public class ColorPalette : ScriptableObject, IGUIColorsPalette, ILightColorsPalette
    {
        [SerializeField] private SerializedDictionary<ColorKey, Color> _colors;
    
        public Color GetColor(ColorKey key) => _colors.TryGetValue(key, out var color) ? color : Color.black;

        public Color GetGUIColor(ColorKey key) => GetColor(key);

        public Color GetLightColor(ColorKey key) => GetColor(key);
    }

    public interface IGUIColorsPalette
    {
        Color GetGUIColor(ColorKey key);
    }

    public interface ILightColorsPalette
    {
        Color GetLightColor(ColorKey key);
    }
}
