using UnityEngine;

namespace ZE.NodeStation
{
    [CreateAssetMenu(fileName = nameof(HighlightMaterialsPack), menuName = Constants.ScriptableObjectsFolderPath + nameof(HighlightMaterialsPack))]
    public class HighlightMaterialsPack : ScriptableObject
    {
        [field: SerializeField] public Material OpaqueHighlightMaterial;
    }
}
