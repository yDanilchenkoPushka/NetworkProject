using TMPro;
using UnityEngine;
using Utilities;

namespace Test
{
    public class CheckUIPosition : MonoBehaviour
    {
        [SerializeField]
        private RectTransform _canvasRectTransform;
        
        [SerializeField]
        private Camera _camera;
        
        [SerializeField]
        private Transform _target;
        
        [SerializeField]
        private TextMeshProUGUI _label;

        [SerializeField]
        private float _offset;

        [ContextMenu("Calculate")]
        public void Calculate()
        {
            UIUtils.PlaceUIElement(_camera, _canvasRectTransform,
                _label.GetComponent<RectTransform>(), _target.position);
        }
    }
}