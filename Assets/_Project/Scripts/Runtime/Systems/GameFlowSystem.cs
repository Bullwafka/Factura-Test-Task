using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer.Unity;

namespace Factura.Gameplay
{
    public sealed class GameFlowSystem : IStartable, IDisposable
    {
        private readonly IPlayerInputService _input;
        private readonly LevelState _levelState;
        private readonly Health _vehicleHealth;
        private readonly HudView _hud;

        private readonly CancellationTokenSource _cancellationTokenSource;
       
        public GameFlowSystem(
            IPlayerInputService input,
            LevelState levelState,
            VehicleEntity vehicle,
            GameSceneContext scene)
        {
            _input = input;
            _levelState = levelState;
            _vehicleHealth = vehicle.Health;
            _hud = scene.Hud;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public void Start()
        {
            Time.timeScale = 0f;
            _hud.StartPrompt.SetActive(true);
            _hud.ResultOverlay.SetActive(false);

            _levelState.Completed += OnLevelCompleted;
            _vehicleHealth.Died += OnVehicleDestroyed;

            WaitForInputFired(() =>
            {
                _hud.StartPrompt.SetActive(false);
                Time.timeScale = 1f;
            }, _cancellationTokenSource.Token).Forget();
        }

        public void Dispose()
        {
            _levelState.Completed -= OnLevelCompleted;
            _vehicleHealth.Died -= OnVehicleDestroyed;
            Time.timeScale = 1f;
            _cancellationTokenSource.Cancel();
        }

        private void OnLevelCompleted()
        {
            Finish("YOU WIN");
        }

        private void OnVehicleDestroyed()
        {
            Finish("YOU LOSE");
        }

        private void Finish(string title)
        {
            _hud.StartPrompt.SetActive(false);
            _hud.ResultTitle.text = title;
            _hud.ResultHint.text = "TAP TO RESTART";
            _hud.ResultOverlay.SetActive(true);
            Time.timeScale = 0f;

            WaitForInputFired(() =>
               SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex),
               _cancellationTokenSource.Token)
               .Forget();
        }

        private async UniTaskVoid WaitForInputFired(Action callback, CancellationToken cancellation = default)
        {
            await UniTask.WaitUntil(() => _input.IsInputFired);

            if (cancellation.IsCancellationRequested)
                return;

            callback?.Invoke();
        }
    }
}
