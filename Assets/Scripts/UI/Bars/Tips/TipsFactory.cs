using UnityEngine;

namespace UI.Bars.Tips
{
    public class TipsFactory
    {
        private PoolMono<TipLabel> _poolMono;

        public TipsFactory(TipLabel labelPrefab, RectTransform root)
        {
            labelPrefab.gameObject.SetActive(false);
    
            _poolMono = new PoolMono<TipLabel>(labelPrefab, root, true)
                .AddCreateCallback(OnTipLabelCreated)
                .Expand(2);
        }

        public TipLabel Take()
        {
            return _poolMono.Take();
        }

        public void Put(TipLabel label)
        {
            _poolMono.Put(label);
        }

        private void OnTipLabelCreated(TipLabel tipLabel) => 
            tipLabel.Hide();
    }
}