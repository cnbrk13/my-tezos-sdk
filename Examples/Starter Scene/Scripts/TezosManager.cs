using System;
using System.Collections;
using UnityEngine;
using System.Text.Json;
using Beacon.Sdk.Beacon.Sign;
using BeaconSDK;
using Scripts.BeaconSDK;
using Scripts.Helpers;
using Scripts.Tezos;
using Scripts.Tezos.API;
using Scripts.Tezos.Wallet;
using Tezos.Contracts;
using Logger = Scripts.Helpers.Logger;

public class TezosManager : MonoBehaviour
{
    [Header("App Configurations")]
    public string appName = "Starter Sample";
    public string appDescription = "Tezos Starter Sample";
    public string appUrl = "https://tezos.com";
    public string[] appIcons = new string[] { "https://tezos.com/favicon.ico" };

    [Header("Storage Options")]
    [Tooltip("IPFS Gateway Override")]
    public string storageIpfsGatewayUrl = "https://infura-ipfs.io/ipfs/";
    
    public WalletMessageReceiver MessageReceiver { get; private set; }
    public ITezosDataAPI API { get; private set; }
    public IBeaconConnector BeaconConnector { get; private set; }
    //public IWalletProvider Wallet { get; private set; }
    
    public Contracts Contracts { get; private set; }

    private string _pubKey;
    private string _handshake = "";
    public string Handshake => _handshake;
    public string HandshakeURI => "tezos://?type=tzip10&data=" + _handshake;

    public static TezosManager Instance { get; private set; }
    
    private void Awake()
    {
        // Single persistent instance check
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Two TezosManager instances were found, removing this one.");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        // Create a BeaconMessageReceiver Game object to receive callback messages
        MessageReceiver = gameObject.AddComponent<WalletMessageReceiver>();
        var dataProviderConfig = new TzKTProviderConfig();
        API = new TezosDataAPI(dataProviderConfig);
        //Wallet = new WalletProvider();
        Contracts = new Contracts(BeaconConnector, API);
        
        InitBeaconConnector();
        
        MessageReceiver.AccountConnected += Callback_OnAccountConnected;
        MessageReceiver.AccountConnectionFailed += Callback_OnAccountConnectionFailed;
        MessageReceiver.AccountDisconnected += Callback_OnAccountDisconnected;
        MessageReceiver.HandshakeReceived += Callback_OnHandshakeReceived;
        MessageReceiver.PairingCompleted += Callback_OnPairingCompleted;
    }
    
    private void InitBeaconConnector()
    {
        // Assign the BeaconConnector depending on the platform.
#if UNITY_WEBGL && !UNITY_EDITOR
		BeaconConnector = new BeaconConnectorWebGl();
#else
        BeaconConnector = new BeaconConnectorDotNet();
        (BeaconConnector as BeaconConnectorDotNet)?.SetWalletMessageReceiver(MessageReceiver);
        Connect(WalletProviderType.beacon, withRedirectToWallet: false);
#endif
        
    }
    
    public void Connect(WalletProviderType walletProvider, bool withRedirectToWallet)
    {
        BeaconConnector.InitWalletProvider(
            network: TezosConfig.Instance.Network.ToString(),
            rpc: TezosConfig.Instance.RpcBaseUrl,
            walletProviderType: walletProvider);

        BeaconConnector.ConnectAccount();
#if UNITY_ANDROID || UNITY_IOS
            if (withRedirectToWallet)
                Application.OpenURL($"tezos://?type=tzip10&data={_handshake}");
#endif
    }
    
    public void Disconnect()
    {
        BeaconConnector.DisconnectAccount();
    }

    public string GetActiveAddress()
    {
        return BeaconConnector.GetActiveAccountAddress();
    }
    
    public IEnumerator GetTezosBalance(Action<ulong> callback, string address)
    {
        return API.GetTezosBalance(callback, address);
    }
    
    public IEnumerator GetLatestBlockLevel(Action<int> callback)
    {
        return API.GetLatestBlockLevel(callback);
    }

    public void RequestSignPayload(SignPayloadType signingType, string payload)
    {
        BeaconConnector.RequestTezosSignPayload(signingType, payload);
    }
    
    public void RequestTransferTezos(string to, ulong amount = 0)
    {
        BeaconConnector.RequestTezosOperation(
            destination: to,
            entryPoint: "default",
            arg: "{\"prim\": \"Unit\"}",
            amount: amount,
            networkName: TezosConfig.Instance.Network.ToString(),
            networkRPC: TezosConfig.Instance.RpcBaseUrl);
    }

    public bool VerifySignedPayload(SignPayloadType signingType, string payload, string pubKey, string signature)
    {
        return NetezosExtensions.VerifySignature(pubKey, signingType, payload, signature);
    }
    
    public void CallContract(
        string contractAddress,
        string entryPoint,
        string input,
        ulong amount = 0)
    {
        BeaconConnector.RequestTezosOperation(
            destination: contractAddress,
            entryPoint: entryPoint,
            arg: input,
            amount: amount,
            networkName: TezosConfig.Instance.Network.ToString(),
            networkRPC: TezosConfig.Instance.RpcBaseUrl);
    }
    
    public struct TransactionResult
    {
        public bool success;
        public string transactionHash;
    }
    
    public IEnumerator TrackTransaction(string transactionHash, Action<TransactionResult> onTransactionCompleted, float timeoutInSeconds = 30, float secondsToWait = 1)
    {
        var success = false;
        var startTimestamp = Time.time;

        // keep making requests until time out or success
        while (!success && Time.time - startTimestamp < timeoutInSeconds)
        {
            Logger.LogDebug($"Checking tx status: {transactionHash}");
            yield return API.GetOperationStatus(result =>
            {
                if (result != null)
                    success = JsonSerializer.Deserialize<bool>(result);
            }, transactionHash);

            yield return new WaitForSecondsRealtime(secondsToWait);
        }

        TransactionResult result;
        result.success = success;
        result.transactionHash = transactionHash;
        onTransactionCompleted?.Invoke(result);
    }
    
    #region Callbacks
    
    private void Callback_OnAccountConnected(string account)
    {
        var json = JsonSerializer.Deserialize<JsonElement>(account);
        if (!json.TryGetProperty("accountInfo", out json)) return;

        _pubKey = json.GetProperty("publicKey").GetString();
        string accountAddress = json.GetProperty("address").GetString();
    }
    
    private void Callback_OnAccountConnectionFailed(string result)
    {
    }
    
    private void Callback_OnAccountDisconnected(string result)
    {
        _pubKey = "";
    }
    
    private void Callback_OnHandshakeReceived(string handshake)
    {
        _handshake = handshake;
    }
    
    private void Callback_OnPairingCompleted(string result)
    {
        BeaconConnector.RequestTezosPermission(networkName: TezosConfig.Instance.Network.ToString(), networkRPC: TezosConfig.Instance.RpcBaseUrl);
    }
    
    private void Callback_OnAccountReceived(string result)
    {
    }

    #endregion
}
