using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;


public class TutorialManager : MonoBehaviour
{
    // 何秒で切り替えるか
    [SerializeField] private float interval = 2f;
    // 入力を受け取る対象のAction
    [SerializeField] TutorialPlayerController playerController;
    [SerializeField] TutorialPlayerMove _playerMove = default;
    [SerializeField] PlayerInputHandler _inputHandler = default;
    [SerializeField] EventProgressManager _progressManager = default;
    [SerializeField] TutorialEventSound _tutorialEventSound = default;
    [SerializeField] TutorialPlayerFollow _tutorialPlayerFollow = default;
    [SerializeField] TutorialPlayerHeat _tutorialPlayerHeat = default;
    [SerializeField] AudioSource _audioSource = default;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] GameObject _mainPanel = default;
    [SerializeField] GameObject _sidePanel = default;
    [SerializeField] Text _sideText = default;
    [SerializeField] Image _correctAnswerImage = default;
    [SerializeField] Image _sideCorrectAnswerImage = default;
    [SerializeField] Text _commonText = default;
    [SerializeField] GameObject _nextSceneImage = default;
    [SerializeField] GameObject _AbtnImage = default;
    [SerializeField] GameObject _LStickImage = default;
    [SerializeField] GameObject _BbtnImage = default;
    [SerializeField] GameObject _RBbtnImage = default;
    [SerializeField] GameObject _LBbtnImage = default;
    [SerializeField] GameObject _sideLBImage = default;
    [SerializeField] GameObject _RTbtnImage = default;
    [SerializeField] GameObject _LTbtnImage = default;
    [SerializeField] GameObject _sideLTImage = default;
    [SerializeField] GameObject _ftherEventObject = default;
    [SerializeField] GameObject _ftherChargeArea = default;
    [SerializeField] GameObject _nextScaneImage = default;
    [SerializeField] GameObject _itemBoxImage = default;
    [SerializeField] GameObject _commentFeeds = default;
    [SerializeField] GameObject _commentFeedImage = default;
    [SerializeField] GameObject _enemyDragon = default;
    [SerializeField] GameObject _skipImage = default;
    // 会話文をまとめた配列
    [Header("最初の説明")]
    [SerializeField] [Multiline(2)] private string[] _shartDialogueLines;
    [Header("ドリフトの説明")]
    [SerializeField] [Multiline(2)] private string[] _doriftDiallogueLines;
    [Header("ダッシュの説明")]
    [SerializeField] [Multiline(2)] private string[] _dahsDialogueLines;
    [Header("アイテムの説明")]
    [SerializeField] [Multiline(2)] private string[] _itemDialogueLines;
    [Header("フレアの説明")]
    [SerializeField] [Multiline(2)] private string[] _flameDialogueLines;
    [Header("シールドの説明")]
    [SerializeField] [Multiline(2)] private string[] _shieldDialogueLines;
    [Header("フェザーゲージの説明")]
    [SerializeField] [Multiline(2)] private string[] _fretherDialogueLines;
    [Header("ソアーの使い方説明")]
    [SerializeField] [Multiline(2)] private string[] _fretherGaugeDialogueLines;
    [Header("ソアーの説明")]
    [SerializeField] [Multiline(2)] private string[] _soarDialogueLines;
    [Header("次ステージの行き方の説明")]
    [SerializeField] [Multiline(2)] private string[] _endDialogueLines;

    //セリフの配列に使うインデックス
    private int dialogueIndex = 0;
    private int driftIndex = 0;
    //Aボタンを連続でTrueできないように
    private bool _AbtnTrigger = true;
    private bool _RBbtnTrigger = true;
    private bool _RTbtnTrigger = true;
    //Aボタンの真偽を返すための変数
    private bool _serifReading = false;
    //操作可能か
    private bool _isOperatable = false;
    //イベント中プレイヤーを止める
    public bool _eventStop { get; private set; } = false;
    //説明中(最初からセリフが出るのでTrue)
    private bool inEventSerif = true;
    //セリフ読みのAボタンが
    //見えているか
    private bool _isCommentFeed = false;
    private float _isCommentFeedTimer = 0;

    private bool _Xbtn = false;
    private bool _pendingSkip = false;
    private float _tutorialSkip = 0;
    private float _skipTimer = 0;
    private float _skipTimerLimit = 5.0f;
    private bool _tutorialCompleteTrigger = true;
    public void OnSkip(InputAction.CallbackContext context)
    {
        _tutorialSkip = context.ReadValue<float>();
    }

    private void TutorialSkip()
    {
        if (_tutorialCompleteTrigger)
        {
            _Xbtn = _tutorialSkip != 0f;
            print(_skipTimer);
            if (_Xbtn)
            {
                _nextScaneImage.SetActive(true);
                _skipImage.SetActive(true);
                _pendingSkip = true;
            }
            if (!_Xbtn && _pendingSkip)
            {
                _skipTimer += Time.deltaTime;
                if (_skipTimer > _skipTimerLimit)
                {
                    _skipTimer = 0;
                    _pendingSkip = false;
                    _nextScaneImage.SetActive(false);
                    _skipImage.SetActive(false);
                }
            }
        }
    }

    #region 説明を開始させるbool
    //スタートの条件がないので最初の説明に入るためにTrueにしておく。
    private bool _startExplanation = true;
    private bool _horizontalExplanation = false;
    private bool _soarInExplanation = false;
    private bool _dashCoolTimeExplanation = false;
    private bool _tutorialComplete = false;
    #endregion

    #region 外部発動イベント
    //外部からイベントの開始をもらうため
    public bool _dashEventExplanation { get; set; } = false;
    public bool _driftExplanation { get; set; } = false;
    public bool _itemBoxExplanation { get; set; } = false;
    public bool _crashBoxExplanation { get; set; } = false;
    public bool _frameExplanation { get; set; } = false;
    public bool _shieldExplanation { get; set; } = false;
    public bool _fretherExplanation { get; set; } = false;
    public bool _soarExplanation { get; set; } = false;
    public bool _canSoar { get; set; } = false;
    #endregion

    #region 進行フラグ
    //各イベント進行フラグ
    private bool _driftEventFlag = false;   
    private bool _itemEventFlag = false;
    private bool _flemeEventFlag = false;
    private bool _shieldEventFlag = false;
    private bool _fretherEventFlag = false;

    //ダッシュイベントの時にダッシュできないと困るので、
    //イベント前はダッシュできないようにするためPlayerControllerに変数を渡す
    public bool _dashEventFlag { get; private set; } = false;
    #endregion

    #region InputHandlerからの入力を受け取るための変数
    private float _horizontal = 0;
    private float _vertical = 0;
    private float _leftTriggrt = 0;
    private float _rightTrigger = 0;
    private bool _Abtn = false;
    private bool _Bbtn = false;
    private bool _RBbtn = false;
    private bool _LBbtn = false;
    private bool _RBbtnDown = false;
    private bool _RBbtnUp = false;
    #endregion

    #region 入力したかどうか判定するための変数（float）
    private bool _isLStick = false;
    private bool _isvertical = false;
    private bool _isLTbtn = false;
    private bool _isRTbtn = false;
    #endregion

    #region 各ボタンを押したときのフラグ用変数
    private bool _isLStickEvent = false;
    private bool _isverticalEvent = false;
    private bool _isRTbtnEvent = false;
    private bool _isAbtnEvent = false;    
    private bool _isRBbtnEvent = false;   
    private bool _isCanSoarEvent = false;

    public bool _isBbtnEvent { get; set; } = false;
    private bool _isBbtnEventTrigger = true;
    //アイテムの説明の際に違うアイテムを使われないように
    //pulicにしてPlayerControllerのLB、LTを止める
    public bool _isLBbtnEvent { get; private set; } = false;
    public bool _isLTbtnEvent { get; private set; } = false;
    #endregion

    #region 一度きり用のフラグ
    //最初の一回かぎりなのでTureにしておく
    private bool _isDriftEventTrigger = true;
    private bool _isDashEventTrigger = true;   
    private bool _isItemBoxEventTrigger = true;
    private bool _isCrashBoxEventTrigger = true;
    private bool _isFrameEventTrigger = true;
    private bool _isShieldEventTrigger = true;
    private bool _isFretherEventTrigger = true;
    private bool _isSoarEventTrigger = true;
    private bool _isCanSoarEventTrigger = true;
    private bool _isSoarInTrigger = true;
    private bool _isTutorialEndTrigger = true;
    #endregion

    #region プレイヤーが上手く進行できない時使う変数
    //プレイヤーが止まっていた時に使うタイマーの変数
    private float stopTimer = 0f;
    //警告の開始
    private bool _StartPlayerStopEvent = false;
    //プレイヤー一定時間止まったら
    private bool _playerStopEvent = false;
    #endregion

    #region 入力用変数
    /// <summary>
    /// 入力判定用のメソッド
    /// </summary>
    private void UpdateInput()
    {
        _horizontal = _inputHandler.Horizontal;
        _vertical = _inputHandler.Vertical;
        //_leftTriggrt = _inputHandler.LeftTriggerAxis;
        _rightTrigger = _inputHandler.RightTriggerAxis;
        _Abtn = _inputHandler.IsAccelPressed;
        _Bbtn = _inputHandler.IsFlyPressed;
        _RBbtn = _inputHandler.IsDriftPressed;
        //_LBbtn = _inputHandler.IsItemPressed;
        _RBbtnDown = _inputHandler.IsDriftPressedDown;
        _RBbtnUp = _inputHandler.IsDriftReleased;
    }

    /// <summary>
    /// 入力したか
    /// </summary>
    private void InputDecision()
    {
        if (_horizontal > 0.5f || _horizontal < -0.5f)
        {
            _isLStick = true;
        }
        else if(_horizontal < 0.1f && _horizontal > -0.1f)
        {
            _isLStick = false;
        }
        if (_leftTriggrt > 0)
        {
            _isLTbtn = true;
        }
        else
        {
            _isLTbtn = false;
        }
        if (_rightTrigger > 0)
        {
            _isRTbtn = true;
        }
        else
        {
            _isRTbtn = false;
        }
    }
    #endregion

    #region プレイヤーが操作可能か
    //これ以降はプレイヤーが動いてしまうので
    //RigidbodyのFreezePositionで止める
    private void Operatable()
    {
        if (_isOperatable)
        {
            _playerMove.PlayUpdate();
        }
    }
    private void StopEvent()
    {
        if (_eventStop)
        {
            //ポジション全てとローテーションXZをロック
            _rigidbody.constraints = RigidbodyConstraints.FreezePosition | 
                RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _audioSource.enabled = false;
        }
        else
        {
            //一回全部のチェックボックスをオフ
            _rigidbody.constraints = RigidbodyConstraints.None;
            //次にRigidbody.RotationのX,Zをオンにする
            _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
            _audioSource.enabled = true;
        }
    }
    #endregion

    void Start()
    {
        // 最初の行を表示
        if (_shartDialogueLines.Length > 0)
        {
            _commonText.enabled = true;
            _commonText.text = _shartDialogueLines[dialogueIndex];
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
        }
    }
    private void Update()
    {
        //入力読み取り
        UpdateInput();
        //入力判定
        InputDecision();
        //ボタンイベント
        ButtonEvent();
        //操作可能か
        Operatable();
        StopEvent();

        TutorialSkip();
        //セリフ時Aボタンを押すよう促す
        //CommentFeed(inEventSerif);

        //各イベントの進行状況を教える
        _progressManager.EventProgress(
            driftEvent: _driftEventFlag,
            dashEvent: _dashEventFlag,
            itemEvent: _itemEventFlag,
            flameEvent: _flemeEventFlag,
            shieldEvent: _shieldEventFlag,
            fretherEventFlag: _fretherEventFlag);

        //プレイヤーが一定時間止まっている
        if (_StartPlayerStopEvent)
        {
            //PlayerStop();
        }

        #region チュートリアルの説明
        #region 最初の説明
        //最初の説明に入る
        if (_startExplanation)
        {
            _startExplanation = false;
            StartCoroutine(StartExplanation());
            //if (_Abtn && _AbtnTrigger)
            //{
            //    _AbtnTrigger = false;
            //    StartExplanation();
            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();

            //    //セリフ開始
            //    inEventSerif = true;
            //}
            //else if (!_Abtn)
            //{
            //    _AbtnTrigger = true;
            //}        
        }
        #endregion

        #region 左右操作の説明
        //左右操作の説明
        if (_horizontalExplanation)
        {
            _horizontalExplanation = false;
            StartHorizontalExplanation();
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
        }
        #endregion

        #region ドリフトの説明
        if (_driftExplanation)
        {
            //Lステックのイベント終了
            _isLStickEvent = false;
            StartDriftExplanation();
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();

            _isRBbtnEvent = true;

            //ドリフトの説明終了
            _driftExplanation = false;

            //ドリフトの説明までいった
            _driftEventFlag = true;         
        }
        #endregion

        #region ダッシュの説明
        //ダッシュの説明
        if (_dashEventExplanation)
        {
            //ドリフトのイベント終了
            _isRBbtnEvent = false;
            StartDashExplanation();

            //パネル起動音を再生
            _tutorialEventSound.PanelSound();

            //イベント開始
            _isRTbtnEvent = true;

            //ダッシュの説明終了
            _dashEventExplanation = false;

            //ダッシュの説明までいった
            _dashEventFlag = true;
        }
        #endregion

        #region アイテムボックスの説明
        if (_itemBoxExplanation)
        {
            //ダッシュのイベント終了
            _isRTbtnEvent =　false;

            _itemBoxExplanation = false;
            //イベント開始プレイヤーを止める
            _eventStop = true;
            _StartPlayerStopEvent = false;
            //コメントリセット
            UIExplanationReset();
            _commonText.enabled = true;
            //アイテムの説明までいった
            _itemEventFlag = true;
            StartCoroutine(StartItemBoxExplanation());          
        }
        #endregion

        #region アイテムを獲得したときの説明
        if (_crashBoxExplanation)
        {
            if (_isCrashBoxEventTrigger)
            {
                _isCrashBoxEventTrigger = false;

                //コメントリセット
                UIExplanationReset();
                //ここではメインのパネルもつける
                _mainPanel.SetActive(true);
                //サイドパネルを消す
                _sidePanel.SetActive(false);
                _sideText.enabled = false;
                _itemBoxImage.SetActive(false);

                _commonText.enabled = true;
                _commonText.text = "いいね";

                //アイテムのファイアの説明までいった
                _flemeEventFlag = true;

                //敵のドラゴンを出す
                _enemyDragon.SetActive(true);
            }
        }
        #endregion

        #region ほのおの説明
        if (_frameExplanation)
        {
            _frameExplanation = false;
            //イベント開始プレイヤーを止める
            _eventStop = true;
            _StartPlayerStopEvent = false;
            StartCoroutine(StartFlameExplanation());
            //if (_isFrameEventTrigger)
            //{
            //    _isFrameEventTrigger = false;

            //    //イベント開始プレイヤーを止める
            //    _eventStop = true;
            //    _StartPlayerStopEvent = false;

            //    //コメントリセット
            //    UIExplanationReset();
            //    _commonText.enabled = true;
            //    _commonText.text = _flameDialogueLines[dialogueIndex];

            //    //セリフ開始
            //    inEventSerif = true;
            //}
            //else if (_Abtn && _AbtnTrigger)
            //{
            //    _AbtnTrigger = false;
            //    StartFlameExplanation();
            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();
            //}
            //else if (!_Abtn)
            //{
            //    _AbtnTrigger = true;
            //}
        }
        #endregion

        #region シールドの説明
        if (_shieldExplanation)
        {
            //ファイヤボールの説明終了
            _isLBbtnEvent = false;

            _shieldExplanation = false;
            //先ほど出ていた敵のドラゴンを消す
            _enemyDragon.transform.position = new Vector3(387, 17, 344);
            _enemyDragon.SetActive(false);
            //イベント開始プレイヤーを止める
            _eventStop = true;
            _StartPlayerStopEvent = false;

            //コメントリセット
            UIExplanationReset();
            _mainPanel.SetActive(true);
            _commonText.enabled = true;
            //炎のサイドパネルを消す
            _sidePanel.SetActive(false);
            _sideLBImage.SetActive(false);

            //シールドの説明までいった
            _shieldEventFlag = true;

            StartCoroutine(StartShieldExplanation());

            //if (_isShieldEventTrigger)
            //{
            //    //先ほど出ていた敵のドラゴンを消す
            //    _enemyDragon.transform.position = new Vector3(387, 17, 344);

            //    _isShieldEventTrigger = false;

            //    //イベント開始プレイヤーを止める
            //    _eventStop = true;
            //    _StartPlayerStopEvent = false;

            //    //コメントリセット
            //    UIExplanationReset();
            //    _commonText.enabled = true;
            //    _commonText.text = _shieldDialogueLines[dialogueIndex];

            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();

            //    //セリフ開始
            //    inEventSerif = true;
            //}
            //else if (_Abtn && _AbtnTrigger)
            //{
            //    _AbtnTrigger = false;
            //    StartShieldExplanation();
            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();
            //}
            //else if (!_Abtn)
            //{
            //    _AbtnTrigger = true;
            //}

        }
        #endregion

        #region 羽ゲージの溜め方の説明
        if (_fretherExplanation)
        {
            //シールドイベント終了
            _isLTbtnEvent = false;

            _fretherExplanation = false;

            //イベント開始プレイヤーを止める
            _eventStop = true;
            _StartPlayerStopEvent = false;

            //羽ゲージの説明までいった
            _fretherEventFlag = true;

            //UIをリセット
            UIExplanationReset();
            //炎のサイドパネルを消す
            _sidePanel.SetActive(false);
            _sideLTImage.SetActive(false);

            //メインパネル付ける
            _mainPanel.SetActive(true);
            _commonText.enabled = true;
            StartCoroutine(StartFretherExplanation());

            //if (_isFretherEventTrigger)
            //{
            //    _isFretherEventTrigger = false;

            //    //イベント開始プレイヤーを止める
            //    _eventStop = true;
            //    _StartPlayerStopEvent = false;

            //    //コメントリセット
            //    UIExplanationReset();
            //    _commonText.enabled = true;
            //    _commonText.text = _fretherDialogueLines[dialogueIndex];

            //    //セリフ開始
            //    inEventSerif = true;
            //}
            //else if (_Abtn && _AbtnTrigger)
            //{
            //    _AbtnTrigger = false;
            //    StartFretherExplanation();
            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();
            //}
            //else if (!_Abtn)
            //{
            //    _AbtnTrigger = true;
            //}
            
        }
        #endregion

        #region ソアーの説明
        if (_soarExplanation)
        {
            _soarExplanation = false;

            //イベント開始プレイヤーを止める
            _eventStop = true;
            _StartPlayerStopEvent = false;

            StartCoroutine(StartGaugeFullExplanation());
            //if (_isSoarEventTrigger)
            //{
            //    _isSoarEventTrigger = false;

            //    //イベント開始プレイヤーを止める
            //    _eventStop = true;
            //    _StartPlayerStopEvent = false;

            //    //コメントリセット
            //    UIExplanationReset();
            //    _commonText.enabled = true;
            //    _commonText.text = _fretherGaugeDialogueLines[dialogueIndex];

            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();

            //    //セリフ開始
            //    inEventSerif = true;
            //}
            //else if (_Abtn && _AbtnTrigger)
            //{
            //    _AbtnTrigger = false;
            //    StartGaugeFullExplanation();
            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();
            //}
            //else if (!_Abtn)
            //{
            //    _AbtnTrigger = true;
            //}
        }

        //ソアー中の説明
        if (_soarInExplanation)
        {
            _soarInExplanation = false;

            //Transformの移動なので、
            //Rigidbodyによる移動速度がはたらかないので
            //falseにしておく
            _StartPlayerStopEvent = false;

            UIExplanationReset();

            _commonText.enabled = true;
            StartCoroutine(StartSoarExplanation());

            //if (_isSoarInTrigger)
            //{
            //    _isSoarInTrigger = false;

            //    //Transformの移動なので、
            //    //Rigidbodyによる移動速度がはたらかないので
            //    //falseにしておく
            //    _StartPlayerStopEvent = false;

            //    //コメントリセット
            //    UIExplanationReset();
            //    _commonText.enabled = true;
            //    _commonText.text = _soarDialogueLines[dialogueIndex];

            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();

            //    //セリフ開始
            //    inEventSerif = true;
            //}
            //else if (_Abtn && _AbtnTrigger)
            //{
            //    _AbtnTrigger = false;
            //    StartSoarExplanation();
            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();
            //}
            //else if (!_Abtn)
            //{
            //    _AbtnTrigger = true;
            //}         
        }
        #endregion

        #region チュートリアル完了
        if (_tutorialComplete)
        {
            _tutorialComplete = false;
            StartCoroutine(TutorialComplete());
            //if (_tutorialCompleteTrigger)
            //{
            //    _tutorialCompleteTrigger = false;

            //    //コメントリセット
            //    UIExplanationReset();
            //    _commonText.enabled = true;
            //    _commonText.text = _endDialogueLines[dialogueIndex];

            //    //セリフ開始
            //    inEventSerif = true;
            //}
            //else if (_Abtn && _AbtnTrigger)
            //{
            //    _AbtnTrigger = false;
            //    TutorialComplete();
            //    //パネル起動音を再生
            //    _tutorialEventSound.PanelSound();
            //}
            //else if (!_Abtn)
            //{
            //    _AbtnTrigger = true;
            //}          
        }
        #endregion
        #endregion
    }

    #region イベント時に対応したボタンを押したら
    private void ButtonEvent()
    {
        if (_isAbtnEvent)
        {
            if (_Abtn)
            {
                _isAbtnEvent = false;
                _correctAnswerImage.enabled = true;
                //正解音を再生
                _tutorialEventSound.CorrectAnswer();
                //セリフ終了
                inEventSerif = false;
                //左右操作の説明をする
                _horizontalExplanation = true;
            }
        }
        if (_isLStickEvent)
        {
            if (_isLStick)
            {
                _isLStickEvent = false;
                _correctAnswerImage.enabled = true;
                //正解音を再生
                _tutorialEventSound.CorrectAnswer();
            }
        }
        if (_isRBbtnEvent)
        {
            if (_RBbtn)
            {
                _isRBbtnEvent = false;
                _correctAnswerImage.enabled = true;
                //正解音を再生
                _tutorialEventSound.CorrectAnswer();
            }
        }
        if (_isRTbtnEvent)
        {
            if (_isRTbtn)
            {
                _isRTbtnEvent = false;
                _correctAnswerImage.enabled = true;
                //正解音を再生
                _tutorialEventSound.CorrectAnswer();
            }
        }
        if (_isLBbtnEvent)
        {
            if (_tutorialPlayerHeat._isFlameItemUse)
            {
                _isLBbtnEvent = false;
                _sideCorrectAnswerImage.enabled = true;
                //正解音を再生
                _tutorialEventSound.CorrectAnswer();

                //操作可能
                _eventStop = false;
                _StartPlayerStopEvent = true;
            }
        }
        if (_isLTbtnEvent)
        {
            if (_tutorialPlayerHeat._isShieldItemUse)
            {
                _isLTbtnEvent = false;
                _sideCorrectAnswerImage.enabled = true;
                _tutorialEventSound.CorrectAnswer();
                //操作可能
                _eventStop = false;
                _StartPlayerStopEvent = true;
                //ファイヤーボールをプレイヤーに打つ
                _tutorialPlayerFollow.FireballShot();
            }
        }
        if (_isCanSoarEvent)
        {
            _ftherChargeArea.SetActive(true);          
            if (_canSoar && _isCanSoarEventTrigger)
            {
                //ソアーできるか(一度きり)
                _isCanSoarEventTrigger = false;
                //ソアーの説明に入る
                _soarExplanation = true;
            }
        }
        if (_isBbtnEvent && _isBbtnEventTrigger)
        {
            if (_Bbtn)
            {
                _isBbtnEventTrigger = false;
                _soarInExplanation = true;
            }
        }
    }
    #endregion

    #region UIをリセットするメソッド
    private void UIExplanationReset()
    {
        _correctAnswerImage.enabled = false;
        _sideCorrectAnswerImage.enabled = false;
        _commonText.enabled = false;
        _AbtnImage.SetActive(false);
        _LStickImage.SetActive(false);
        _BbtnImage.SetActive(false);
        _RBbtnImage.SetActive(false);
        _LBbtnImage.SetActive(false);
        _RTbtnImage.SetActive(false);
        _LTbtnImage.SetActive(false);
    }
    #endregion

    #region 最初の導入とアクセルの説明
    //private void StartExplanation()
    //{
    //    dialogueIndex++;

    //    if (dialogueIndex == 2)
    //    {
    //        _AbtnImage.SetActive(true);
    //    }

    //    if (dialogueIndex < _shartDialogueLines.Length)
    //    {
    //        _commonText.text = _shartDialogueLines[dialogueIndex];
    //    }
    //    else
    //    {
    //        //ここでUIを閉じたり、次のイベントに移る

    //        //操作可能にする
    //        _isOperatable = true;

    //        //Aボタンを押したら丸を出す
    //        _isAbtnEvent = true;

    //        //ここからプレイヤーは一定時間止まっていると警告を受ける
    //        _StartPlayerStopEvent = true;

    //        //最初の説明終了
    //        _startExplanation = false;

    //        //インデックスリセット
    //        dialogueIndex = 0;
    //    }
    //}

    private IEnumerator StartExplanation()
    {
        foreach (string msg in _shartDialogueLines)
        {
            _commonText.text = msg;
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
            yield return new WaitForSeconds(interval); // interval秒待つ
        }

        //パネル起動音を再生
        _tutorialEventSound.PanelSound();
        //ここでUIを閉じたり、次のイベントに移る
        _commonText.text = "";
        _AbtnImage.SetActive(true);
        //操作可能にする
        _isOperatable = true;

        //Aボタンを押したら丸を出す
        _isAbtnEvent = true;

        //ここからプレイヤーは一定時間止まっていると警告を受ける
        _StartPlayerStopEvent = true;

        //最初の説明終了
        _startExplanation = false;
    }
    #endregion

    #region 左右操作の説明
    private void StartHorizontalExplanation()
    {
        StartCoroutine(StartHorizontalExplanationWait());
    }

    private IEnumerator StartHorizontalExplanationWait()
    {
        yield return new WaitForSeconds(1.5f);

        //UIをリセット
        UIExplanationReset();

        yield return new WaitForSeconds(0.1f);

        _LStickImage.SetActive(true);
        //パネル起動音を再生
        _tutorialEventSound.PanelSound();

        yield return new WaitForSeconds(0.5f);

        _isLStickEvent = true;
    }
    #endregion

    #region ドリフトの説明
    private void StartDriftExplanation()
    {
        UIExplanationReset();

        _RBbtnImage.SetActive(true);
    }
    #endregion

    #region ダッシュの説明
    private void StartDashExplanation()
    {
        //コメントリセット
        UIExplanationReset();

        _RTbtnImage.SetActive(true);
    }
    #endregion

    #region アイテムボックスを取るようにの指示
    //private void StartItemBoxExplanation()
    //{
    //    dialogueIndex++;

    //    if (dialogueIndex < _itemDialogueLines.Length)
    //    {
    //        _commonText.text = _itemDialogueLines[dialogueIndex];
    //    }
    //    else
    //    {
    //        //ここでUIを閉じたり、次のイベントに移る

    //        //操作可能
    //        _eventStop = false;
    //        _StartPlayerStopEvent = true;

    //        //イベント開始
    //        _isLBbtnEvent = true;

    //        //ドリフトの説明終了
    //        _itemBoxExplanation = false;

    //        //セリフ終了
    //        inEventSerif = false;

    //        //インデックスリセット
    //        dialogueIndex = 0;

    //        //UIをリセット
    //        UIExplanationReset();

    //        //ここではメインのパネルも消す
    //        _mainPanel.SetActive(false);
    //        //サイドパネルをつける
    //        _sidePanel.SetActive(true);
    //        _itemBoxImage.SetActive(true);
    //    }
    //}

    private IEnumerator StartItemBoxExplanation()
    {
        foreach (string msg in _itemDialogueLines)
        {
            _commonText.text = msg;
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
            yield return new WaitForSeconds(interval); // interval秒待つ
        }

        //パネル起動音を再生
        _tutorialEventSound.PanelSound();
        //ここでUIを閉じたり、次のイベントに移る
        _commonText.text = "";

        //操作可能
        _eventStop = false;
        _StartPlayerStopEvent = true;

        //UIをリセット
        UIExplanationReset();

        //ここではメインのパネルも消す
        _mainPanel.SetActive(false);
        //サイドパネルをつける
        _sidePanel.SetActive(true);
    }
    #endregion

    #region ファイアの説明
    //private void StartFlameExplanation()
    //{
    //    dialogueIndex++;

    //    if (dialogueIndex < _flameDialogueLines.Length)
    //    {       
    //        _commonText.text = _flameDialogueLines[dialogueIndex];
    //    }
    //    else
    //    {
    //        //ここでUIを閉じたり、次のイベントに移る

    //        //UIをリセット
    //        UIExplanationReset();
    //        _LBbtnImage.SetActive(true);

    //        //イベント開始
    //        _isLBbtnEvent = true;

    //        //ファイアの説明終了
    //        _frameExplanation = false;

    //        //セリフ終了
    //        inEventSerif = false;

    //        //インデックスリセット
    //        dialogueIndex = 0;
    //    }
    //}

    private IEnumerator StartFlameExplanation()
    {
        foreach (string msg in _flameDialogueLines)
        {
            _commonText.text = msg;
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
            yield return new WaitForSeconds(interval); // interval秒待つ
        }

        //ここでUIを閉じたり、次のイベントに移る
        _commonText.text = "";
        //UIをリセット
        UIExplanationReset();
        _LBbtnImage.SetActive(true);

        //パネル起動音を再生
        _tutorialEventSound.PanelSound();

        yield return new WaitForSeconds(interval); // interval秒待つ

        //UIをリセット
        UIExplanationReset();
        _mainPanel.SetActive(false);

        //パネル起動音を再生
        _tutorialEventSound.PanelSound();

        //LBボタンのサイドパネルを出す
        _sidePanel.SetActive(true);
        _sideLBImage.SetActive(true);

        //イベント開始
        _isLBbtnEvent = true;
    }
    #endregion

    #region シールドの説明
    //private void StartShieldExplanation()
    //{
    //    dialogueIndex++;

    //    if (dialogueIndex == 2)
    //    {
    //        UIExplanationReset();
    //        _LTbtnImage.SetActive(true);
    //    }

    //    if (dialogueIndex < _shieldDialogueLines.Length)
    //    {         
    //        _commonText.text = _shieldDialogueLines[dialogueIndex];
    //    }
    //    else
    //    {
    //        //ここでUIを閉じたり、次のイベントに移る

    //        //イベント開始
    //        _isLTbtnEvent = true;

    //        //ドリフトの説明終了
    //        _shieldExplanation = false;

    //        //セリフ終了
    //        inEventSerif = false;

    //        //インデックスリセット
    //        dialogueIndex = 0;
    //    }
    //}

    private IEnumerator StartShieldExplanation()
    {
        foreach (string msg in _shieldDialogueLines)
        {
            _commonText.text = msg;
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
            yield return new WaitForSeconds(interval); // interval秒待つ
        }
        //ここでUIを閉じたり、次のイベントに移る        
        //パネル起動音を再生
        _tutorialEventSound.PanelSound();

        //UIをリセット
        UIExplanationReset();
        _commonText.text = "";
        _LTbtnImage.SetActive(true);

        yield return new WaitForSeconds(interval); // interval秒待つ

        //UIをリセット
        UIExplanationReset();
        _mainPanel.SetActive(false);

        //サイドパネルを出す
        _sidePanel.SetActive(true);
        _sideLTImage.SetActive(true);

        //イベント開始
        _isLTbtnEvent = true;
    }
    #endregion

    #region はねゲージの溜め方の説明
    //private void StartFretherExplanation()
    //{
    //    dialogueIndex++;

    //    if (dialogueIndex < _fretherDialogueLines.Length)
    //    {
    //        _commonText.text = _fretherDialogueLines[dialogueIndex];
    //    }
    //    else
    //    {
    //        //ここでUIを閉じたり、次のイベントに移る

    //        //操作可能
    //        _eventStop = false;
    //        _StartPlayerStopEvent = true;

    //        //ドリフトの説明終了
    //        _fretherExplanation = false;

    //        //セリフ終了
    //        inEventSerif = false;

    //        //インデックスリセット
    //        dialogueIndex = 0;

    //        //チャージエリアを展開
    //        _isCanSoarEvent = true;
    //    }
    //}

    private IEnumerator StartFretherExplanation()
    {
        foreach (string msg in _fretherDialogueLines)
        {
            _commonText.text = msg;
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
            yield return new WaitForSeconds(interval); // interval秒待つ
        }

        //パネル起動音を再生
        _tutorialEventSound.PanelSound();
        //ここでUIを閉じたり、次のイベントに移る

        //操作可能
        _eventStop = false;
        _StartPlayerStopEvent = true;

        //チャージエリアを展開
        _isCanSoarEvent = true;
    }
    #endregion

    #region ゲージがたまった時
    //private void StartGaugeFullExplanation()
    //{
    //    dialogueIndex++;

    //    if (dialogueIndex < _fretherGaugeDialogueLines.Length)
    //    {
    //        _commonText.text = _fretherGaugeDialogueLines[dialogueIndex];
    //    }
    //    else
    //    {
    //        //ここでUIを閉じたり、次のイベントに移る

    //        UIExplanationReset();

    //        _BbtnImage.SetActive(true);

    //        //イベント開始プレイヤーを止める
    //        _eventStop = false;

    //        //イベント開始
    //        _isBbtnEvent = true;

    //        //ドリフトの説明終了
    //        _soarExplanation = false;

    //        //セリフ終了
    //        inEventSerif = false;

    //        //インデックスリセット
    //        dialogueIndex = 0;
    //    }
    //}

    private IEnumerator StartGaugeFullExplanation()
    {
        foreach (string msg in _fretherGaugeDialogueLines)
        {
            _commonText.text = msg;
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
            yield return new WaitForSeconds(interval); // interval秒待つ
        }

        //パネル起動音を再生
        _tutorialEventSound.PanelSound();
        //ここでUIを閉じたり、次のイベントに移る
        _commonText.text = "";

        UIExplanationReset();

        _BbtnImage.SetActive(true);

        //イベント終了
        _eventStop = false;

        //イベント開始
        _isBbtnEvent = true;
    }
    #endregion

    #region ソアーの説明
    //private void StartSoarExplanation()
    //{
    //    dialogueIndex++;

    //    if (dialogueIndex < _soarDialogueLines.Length)
    //    {
    //        _commonText.text = _soarDialogueLines[dialogueIndex];
    //    }
    //    else
    //    {
    //        //ここでUIを閉じたり、次のイベントに移る

    //        //イベント終了
    //        _isBbtnEvent = false;

    //        //ドリフトの説明終了
    //        _soarInExplanation = false;

    //        //セリフ終了
    //        inEventSerif = false;

    //        //インデックスリセット
    //        dialogueIndex = 0;

    //        _tutorialComplete = true;
    //    }
    //}

    private IEnumerator StartSoarExplanation()
    {
        foreach (string msg in _soarDialogueLines)
        {
            _commonText.text = msg;
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
            yield return new WaitForSeconds(interval); // interval秒待つ
        }

        //パネル起動音を再生
        _tutorialEventSound.PanelSound();
        //ここでUIを閉じたり、次のイベントに移る
        _commonText.text = "";

        //操作可能
        _eventStop = false;

        //イベント終了
        //_isBbtnEvent = false;

        _tutorialComplete = true;
    }
    #endregion

    #region チュートリアル完了時、次の説明に移る時の説明
    //private void TutorialComplete()
    //{
    //    dialogueIndex++;

    //    if (dialogueIndex < _endDialogueLines.Length)
    //    {
    //        _commonText.text = _endDialogueLines[dialogueIndex];
    //    }
    //    else
    //    {
    //        //ここでUIを閉じたり、次のイベントに移る

    //        //操作可能
    //        _eventStop = false;

    //        //セリフ終了
    //        inEventSerif = false;

    //        //インデックスリセット
    //        dialogueIndex = 0;

    //        //終了したら繰り返し再生させない
    //        _isTutorialEndTrigger = false;

    //        //次のステージに行くように促す
    //        _nextScaneImage.SetActive(true);
    //    }
    //}

    private IEnumerator TutorialComplete()
    {
        foreach (string msg in _endDialogueLines)
        {
            _commonText.text = msg;
            //パネル起動音を再生
            _tutorialEventSound.PanelSound();
            yield return new WaitForSeconds(interval); // interval秒待つ
        }

        //パネル起動音を再生
        _tutorialEventSound.PanelSound();
        //ここでUIを閉じたり、次のイベントに移る

        //操作可能
        _eventStop = false;

        //終了したら繰り返し再生させない
        _isTutorialEndTrigger = false;

        //次のステージに行くように促す
        _nextScaneImage.SetActive(true);

        //スキップによって消えないように
        _tutorialCompleteTrigger = false;
    }
    #endregion

    #region プレイヤーが止まっている時に出すイベント
    /// <summary>
    /// プレイヤーが止まっていた時に出す表示を消すメソッド
    /// </summary>
    private void PlayerStop()
    {
        if(_playerMove._groundPath && !_playerMove._changePath)
        {
            // 速度がほぼゼロかを判定
            if (_rigidbody.velocity.magnitude < 0.1f)
            {
                stopTimer += Time.deltaTime;
                if (stopTimer >= 5f)
                {
                    //一定時間止まってた
                    _playerStopEvent = true;
                    //UIをリセット
                    UIExplanationReset();
                    _commonText.enabled = true;
                    _commonText.text = "まえにすすんでね";
                }
            }
            else
            {
                //一定時間以内に動く可能性があるのでIFの外側に
                stopTimer = 0f;
                if (_playerStopEvent)
                {
                    _playerStopEvent = false;
                    // 動いたらリセット

                    //UIをリセット
                    UIExplanationReset();
                }
            }
        }   
    }
    #endregion

    #region Aボタンで促すメソッド
    private void CommentFeed(bool isComment)
    {
        if (isComment)
        {
            _commentFeeds.SetActive(true);
            _isCommentFeedTimer += Time.deltaTime;
            if (_isCommentFeedTimer >= 0.8f)
            {
                if (_isCommentFeed)
                {
                    _commentFeedImage.SetActive(true);
                    _isCommentFeed = false;
                }
                else
                {
                    _commentFeedImage.SetActive(false);
                    _isCommentFeed = true;
                }
                _isCommentFeedTimer = 0f;
            }
        }
        else
        {
            _commentFeeds.SetActive(false);
        }
        
    }
    #endregion
}