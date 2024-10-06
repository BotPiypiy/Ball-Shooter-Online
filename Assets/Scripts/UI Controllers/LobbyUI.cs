using Mirror;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

public class LobbyUI : MonoBehaviour
{
    [SerializeField]
    private LobbyNetworkManager _networkManager;

    [Header("Lobby Panel")]
    [SerializeField]
    private GameObject _lobbyPanel;
    [Space]
    [SerializeField]
    private TMP_InputField _ip;
    [SerializeField]
    private TMP_InputField _port;

    [Space]
    [SerializeField]
    private GameObject _disconnectButton;

    [Header("Color Pick Panel")]
    [SerializeField]
    private GameObject _colorPickPanel;
    [Space]
    [SerializeField]
    private GameObject _readyButton;

    [Space]
    [SerializeField]
    private Toggle _redToggle;
    [SerializeField]
    private Toggle _greenToggle;
    [SerializeField]
    private Toggle _blueToggle;
    [SerializeField]
    private Toggle _yellowToggle;
    [SerializeField]
    private GameObject _notifyText;


    private Toggle _pickedToggle;
    private char _pickedColor = ' ';


    private bool _isAwaitingResponse;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _disconnectButton.SetActive(!_disconnectButton.activeSelf);
        }
    }

    public void OnCreateHostClick()
    {
        try
        {
            if (!NetworkClient.active)
            {
                this.UpdateNetData();
                _networkManager.StartHost();
            }

            NetworkClient.OnConnectedEvent += OnConnected;
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void OnConnectClick()
    {
        try
        {
            if (!NetworkClient.active)
            {
                this.UpdateNetData();
                _networkManager.StartClient();
            }

            NetworkClient.OnConnectedEvent += OnConnected;

        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void OnDisconnectClick()
    {
        try
        {
            if (NetworkClient.active)
            {
                NetworkClient.OnConnectedEvent -= OnConnected;

                if (NetworkServer.active)
                {
                    _networkManager.StopHost();
                }
                else
                {
                    _networkManager.StopClient();
                }

                _lobbyPanel.SetActive(true);
                _colorPickPanel.SetActive(false);
                _disconnectButton.SetActive(false);
            }
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void OnReadyButtonClick()
    {
        if (!_isAwaitingResponse)
        {
            if (NetworkClient.isConnected)
            {
                OnColorToggleClick();
                _notifyText.SetActive(false);
                _isAwaitingResponse = true;
                NetworkClient.connection.Send(new ReadyRequest(_pickedColor));
            }
        }
    }

    private void UpdateNetData()
    {
        _networkManager.networkAddress = _ip.text;

        if (Transport.active is PortTransport portTransport)
        {
            if (ushort.TryParse(_port.ToString(), out ushort port))
            {
                portTransport.Port = port;
            }
        }
    }

    private void OnConnected()
    {
        _lobbyPanel.SetActive(false);
        _colorPickPanel.SetActive(true);
        _disconnectButton.SetActive(true);
        NetworkClient.RegisterHandler<ReadyResponse>(OnReadyResponse, false);
    }

    private void OnReadyResponse(ReadyResponse readyResponse)
    {
        if (_isAwaitingResponse)
        {
            if (readyResponse.Accepted)
            {
                _colorPickPanel.SetActive(false);
                _disconnectButton.SetActive(false);
                NetworkClient.AddPlayer();
            }
            else
            {
                _pickedColor = ' ';
                _notifyText.SetActive(true);
            }
        }

        _isAwaitingResponse = false;
    }

    private void OnColorToggleClick()
    {
        if (_redToggle.isOn)
        {
            _pickedToggle = _redToggle;
            _pickedColor = 'r';
        }
        else if (_greenToggle.isOn)
        {
            _pickedToggle = _greenToggle;
            _pickedColor = 'g';
        }
        else if (_blueToggle.isOn)
        {
            _pickedToggle = _blueToggle;
            _pickedColor = 'b';
        }
        else if (_yellowToggle.isOn)
        {
            _pickedToggle = _yellowToggle;
            _pickedColor = 'y';
        }
    }
}
