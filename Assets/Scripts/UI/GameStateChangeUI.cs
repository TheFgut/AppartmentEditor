using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameStateChangeUI : MonoBehaviour
{
    [SerializeField] private List<StateChangerButton> stateChangerButtons = new List<StateChangerButton>();

    private StateChangerButton activeBut;

    private GlobalStateController globalStateController;
    [Inject]
    protected void Construct(GlobalStateController globalStateController)
    {
        this.globalStateController = globalStateController;
    }

    private void Start()
    {
        globalStateController.onStateChanges += GameStateChanged;

        //buttons init
        foreach (StateChangerButton button in stateChangerButtons)
        {
            button.Init(globalStateController);
        }
    }

    private void GameStateChanged(GlobalGameState newState)
    {
        if (activeBut != null) activeBut.Deactivate();
        StateChangerButton newActivatedButton = stateChangerButtons.FirstOrDefault(but =>
            but.switchState == newState);
        newActivatedButton.Activate();
        activeBut = newActivatedButton;
    }

    [Serializable]
    private class StateChangerButton
    {
        [SerializeField] private GlobalGameState _gameState;
        [SerializeField] private Button _button;
        [SerializeField] private float activatedSizeCoef = 1.1f;

        private RectTransform buttonTransform;

        private Vector3 defaultButtonScale;

        public GlobalGameState switchState => _gameState;
        public void Init(GlobalStateController globalStateController)
        {
            _button.onClick.AddListener(() => globalStateController.ChangeState(_gameState));
            buttonTransform = _button.transform as RectTransform;
            defaultButtonScale = buttonTransform.localScale;
        }

        public void Activate(bool permanent = false)
        {
            if (!permanent) buttonTransform.DOScale(defaultButtonScale * activatedSizeCoef, 0.5f);
            else buttonTransform.localScale = defaultButtonScale * activatedSizeCoef;
        }

        public void Deactivate(bool permanent = false)
        {
            if (!permanent) buttonTransform.DOScale(defaultButtonScale, 0.5f);
            else buttonTransform.localScale = defaultButtonScale;
        }
    }
}
