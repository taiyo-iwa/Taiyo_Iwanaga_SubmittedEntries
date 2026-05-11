using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialGameManager : MonoBehaviour
{
    //カウントダウン等のUIを表示させるために取得
    [SerializeField] private UIController _uiController = default;
    //ゴール判定イベントを取得するために使う
    [SerializeField] private LapController _lapController = default;
    [SerializeField] private LapController _twoPlayerLapController = default;
    //BGMを再生させるために必要
    [SerializeField] private AudioSource _audioSource = default;
    //SEを再生させるために必要
    [SerializeField] private AudioManager _audioManager = default;
    //リザルトでカメラを遷移させるために使う
    [SerializeField] private CameraGoToPodium _cameraGoToPodium = default;
    //リザルトでドラゴンを移動させるのに使う（順位データが必要なため_pathRankingのメソッドを使う）
    [SerializeField] private pathRanking _pathRanking = default;
    //ドラゴンたちのUpdate処理をここのスクリプトで実行する(ゲーム中の場合のみ)
    [SerializeField] private TutorialPlayerMove[] _playerMoves = default;
    //リザルトに遷移するときに必要な情報を渡すのに使う
    [SerializeField] private ResultInformation _resultInformation = default;
    //前のシーンで選んだドラゴンの番号を格納しているスクリプト
    private CharacterDataManager _characterDataManager = default;
    //前のシーンで選んだドラゴンの番号を格納しているスクリプト(2P)
    private Character2PDataManager _character2PDataManager = default;

    //タイム計測のため、スタート時の時間を記録する
    private float _startTime = 0;
    //プレイヤーのゴール時のタイムを記録
    private float _onePlayerGoalTime = 0;
    private float _twoPlayerGoalTime = 0;

    private bool _isOnePlayerGoal = false;
    private bool _isTwoPlayerGoal = false;

    /// <summary>
    /// ゲームの進行状況を入れる
    /// 他スクリプトからの値の参照はOK ただし書き換えるのはこのスクリプトでのみ
    /// </summary>
    public GameState _currentState { get; private set; }

    public enum GameState
    {
        None,   //初期状態
        Booting,//準備前
        Ready,  //準備中
        Playing,//ゲーム中
        Finish, //ゴール直後
        Result, //リザルト画面
    }

    //やむを得ない理由でこのコードを用意しているけど、マージした後にこれを消す作業を後でする
    public bool isGameStarted = false;

    public int _selectDragon { get; set; } = 0;
    public int _twoPlayerSelectDragon { get; set; } = -1;

    private void Awake()
    {
        //ドラゴンの番号が入ったオブジェクトを探す
        //１Pプレイ用と２Pプレイ用のオブジェクトを探す。そしてスクリプトを見つける。
        GameObject selectData1P = GameObject.Find("CharacterDataManager");
        GameObject selectData2P = GameObject.Find("Character2PDataManager");
        if (selectData1P != null)
        {
            _characterDataManager = selectData1P.GetComponent<CharacterDataManager>();
        }
        else if (selectData2P != null)
        {
            _character2PDataManager = selectData2P.GetComponent<Character2PDataManager>();
        }

        //1Pのオブジェクトが見つかり、データが入っていたら番後を入れていく
        //1Pが見つからなければ、２P用のシーンなので１Pと２Pのドラゴン番号を入れる
        if (_characterDataManager != null)
        {
            _selectDragon = _characterDataManager.SelectedCharacter;
        }
        else if (_character2PDataManager != null)
        {
            _selectDragon = _character2PDataManager.SelectedCharacter;
            _twoPlayerSelectDragon = _character2PDataManager.Selected2PCharacter;
        }
    }

    private void Start()
    {
        _currentState = GameState.Booting;
    }

    private void Update()
    {
        //準備中であればボタンが押されたかどうかの検知のみ行う
        if (_currentState == GameState.Booting)
        {
            // スタートボタンが押されたらカウントダウン開始
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetButton("Abtn"))
            {
                //_currentState = GameState.Ready;
                //_uiController.GameReady();
                //_audioManager.PlaySE(AudioManager.SEType.countDown);
            }
            return;
        }
        //ドラゴンたちのUpdate処理を行う(ゲーム中、終了直後のみ)
        if (_currentState == GameState.Playing || _currentState == GameState.Finish)
        {
            foreach (TutorialPlayerMove playerMove in _playerMoves)
            {
                //playerMove.PlayUpdate();
            }
        }
    }

    //レースがスタートされたときに実行　
    public void StartRace()
    {
        _startTime = Time.time;
        _currentState = GameState.Playing;
        _audioSource.Play();
    }

    // ゴールした時の処理　
    public void OnePlayerOnGoal()
    {
        _isOnePlayerGoal = true;
        _onePlayerGoalTime = Time.time - _startTime;
        //UIにゴール画面に移行するまでのアニメーションを再生させる
        _uiController.OnePlayerGoal();
        _audioManager.PlaySE(AudioManager.SEType.goalWhistle);
        if ((_twoPlayerSelectDragon < 0 && _isOnePlayerGoal) || (_twoPlayerSelectDragon >= 0 && _isTwoPlayerGoal && _isOnePlayerGoal))
        {
            _currentState = GameState.Finish;
            //BGMを止める
            _audioSource.Stop();
            StartCoroutine(ChangeResult());
        }
    }
    public void TwoPlayerOnGoal()
    {
        _isTwoPlayerGoal = true;
        _twoPlayerGoalTime = Time.time - _startTime;
        //UIにゴール画面に移行するまでのアニメーションを再生させる
        _uiController.TwoPlayerGoal();
        _audioManager.PlaySE(AudioManager.SEType.goalWhistle);
        if ((_twoPlayerSelectDragon < 0 && _isOnePlayerGoal) || (_twoPlayerSelectDragon >= 0 && _isTwoPlayerGoal && _isOnePlayerGoal))
        {
            _currentState = GameState.Finish;
            //BGMを止める
            _audioSource.Stop();
            StartCoroutine(ChangeResult());
        }
    }

    public IEnumerator ChangeResult()
    {
        yield return new WaitForSeconds(3.25f);
        _pathRanking.CallPodiumMove();
        _resultInformation._onePlayerSelect = _selectDragon;
        _resultInformation._twoPlayerSelect = _twoPlayerSelectDragon;
        _resultInformation._onePlayerTime = _onePlayerGoalTime;
        _resultInformation._twoPlayerTime = _twoPlayerGoalTime;
        SceneManager.LoadScene("Result");
    }

    //リザルトを表示するときの処理
    public void ShowResult()
    {
        //_cameraGoToPodium.CameraMove();

        //SceneManager.LoadScene("Result");
        //_audioManager.PlaySE(AudioManager.SEType.showResult);
    }

    void OnEnable()
    {
        _uiController.OnStart += StartRace;
        _uiController.OnResult += ShowResult;
        _lapController.OnGoalEvent += OnePlayerOnGoal;
        if (_twoPlayerLapController != null)
        {
            _twoPlayerLapController.OnGoalEvent += TwoPlayerOnGoal;
        }
    }

    private void OnDisable()
    {
        _uiController.OnStart -= StartRace;
        _uiController.OnResult -= ShowResult;
        _lapController.OnGoalEvent -= OnePlayerOnGoal;
        if (_twoPlayerLapController != null)
        {
            _twoPlayerLapController.OnGoalEvent -= TwoPlayerOnGoal;
        }
    }
}
