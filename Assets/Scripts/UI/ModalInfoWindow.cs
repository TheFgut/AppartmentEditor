using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ModalInfoWindow : MonoBehaviour
{
    [SerializeField] private Button okBut;
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI messageText;

    [Inject]
    protected void Construct()
    {
        try
        {
            Init();
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private void Init()
    {
        okBut.onClick.AddListener(Close);
    }

    public void Open(string title, string message)
    {
        this.gameObject.SetActive(true);
        titleText.color = Color.black;
        titleText.text = title;
        messageText.text = message;
    }

    public void OpenForError(string title, string message)
    {
        this.gameObject.SetActive(true);
        titleText.color = Color.red;
        titleText.text = title;
        messageText.text = message;
    }

    public void Close()
    {
        this.gameObject.SetActive(false);
    }
}
