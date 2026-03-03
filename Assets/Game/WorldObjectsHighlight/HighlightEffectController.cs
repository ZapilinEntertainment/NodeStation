using System;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using UniRx;

namespace ZE.NodeStation
{
    public class HighlightEffectController : IStartable 
    {
        private readonly ICameraController _cameraController;
        private bool _isEffectActive = false;

        [Inject]
        public HighlightEffectController(ICameraController cameraController)
        {
            _cameraController = cameraController;
        }

        public void Start()
        {
            // TODO: temp solution, add flags
            // todo: material change via shader (for both opaque & transparent)
            Observable.EveryUpdate().Where(_ => Input.GetKeyDown(KeyCode.M))
                .Subscribe(_ => 
                { 
                    _isEffectActive = !_isEffectActive;
                    _cameraController.SwitchRenderMode(_isEffectActive ? CameraRenderMode.Grayscale : CameraRenderMode.Default);
                });
        }
    }
}
