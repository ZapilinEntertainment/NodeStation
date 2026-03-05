using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace ZE.NodeStation
{
    // apply grayscale + colouring effect when a route has been selected by player
    public class TrainRouteHighlightEffectController : IStartable, IDisposable 
    {
        private readonly ICameraController _cameraController;
        private readonly ISceneFlagsManager _flags;
        private readonly IMaterialColorsPalette _materialColors;
        private readonly HighlightMaterialsPack _highlightMaterialsPack;
        private readonly CompositeDisposable _compositeDisposable = new();

        private const string GLOBAL_HIGHLIGHT_COLOR = "_HighlightColorGlobal";

        [Inject]
        public TrainRouteHighlightEffectController(
            ICameraController cameraController, 
            ISceneFlagsManager sceneFlagsManager, 
            IMaterialColorsPalette materialColors,
            HighlightMaterialsPack highlightMaterialsPack)
        {
            _cameraController = cameraController;
            _flags = sceneFlagsManager;
            _materialColors = materialColors;
            _highlightMaterialsPack = highlightMaterialsPack;
        }

        public void Start()
        {
            _flags.Subscribe<TrainRouteHighlightFlag>(OnRouteHighlightChanged)
                .AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable.Dispose();
        }

        private void OnRouteHighlightChanged(bool isActive)
        {
            if (isActive)
            {
                _cameraController.SwitchRenderMode(CameraRenderMode.Grayscale);

                var activeHighlightFlag = _flags.GetFirstFlag<TrainRouteHighlightFlag>();
                var color = _materialColors.GetMaterialColor(activeHighlightFlag.Route.ColorKey);

                Shader.SetGlobalColor(GLOBAL_HIGHLIGHT_COLOR, color);
            }
            else
            {
                _cameraController.SwitchRenderMode(CameraRenderMode.Default);
            }
        }
    }
}
