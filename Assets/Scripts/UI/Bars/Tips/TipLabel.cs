using TMPro;
using UnityEngine;

namespace UI.Bars.Tips
{
    public class TipLabel : MonoBehaviour
    {
        public RectTransform RectTransform => _rectTransform;

        [SerializeField]
        private RectTransform _rectTransform;

        [SerializeField]
        private TextMeshProUGUI _label;

        public void Show(string message)
        {
            _label.text = message;

            _label.enabled = true;
        }

        public void Hide() => 
            _label.enabled = false;
    }
}