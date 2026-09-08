using UnityEngine;
using UnityEngine.UI;

namespace Factura.Gameplay
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private Image _vehicleHealthFill;
        [SerializeField] private Text _vehicleHealthText;
        [SerializeField] private Text _killCountText;
        [SerializeField] private Image _levelProgressFill;
        [SerializeField] private GameObject _startPrompt;
        [SerializeField] private GameObject _resultOverlay;
        [SerializeField] private Text _resultTitle;
        [SerializeField] private Text _resultHint;

        public Image VehicleHealthFill => _vehicleHealthFill;
        public Text VehicleHealthText => _vehicleHealthText;
        public Text KillCountText => _killCountText;
        public Image LevelProgressFill => _levelProgressFill;
        public GameObject StartPrompt => _startPrompt;
        public GameObject ResultOverlay => _resultOverlay;
        public Text ResultTitle => _resultTitle;
        public Text ResultHint => _resultHint;
    }
}
