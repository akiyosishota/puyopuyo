using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldArrayData : MonoBehaviour
{
    // タグリストの名前に紐づく番号
    ///<summary>
    ///フィールドのオブジェクトリスト
    /// 0 空欄
    /// 1 赤いぷよ
    /// 2 緑のぷよ
    /// 3 青いぷよ
    /// 4 黄色いぷよ
    ///</summary>
    private int _noBlock = 0;
    private int _redPuyo = 1;
    private int _greenPuyo = 2;
    private int _bluePuyo = 3;
    private int _yellowPuyo = 4;
    /// <summary>
    /// 「_Top」はぷよが生成されるポジション
    /// 「_minX」「_minY」「_maxX」「_maxY」で
    /// 動かすぷよの移動させる座標の上限値、下限値を設定
    /// 「_moveX」「_moveY」はConstを経て「_movePuyo」のポジションを変更する変数
    /// 「_deltaX」「_deltaY」はぷよを動かすための座標の加算、減算するための変数
    /// 「_rotate」は「_movePuy」を回転させるための変数
    /// </summary>
    private int _Top = 6;
    private int _minX = -3;
    private int _maxX = 3;
    private int _minY = -6;
    private int _maxY = 6;
    private int _deltaX = 0;
    private int _deltaY = 0;
    private int _moveX = 0;
    private int _moveY = 6;
    private int _rotate = 0;

    private bool _inputState = false;
    GameObject _movePuyo;


    ///<summary>
    ///動かすふたつのぷよの親になるPrefabを入れる
    ///</summary>
    [Header("動かすぷよの親のPrefabを挿入")]
    [SerializeField] private GameObject _movePuyoPrefab = null;
    [SerializeField] private GameState _gameState = GameState.START;
    public GameObject[] blocks;

    ///<summary>
    ///フィールドデータ用の変数を定義
    ///</summary>
    string[] _fieldObjectTagList =
    {
        "","RedPuyo","GreenPuyo","BluePuyo","YellowPuyo",
    };
    
    private int[,] _fieldDate =
    {
        {0,0,0,0,0,0,},
    };
    //縦横の最大値
    private int _horizontalMaxCount = 12;
    private int _verticalMaxCount = 6;

    private void Awake()
    {
        CleateBlock();
    }
    private void Update()
    {
        
        //ゲーム状態によって処理を分ける
        switch (_gameState)
        {
            case GameState.START:
                SetGameState(GameState.PLAYER);
                break;
            case GameState.PLAYER:
                _deltaX = 0;
                _deltaY = 0;
                if (Input.GetKeyDown(KeyCode.RightArrow) && !_inputState)
                {
                    _deltaX++;
                    print(_deltaX);
                    _inputState = true;
                }
                else if(Input.GetKeyDown(KeyCode.LeftArrow) && !_inputState)
                {
                    _deltaX--;
                    print(_deltaX);
                    _inputState = true;
                }
                if (Input.GetKeyDown(KeyCode.UpArrow) && !_inputState)
                {
                    _deltaY++;
                    print(_deltaY);
                    _inputState = true;
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow) && !_inputState)
                {
                    _deltaY--;
                    print(_deltaY);
                    _inputState = true;
                }
                if(Input.GetKeyDown(KeyCode.X))
                {
                    _movePuyo.transform.RotateAround(transform.position, new Vector3(0, 0, 1), _rotate + 90);
                    foreach(Transform child in transform)
                    {
                        child.transform.RotateAround(transform.position, new Vector3(0, 0, 1), _rotate - 90);
                    }
                    
                }
                if (Input.GetKeyDown(KeyCode.Y))
                {
                    _movePuyoPrefab.transform.RotateAround(transform.position, new Vector3(0, 0, 1), _rotate - 90);
                }
                _moveX = Mathf.Clamp(_moveX + _deltaX, _minX, _maxX);
                _moveY = Mathf.Clamp(_moveY + _deltaY, _minY, _maxY);
                MovePuyoPositionChange(_moveX, _moveY);
                _inputState = false;
                break;
        }
    }
    private enum GameState
    {
        START, PLAYER, END,
    }
    private void SetGameState(GameState gameState)
    {
        this._gameState = gameState;
    }
    private void MovePuyoPositionChange(int _ChangeX, int _ChangeY)
    {
        _movePuyo.transform.localPosition = new Vector3(_ChangeX, _ChangeY, 0f);
    }
    /// <summary>
    /// 操作するぷよの
    /// 「_headPuyo」=上のぷよ
    /// 「_tailPuyo」=下のぷよ
    /// 「_movePuyo」=操作する2つのぷよ
    /// </summary>
    private void CleateBlock()
    {
        _movePuyo = Instantiate(_movePuyoPrefab);
        _movePuyo.transform.position = new Vector2(0, _Top);
        GameObject _headPuyo = Instantiate(blocks[Random.Range(0, 4)]);
        _headPuyo.transform.position = new Vector2(0, _Top);
        _headPuyo.transform.SetParent(_movePuyo.transform, true);
        GameObject _tailPuyo = Instantiate(blocks[Random.Range(0, 4)]);
        _tailPuyo.transform.position = new Vector2(0, _Top-1);
        _tailPuyo.transform.SetParent(_movePuyo.transform, true);
    }
    
}
