using Score;
using TMPro;
using UnityEngine;

namespace UI.Bars
{
    public class ScoreBar : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _scoreTitle;
        
        private IScoreReader _scoreReader;
        
        public void Construct(IScoreReader scoreReader)
        {
            _scoreReader = scoreReader;
            
            SetScore(0);

            _scoreReader.CurrentScore.OnValueChanged += UpdateScore;
        }

        public void DeInitialize() => 
            _scoreReader.CurrentScore.OnValueChanged -= UpdateScore;

        private void SetScore(int score) => 
            _scoreTitle.text = score.ToString();

        private void UpdateScore(int previousvalue, int newvalue) => 
            SetScore(newvalue);
    }
}