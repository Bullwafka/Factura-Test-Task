using System;
using UnityEngine;

namespace Factura.Gameplay
{
    public sealed class LevelState
    {
        public event Action<float> ProgressChanged;
        public event Action Completed;

        public float Progress { get; private set; }
        public bool IsCompleted { get; private set; }

        public void SetProgress(float progress)
        {
            var clampedProgress = Mathf.Clamp01(progress);
            if (Mathf.Approximately(Progress, clampedProgress))
                return;

            Progress = clampedProgress;
            ProgressChanged?.Invoke(Progress);
        }

        public void Complete()
        {
            if (IsCompleted)
                return;

            IsCompleted = true;
            SetProgress(1f);
            Completed?.Invoke();
        }
    }
}
