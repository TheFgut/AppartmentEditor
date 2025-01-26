using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SystemsInstaller : MonoInstaller
{
    [SerializeField] private WallSelectorController wallSelector;
    [SerializeField] private TexturesPalette awailableTexturesPalette;
    [SerializeField] private GlobalStateController globalStateController;
    [SerializeField] private ModalInfoWindow modalInfoWindow;

    public override void InstallBindings()
    {
        Container.BindInstance(wallSelector);
        Container.BindInstance(awailableTexturesPalette);
        Container.BindInstance(globalStateController);
        Container.BindInstance(modalInfoWindow);
    }
}
