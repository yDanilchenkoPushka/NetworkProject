using System;
using System.Collections.Generic;
using Services.Input;
using UI.Buttons;
using UnityEngine;

namespace UI.Bars
{
    public class ButtonBar : MonoBehaviour
    {
        private const int DefaultButtonIndex = 0;
        
        [SerializeField]
        private Color _selectedColor;
        
        [SerializeField]
        private InteractiveButton[] _buttons;

        private Dictionary<Type, InteractiveButton> _buttonDictionary = new Dictionary<Type, InteractiveButton>();
        private int _selectedIndex = 0;

        private ISimpleInput _simpleInput;

        public void Construct(ISimpleInput simpleInput)
        {
            _simpleInput = simpleInput;
            
            InitializeButtons();
            
            _simpleInput.OnUpClicked += ToUp;
            _simpleInput.OnDownClicked += ToDown;

            _simpleInput.OnTaped += Click;
            
            _selectedIndex = DefaultButtonIndex;
            Select();
        }

        private void InitializeButtons()
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttonDictionary.Add(_buttons[i].GetType(), _buttons[i]);
                
                _buttons[i].Initialize(i);

                _buttons[i].OnOvered += Choose;
            }
        }

        public void DeInitialize()
        {
            _simpleInput.OnUpClicked -= ToUp;
            _simpleInput.OnDownClicked -= ToDown;

            _simpleInput.OnTaped -= Click;

            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].OnOvered -= Choose;
                
                _buttons[i].DeInitialize();
            }
        }

        public bool TryGetButton<TButton>(out InteractiveButton interactiveButton) where TButton : InteractiveButton
        {
            if (_buttonDictionary.TryGetValue(typeof(TButton), out InteractiveButton foundButton))
            {
                interactiveButton = foundButton;
                return true;
            }

            interactiveButton = default;
            return false;
        }

        private void ToUp()
        {
            CleanSelected();

            _selectedIndex--;
            
            if (_selectedIndex <= -1)
                _selectedIndex = _buttons.Length - 1;
            
            Select();
        }

        private void ToDown()
        {
            CleanSelected();
            
            _selectedIndex++;
            
            if (_selectedIndex >= _buttons.Length)
                _selectedIndex = 0;
            
            Select();
        }

        private void Select() => 
            _buttons[_selectedIndex].Select(_selectedColor);

        private void CleanSelected() => 
            _buttons[_selectedIndex].UnSelect();

        private void Click() => 
            _buttons[_selectedIndex].Click();

        private void Choose(int id)
        {
            CleanSelected();
            
            _selectedIndex = id;
            Select();
        }
    }
}