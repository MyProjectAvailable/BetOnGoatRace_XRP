using Cysharp.Threading.Tasks;
using Defective.JSON;
using Fusion.Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class CoreChainManager : MonoBehaviourSingletonPersistent<CoreChainManager>
{
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    public static string abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnerAdded\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"oldOwner\",\"type\":\"address\"}],\"name\":\"OwnerRemoved\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"byOwner\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"Withdrawn\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"GetCurrentTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"_result\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address payable\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"addOwner\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"name\":\"bets\",\"outputs\":[{\"internalType\":\"address payable\",\"name\":\"bettor\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"int256\",\"name\":\"_itemId\",\"type\":\"int256\"}],\"name\":\"buyCoins\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_winnerHorseId\",\"type\":\"uint256\"}],\"name\":\"endRace\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"userProvidedNumber\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"userProvidedNumberMultiply\",\"type\":\"uint256\"}],\"name\":\"generateRandomNumber\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"name\":\"isOwner\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"ownerCount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"horseId\",\"type\":\"uint256\"}],\"name\":\"placeBet\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"poolAmount\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"raceEndTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"raceStatus\",\"outputs\":[{\"internalType\":\"enum AnimalBetRacev1.RaceStatus\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address payable\",\"name\":\"oldowner\",\"type\":\"address\"}],\"name\":\"removeOwner\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_time\",\"type\":\"uint256\"}],\"name\":\"startRace\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalPurchaseOrder\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"winnerHorseId\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"withdrawFund\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";

    public static string contract = "";

    const string chain = "";
    const string network = "";
    static string chainId = "";
    static string networkRPC = "";

    public static int chainType = 0;

    float[] coinCost = { 1, 2, 3, 4 };

    public static string userBalance = "0";



    private int expirationTime;
    private string account;

    public static bool isOwner = false;

    [SerializeField] TMP_Text _status;
    ProjectConfigScriptableObject projectConfigSO = null;

    //multichain stuff
    [DllImport("__Internal")]
    private static extern int SetChainNetwork(int _value);

    public void ChangeChain(int _no)
    {
        switch (_no)
        {
            case 3: // XRPL EVM Sidechain Devnet
                chainId = "1440002";
                networkRPC = "https://rpc-evm-sidechain.xrpl.org";
                contract = "0x3D99d084EA6EAdD64F9514db878a82480d451bb6";
                vrfcontract = "";
                Debug.Log("XRPL EVM Sidechain Devnet");
                ConstantManager.getProfile_api = "https://firestore.googleapis.com/v1/projects/metahackprojects/databases/(default)/documents/horsebet_XRPEVM_userdata/";
                PhotonAppSettings.Instance.AppSettings.AppVersion = "XRPEVM_1.0.0";
                chainType = 3;
                break;



        }

#if !UNITY_EDITOR
    SetChainNetwork(int.Parse(chainId));
#endif

        projectConfigSO = (ProjectConfigScriptableObject)Resources.Load("ProjectConfigData", typeof(ScriptableObject));
        projectConfigSO.ChainId = chainId;
        projectConfigSO.Rpc = networkRPC;

        PlayerPrefs.SetString("ProjectID", projectConfigSO.ProjectId);
        PlayerPrefs.SetString("ChainID", projectConfigSO.ChainId);
        PlayerPrefs.SetString("Chain", projectConfigSO.Chain);
        PlayerPrefs.SetString("Network", projectConfigSO.Network);
        PlayerPrefs.SetString("RPC", projectConfigSO.Rpc);
        PlayerPrefs.Save();

        LoginWallet();

    }




    public static string SecondsToFormattedTime(int totalSeconds)
    {
        int hours = totalSeconds / 3600;
        int remainingSeconds = totalSeconds % 3600;
        int minutes = remainingSeconds / 60;
        int seconds = remainingSeconds % 60;

        return string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }

    public async UniTaskVoid LoginWallet()
    {
        _status.text = "Connecting...";
#if !UNITY_EDITOR
        Web3Connect();
        OnConnected();
#else
        // get current timestamp
        int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // set expiration time
        int expirationTime = timestamp + 60;
        // set message
        string message = "Welcome to Animal Betting Game\n" + expirationTime.ToString();
        // sign message
        string signature = await Web3Wallet.Sign(message);
        // verify account
        string account = await EVM.Verify(message, signature);
        int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // validate
        if (account.Length == 42 && expirationTime >= now)
        {
            // save account
            PlayerPrefs.SetString("Account", account.ToLower());

            print("Account: " + account.ToLower());
            _status.text = "connected : " + account.ToLower();

            await UniTask.Delay(50);
            CheckUserBalance();
            await UniTask.Delay(50);
            getRaceStausRepeating();
            await UniTask.Delay(50);
            getRaceEndTimeRepeat();
            await UniTask.Delay(50);
            CheckTimeStatusReapeatly();
            await UniTask.Delay(50);
            //getTokenBalance();
            getRaceStaus();
            await UniTask.Delay(50);
            getPoolAmount();
            await UniTask.Delay(50);
            getPoolOwner();
            await UniTask.Delay(50);
            getRaceEndTime();
            await UniTask.Delay(50);
            CheckTimeStatus();
            await UniTask.Delay(50);

            if (DatabaseManager.Instance)
            {
                DatabaseManager.Instance.GetData(true);
            }
            // load next scene


        }



        SingletonDataManager.userethAdd = account.ToLower();
        //CovalentManager.insta.GetNFTUserBalance();
        // PhotonManager.Instance.ConnectToPhotonNow();

        Debug.Log("Start Game");
        SceneManager.LoadScene(1);

        //var checkThis = await requestRandomNumber();
        //Debug.Log("Random Number is " + checkThis);
#endif

    }



    async UniTaskVoid OnConnected()
    {

        account = ConnectAccount();
        while (account == "")
        {
            await new WaitForSeconds(1f);
            account = ConnectAccount();
        };

        //account = account.ToLower();
        // save account for next scene
        PlayerPrefs.SetString("Account", account.ToLower());
        // reset login message
        SetConnectAccount("");

        _status.text = "connected : " + account.ToLower();
        // reset login message
        //SetConnectAccount("");
        await UniTask.Delay(50);
        CheckUserBalance();
        await UniTask.Delay(50);
        getRaceStausRepeating();
        await UniTask.Delay(50);
        getRaceEndTimeRepeat();
        await UniTask.Delay(50);
        CheckTimeStatusReapeatly();
        await UniTask.Delay(50);
        //getTokenBalance();
        getRaceStaus();
        await UniTask.Delay(50);
        getPoolAmount();
        await UniTask.Delay(50);
        getPoolOwner();
        await UniTask.Delay(50);
        getRaceEndTime();
        await UniTask.Delay(50);
        CheckTimeStatus();
        await UniTask.Delay(50);

        if (DatabaseManager.Instance)
        {
            DatabaseManager.Instance.GetData(true);
        }
        // load next scene
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);


        SingletonDataManager.userethAdd = account.ToLower();


        //CoinBuyOnSendContract(0);
        SceneManager.LoadScene(1);

    }
    #region Bet
    async public UniTaskVoid PutBet(float _BetAmount, int _HorseToBet)
    {

        object[] inputParams = { _HorseToBet };

        float _amount = _BetAmount;
        float decimals = 1000000000000000000; // 18 decimals
        float wei = _amount * decimals;
        print(Convert.ToDecimal(wei).ToString() + " " + inputParams.ToString() + " + " + Newtonsoft.Json.JsonConvert.SerializeObject(inputParams));

        string method = "placeBet";
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        string value = Convert.ToDecimal(wei).ToString();
        string gasLimit = "";
        string gasPrice = "";
        try
        {
#if !UNITY_EDITOR
            string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
            Debug.Log(response);
#else
            string data = await EVM.CreateContractData(abi, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contract, value, data, gasLimit, gasPrice);
            Debug.Log(response);
#endif

            if (!string.IsNullOrEmpty(response))
            {




                LocalData data2 = DatabaseManager.Instance.GetLocalData();
                data2.realBetData.whichHorsesBetted[_HorseToBet] = true;
                DatabaseManager.Instance.UpdateData(data2);

                RealBetUI realBetUI = FindObjectOfType<RealBetUI>();
                if (realBetUI != null)
                {
                    realBetUI.DisableBetting();
                }
                MessageBox.Instance.showMsg("Bet Placed Successfully! now please wait for race to begin.", true);


            }

        }
        catch (Exception e)
        {
            if (MessageBox.Instance) MessageBox.Instance.showMsg("Transaction Has Been Failed", true);
            Debug.Log(e, this);
        }
    }

    async public UniTaskVoid EndRace(int _horseNo)
    {

        Debug.Log("Wiiner sent " + _horseNo);

        object[] inputParams = { _horseNo };

        float _amount = 0;
        float decimals = 1000000000000000000; // 18 decimals
        float wei = _amount * decimals;
        print(Convert.ToDecimal(wei).ToString() + " " + inputParams.ToString() + " + " + Newtonsoft.Json.JsonConvert.SerializeObject(inputParams));

        string method = "endRace";
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        string value = Convert.ToDecimal(wei).ToString();
        string gasLimit = "";
        string gasPrice = "";
        try
        {
#if !UNITY_EDITOR
            string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
            Debug.Log(response);
#else
            string data = await EVM.CreateContractData(abi, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contract, value, data, gasLimit, gasPrice);
            Debug.Log(response);
#endif

            if (!string.IsNullOrEmpty(response))
            {

            }

        }
        catch (Exception e)
        {
            if (MessageBox.Instance) MessageBox.Instance.showMsg("Transaction Has Been Failed", true);
            Debug.Log(e, this);
        }
    }
    async public UniTaskVoid StartRace(float _setPoolAmount, int _minutes)
    {

        object[] inputParams = { _minutes };

        float _amount = _setPoolAmount;
        float decimals = 1000000000000000000; // 18 decimals
        float wei = _amount * decimals;
        print(Convert.ToDecimal(wei).ToString() + " " + inputParams.ToString() + " + " + Newtonsoft.Json.JsonConvert.SerializeObject(inputParams));

        string method = "startRace";
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        string value = Convert.ToDecimal(wei).ToString();
        string gasLimit = "";
        string gasPrice = "";
        try
        {
#if !UNITY_EDITOR
            string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
            Debug.Log(response);
#else
            string data = await EVM.CreateContractData(abi, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contract, value, data, gasLimit, gasPrice);
            Debug.Log(response);
#endif

            if (!string.IsNullOrEmpty(response))
            {

            }

        }
        catch (Exception e)
        {
            if (MessageBox.Instance) MessageBox.Instance.showMsg("Transaction Has Been Failed", true);
            Debug.Log(e, this);
        }
    }
    #endregion

    #region BuyCoins
    async public void CoinBuyOnSendContract(int _pack)
    {
        if (MessageBox.Instance) MessageBox.Instance.showMsg("Coin purchase process started\nThis can up to minute", false);

        object[] inputParams = { _pack };

        float _amount = coinCost[_pack];
        float decimals = 1000000000000000000; // 18 decimals
        float wei = _amount * decimals;
        print(Convert.ToDecimal(wei).ToString() + " " + inputParams.ToString() + " + " + Newtonsoft.Json.JsonConvert.SerializeObject(inputParams));
        // smart contract method to call
        string method = "buyCoins";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = Convert.ToDecimal(wei).ToString();
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {


#if !UNITY_EDITOR
            string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
            Debug.Log(response);
#else
            // string response = await EVM.c(method, abi, contract, args, value, gasLimit, gasPrice);
            // Debug.Log(response);
            string data = await EVM.CreateContractData(abi, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contract, value, data, gasLimit, gasPrice);


            Debug.Log(response);
#endif

            if (!string.IsNullOrEmpty(response))
            {
                if (DatabaseManager.Instance)
                {
                    DatabaseManager.Instance.AddTransaction(response, "pending", _pack);
                }

                CheckTransactionStatus(response);
                if (MessageBox.Instance) MessageBox.Instance.showMsg("Your Transaction has been recieved\nCoins will reflect to your account once it is completed!", true);
            }




        }
        catch (Exception e)
        {
            if (MessageBox.Instance) MessageBox.Instance.showMsg("Transaction Has Been Failed", true);
            Debug.Log(e, this);
        }
    }
    #endregion

    #region CheckUserBalance
    public async UniTaskVoid CheckUserBalance()
    {

        COMEHERE:
        try
        {

            string response = await EVM.BalanceOf(chain, network, PlayerPrefs.GetString("Account"), networkRPC);
            Debug.Log("CheckUserBalance " + response);
            if (!string.IsNullOrEmpty(response))
            {
                float wei = float.Parse(response);
                float decimals = 1000000000000000000; // 18 decimals
                float eth = wei / decimals;
                // print(Convert.ToDecimal(eth).ToString());
                Debug.Log(Convert.ToDecimal(eth).ToString());
                userBalance = Convert.ToDecimal(eth).ToString();
                /* if (StoreManager.insta)
                 {
                     StoreManager.insta.SetBalanceText();
                 }*/
                // GameUIManager.Instance.UpdateBalance();
            }
        }
        catch (Exception e)
        {
            Debug.Log("CheckUserBalance " + e);
        }

        await UniTask.Delay(UnityEngine.Random.Range(5200, 12500));
        goto COMEHERE;
    }
    #endregion

    #region CheckTRansactionStatus
    //private string transID;
    public static string userTokenBalance = "0";

    public async UniTaskVoid CheckTransactionStatus(string _tranID)
    {
        bool NoCheckAgain = false;
        COMEHERE:
        await UniTask.Delay(UnityEngine.Random.Range(4000, 10000));
        try
        {
            string txConfirmed = await EVM.TxStatus(chain, network, _tranID, networkRPC);
            print(txConfirmed); // success, fail, pending
            if (txConfirmed.Equals("success") || txConfirmed.Equals("fail"))
            {
                NoCheckAgain = true;
                // NonBurnNFTBuyContract(0, "ipfs://bafyreigkpnryq6t53skpbmfylegrp7wl3xkegzxq7ogimvnkzdceisya4a/metadata.json");
                // CancelInvoke("CheckTransactionStatus");
                if (DatabaseManager.Instance)
                {
                    DatabaseManager.Instance.ChangeTransactionStatus(_tranID, txConfirmed);
                }

            }

        }
        catch (Exception e)
        {
            Debug.Log(e, this);
        }

        if (!NoCheckAgain) goto COMEHERE;
    }


    #endregion

    #region RaceInfo

    public async static UniTask getRaceStausRepeating()
    {

        COMEHERE:
        // smart contract method to call
        string method = "raceStatus";
        // array of arguments for contract
        object[] inputParams = { };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        var _raceStatus = "";
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            _raceStatus = response;
            raceStatus = response;
            Debug.Log("getRaceStaus " + response);
        }
        catch (Exception e)
        {
            Debug.Log("getRaceStausRepeating " + e);
        }

        if (_raceStatus != "1")
        {
            PlayerPrefs.SetString("placedbet", "");
            if (DatabaseManager.Instance != null)
            {
                DatabaseManager.Instance.resetBettingData();
            }
        }



        await UniTask.Delay(UnityEngine.Random.Range(5200, 12500));
        goto COMEHERE;

    }

    public static string raceStatus;
    public static string raceEndTime;
    public static long raceEndTimeLong;

    public async static UniTask<string> getRaceStaus()
    {
        await UniTask.Delay(1500);
        // smart contract method to call
        string method = "raceStatus";
        // array of arguments for contract
        object[] inputParams = { };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            raceStatus = response;
            Debug.Log("getRaceStaus " + response);
            if (raceStatus != "1")
            {
                PlayerPrefs.SetString("placedbet", "");



            }

            return response;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return "";
    }

    public async static UniTask<int> getRandomNumberDirect()
    {

        // smart contract method to call
        string method = "generateRandomNumber";
        // array of arguments for contract
        object[] inputParams = { UnityEngine.Random.Range(100000, 99999999), UnityEngine.Random.Range(999, 9999) };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            raceStatus = response;
            Debug.Log("getRandomNumber " + response);

            try
            {
                return int.Parse(response);
            }
            catch (Exception e)
            {

                Debug.Log("Error " + e.Message);
            }


        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return UnityEngine.Random.Range(0, 9);
    }


    public static string totalPoolAmount;
    public async static UniTask<string> getPoolAmount()
    {
        // smart contract method to call
        string method = "poolAmount";
        // array of arguments for contract
        object[] inputParams = { };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);

            float decimals = 1000000000000000000; // 18 decimals
            float poolAmount = (float.Parse(response)) / decimals;
            // print(Convert.ToDecimal(eth).ToString());
            Debug.Log(Convert.ToDecimal(poolAmount).ToString());
            string pAmount = Convert.ToDecimal(poolAmount).ToString();
            totalPoolAmount = pAmount;
            Debug.Log("totalPool " + pAmount);
            return pAmount;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return "";
    }


    public static long startTime;
    public async Task<long> CheckTimeStatus()
    {
        await UniTask.Delay(500);
        // smart contract method to call
        string method = "GetCurrentTime";
        // array of arguments for contract
        object[] inputParams = { };
        long currentEpoch = 1659504437;
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            Debug.Log(response);

            Debug.Log("result: " + response);
            if (!string.IsNullOrEmpty(response))
            {
                currentEpoch = long.Parse(response);
            }
            else
            {
                currentEpoch = 1659504437;
            }
            startTime = currentEpoch;
        }
        catch (Exception e)
        {
            Debug.Log(e, this);

        }
        return currentEpoch;
    }

    public async UniTaskVoid CheckTimeStatusReapeatly()
    {
        COMEHERE:
        // smart contract method to call
        string method = "GetCurrentTime";
        // array of arguments for contract
        object[] inputParams = { };
        long currentEpoch = 1659504437;
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            Debug.Log(response);

            Debug.Log("result: " + response);
            if (!string.IsNullOrEmpty(response))
            {
                currentEpoch = long.Parse(response);
            }
            else
            {
                currentEpoch = 1659504437;
            }
            startTime = currentEpoch;
        }
        catch (Exception e)
        {
            Debug.Log(e, this);

        }
        await UniTask.Delay(UnityEngine.Random.Range(5200, 12500));
        goto COMEHERE;
    }

    public async static UniTask<int> getHorseWinner()
    {
        // smart contract method to call
        string method = "winnerHorseId";
        // array of arguments for contract
        object[] inputParams = { };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            int number = int.Parse(response);
            Debug.Log("winnerHorseId " + response);
            return number;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return -1;
    }



    public async static UniTask<bool> getPoolOwner()
    {
        // smart contract method to call
        // smart contract method to call
        string method = "isOwner";
        // array of arguments for contract
        object[] inputParams = { PlayerPrefs.GetString("Account").ToLower() };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            Debug.Log("owner " + response);

            if (response.ToLower().Equals("true"))
            {
                Debug.Log("You are Owner ");
                isOwner = true;
            }
            else
            {
                Debug.Log("You are NOT Owner ");
                isOwner = false;
            }
            return isOwner;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return false;
    }

    public async static UniTask<string> getRaceEndTime()
    {
        // smart contract method to call
        string method = "raceEndTime";
        // array of arguments for contract
        object[] inputParams = { };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            raceEndTime = response;
            raceEndTimeLong = long.Parse(raceEndTime);
            Debug.Log("raceEndTime " + response);
            return response;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        return "";
    }
    public async static UniTaskVoid getRaceEndTimeRepeat()
    {
        COMEHERE:
        // smart contract method to call
        string method = "raceEndTime";
        // array of arguments for contract
        object[] inputParams = { };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call(chain, network, contract, abi, method, args, networkRPC);
            raceEndTime = response;
            raceEndTimeLong = long.Parse(raceEndTime);
            Debug.Log("raceEndTime " + response);

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        await UniTask.Delay(UnityEngine.Random.Range(10000, 15000));
        goto COMEHERE;

    }


    #endregion




}

[System.Serializable]
public class RealBetData
{
    public List<bool> whichHorsesBetted = new List<bool>();

    public RealBetData()
    {
        whichHorsesBetted = new List<bool>();
    }
}