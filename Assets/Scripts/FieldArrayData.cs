using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FieldArrayData : MonoBehaviour
{
    /// <summary>
    /// 「_movePuyoY」はぷよが生成されるポジション
    /// 「_minX」「_minY」「_maxX」「_maxY」で
    /// 動かすぷよの移動させる座標の上限値、下限値を設定
    /// 「_moveX」「_moveY」はConstを経て「_movePuyo」のポジションを変更する変数
    /// 「_deltaX」「_deltaY」はぷよを動かすための座標の加算、減算するための変数
    /// 「_rotate」は「_movePuyo」を回転させ、回転による移動の制限をつけるための変数
    /// 「_puyoFallTime」は経過時間ごとにぷよを自動で落下させるための時間変数
    /// </summary>
    private float _movePuyoY = 12;
    private int _movePuyoX = 2;
    private int _maxX = 6;
    private int _maxY = 13;
    private int _outX = 2;
    private int _outY = 11;
    private int _nextFirstX = 7;
    private int _nextFirstY = 10;
    private int _nextSecondX = 8;
    private float _nextSecondY = 7.5f;

    public GameObject[,] _fieldPuyoData;
    // 縦横の最大数
    const int FIELD_SIZE_X = 6;
    const int FIELD_SIZE_Y = 13;

    GameObject _movePuyo;
    GameObject _nextPuyoFirst;
    GameObject _nextPuyoSecond;

    private Text _gameOver = default;
    ///<summary>
    ///動かすふたつのぷよの親になるPrefabを入れる
    ///</summary>
    [Header("動かすぷよの親のPrefabを挿入")]
    [SerializeField] private GameObject _movePuyoPrefab = null;
    [SerializeField] private GameObject _textUi = default;
    public GameObject[] puyos;
    List<GameObject> checkZumiFieldBlocks = new List<GameObject>();

    private void Awake()
    {
        StartCleatePuyo();
        _gameOver = _textUi.GetComponent<Text>();
        _fieldPuyoData = new GameObject[_maxX, _maxY ];
    }
    private void Update()
    {
        if( Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("PuyoPuyo");
        }
    }
    void Hairetu()
    {
        for(int x = 0; x < 6; x++)
        {
            for(int y=0; y < 10; y++)
            {
                GameObject piece = Instantiate(puyos[Random.Range(0, 4)]);
                piece.transform.position = new Vector3(x, y, 0);
                _fieldPuyoData[x, y] = piece;
            }
        }
    }

    public void Drop()
    {
        int nullCount = 0;
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 13; y++)
            {
                if(_fieldPuyoData[x, y] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    _fieldPuyoData[x, y].transform.position += new Vector3(0, -nullCount, 0);
                    _fieldPuyoData[x, y -nullCount] = _fieldPuyoData[x, y];
                    _fieldPuyoData[x, y] = null;
                }
            }
            nullCount = 0;
        }

        if (RenketuAri())
        {
            StartCoroutine(Erase());
        }
        if (!RenketuAri())
        {
            if (_fieldPuyoData[_outX, _outY] == null)
            {
                CleatePuyo();
            }
            else
            {
                _gameOver.enabled = true;
            }
        }
    }

    bool RenketuAri()
    {
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 13; y++)
            {
                checkZumiFieldBlocks.Clear();
                if (Renketusuu(x, y, 0) >= 4 && _fieldPuyoData[x, y] != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

        IEnumerator Erase()
    {
        yield return new WaitForSeconds(0.5f);
        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 13; y++)
            {
                checkZumiFieldBlocks.Clear();
                if(Renketusuu(x, y, 0) >= 4 && _fieldPuyoData[x, y] != null)
                {
                    Destroy(_fieldPuyoData[x, y]);
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        Drop();
    }

    int Renketusuu(int x,int y, int renketusuu)
    {
        if(_fieldPuyoData[x, y] == null || checkZumiFieldBlocks.Contains(_fieldPuyoData[x, y]))
        {
            return renketusuu;
        }
        checkZumiFieldBlocks.Add(_fieldPuyoData[x, y]);
        renketusuu++;
        if(x != _maxX - 1 && _fieldPuyoData[x + 1, y] != null && _fieldPuyoData[x, y].name == _fieldPuyoData[x + 1, y].name)
        {
            renketusuu = Renketusuu(x + 1, y, renketusuu);
        }
        if (x != 0 && _fieldPuyoData[x - 1, y] != null && _fieldPuyoData[x, y].name == _fieldPuyoData[x - 1, y].name)
        {
            renketusuu = Renketusuu(x - 1, y, renketusuu);
        }
        if (y != 0 && _fieldPuyoData[x, y - 1] != null && _fieldPuyoData[x, y].name == _fieldPuyoData[x, y - 1].name)
        {
            renketusuu = Renketusuu(x, y - 1, renketusuu);
        }
        if (y != _maxY - 1 && _fieldPuyoData[x, y + 1] != null && _fieldPuyoData[x, y].name == _fieldPuyoData[x, y + 1].name)
        {
            renketusuu = Renketusuu(x, y + 1, renketusuu);
        }
        return renketusuu;
    }
    /// <summary>
    /// 操作するぷよの
    /// 「_headPuyo」=上のぷよ
    /// 「_tailPuyo」=下のぷよ
    /// 「_movePuyo」=操作する2つのぷよ
    /// をGameObject「Stage」の子として生成
    /// </summary>
    public void StartCleatePuyo()
    {
        _movePuyo=Instantiate(_movePuyoPrefab);
        _movePuyo.transform.position = new Vector2(_movePuyoX, _movePuyoY);
        GameObject _headPuyo = Instantiate(puyos[Random.Range(0, 4)]);
        _headPuyo.transform.position = new Vector2(_movePuyoX, _movePuyoY);
        _headPuyo.transform.SetParent(_movePuyo.transform, true);
        GameObject _tailPuyo = Instantiate(puyos[Random.Range(0, 4)]);
        _tailPuyo.transform.position = new Vector2(_movePuyoX, _movePuyoY - 1);
        _tailPuyo.transform.SetParent(_movePuyo.transform, true);

        _nextPuyoFirst = Instantiate(_movePuyoPrefab);
        _nextPuyoFirst.GetComponent<MovePuyo>().enabled = false;
        _nextPuyoFirst.transform.position = new Vector2(_nextFirstX, _nextFirstY);
        GameObject _nextPuyoFirstHead = Instantiate(puyos[Random.Range(0, 4)]);
        _nextPuyoFirstHead.transform.position = new Vector2(_nextFirstX, _nextFirstY);
        _nextPuyoFirstHead.transform.SetParent(_nextPuyoFirst.transform, true);
        GameObject _nextPuyoFirstTail = Instantiate(puyos[Random.Range(0, 4)]);
        _nextPuyoFirstTail.transform.position = new Vector2(_nextFirstX, _nextFirstY - 1);
        _nextPuyoFirstTail.transform.SetParent(_nextPuyoFirst.transform, true);

        _nextPuyoSecond = Instantiate(_movePuyoPrefab);
        _nextPuyoSecond.GetComponent<MovePuyo>().enabled = false;
        _nextPuyoSecond.transform.position = new Vector2(_nextSecondX, _nextSecondY);
        GameObject _nextPuyoSecondHead = Instantiate(puyos[Random.Range(0, 4)]);
        _nextPuyoSecondHead.transform.position = new Vector2(_nextSecondX, _nextSecondY);
        _nextPuyoSecondHead.transform.SetParent(_nextPuyoSecond.transform, true);
        GameObject _nextPuyoSecondTail = Instantiate(puyos[Random.Range(0, 4)]);
        _nextPuyoSecondTail.transform.position = new Vector2(_nextSecondX, _nextSecondY - 1);
        _nextPuyoSecondTail.transform.SetParent(_nextPuyoSecond.transform, true);
    }
    public void CleatePuyo()
    {
        _movePuyo = _nextPuyoFirst;
        _movePuyo.transform.position = new Vector2(_movePuyoX, _movePuyoY);
        _nextPuyoFirst.GetComponent<MovePuyo>().enabled = true;
    
        _nextPuyoFirst = _nextPuyoSecond;
        _nextPuyoFirst.transform.position = new Vector2(_nextFirstX, _nextFirstY);
    
        _nextPuyoSecond = Instantiate(_movePuyoPrefab);
        _nextPuyoSecond.GetComponent<MovePuyo>().enabled = false;
        _nextPuyoSecond.transform.position = new Vector2(_nextSecondX, _nextSecondY);
        GameObject _nextPuyoSecondHead = Instantiate(puyos[Random.Range(0, 4)]);
        _nextPuyoSecondHead.transform.position = new Vector2(_nextSecondX, _nextSecondY);
        _nextPuyoSecondHead.transform.SetParent(_nextPuyoSecond.transform, true);
        GameObject _nextPuyoSecondTail = Instantiate(puyos[Random.Range(0, 4)]);
        _nextPuyoSecondTail.transform.position = new Vector2(_nextSecondX, _nextSecondY - 1);
        _nextPuyoSecondTail.transform.SetParent(_nextPuyoSecond.transform, true);
    }
}
