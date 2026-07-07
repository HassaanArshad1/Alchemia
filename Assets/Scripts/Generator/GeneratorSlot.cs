using UnityEngine;
using UnityEngine.UI;
using Alchemia.Data;

namespace Alchemia.Generator
{
    [RequireComponent(typeof(Button))]
    public class GeneratorSlot : MonoBehaviour
    {
        [SerializeField] private GeneratorData generator;
        [SerializeField] private GeneratorManager generatorManager;

        [Header("Optional visuals")]
        [SerializeField] private Image cooldownOverlay;
        [SerializeField] private Image icon;

        private Button _button;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnTapped);
        }

        private void OnDestroy()
        {
            if (_button != null) _button.onClick.RemoveListener(OnTapped);
        }

        private void Update()
        {
            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount =
                    generatorManager.CooldownFraction(generator.generatorId, generator.cooldownSeconds);
        }

        private void OnTapped()
        {
            generatorManager.TryGenerate(generator);
        }
    }
}