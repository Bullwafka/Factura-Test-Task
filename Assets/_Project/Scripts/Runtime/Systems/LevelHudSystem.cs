using System;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class LevelHudSystem : IStartable, IDisposable
    {
        private readonly LevelState _levelState;
        private readonly HudView _hud;

        public LevelHudSystem(LevelState levelState, GameSceneContext scene)
        {
            _levelState = levelState;
            _hud = scene.Hud;
        }

        public void Start()
        {
            _levelState.ProgressChanged += OnProgressChanged;
            OnProgressChanged(_levelState.Progress);
        }

        public void Dispose()
        {
            _levelState.ProgressChanged -= OnProgressChanged;
        }

        private void OnProgressChanged(float progress)
        {
            var fillAnchors = _hud.LevelProgressFill.rectTransform.anchorMax;
            fillAnchors.y = progress;
            _hud.LevelProgressFill.rectTransform.anchorMax = fillAnchors;
        }
    }
}
