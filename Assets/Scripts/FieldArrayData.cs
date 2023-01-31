using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class FieldArrayData : MonoBehaviour
{
    /// <summary>
    /// 「_movePuyoY」「_movePuyoX」はぷよが生成されるポジション
    /// 「_nextFirstX」「_nextFirstY」はネクストのぷよが生成されるポジション
    /// 「_nextSecondX」「_nextSecondY」は２つ目のネクストのぷよが生成されるポジション
    /// 「_outX」「_outY」は×マークの位置（ゲームオーバー）
    /// 「_maxX」「_maxY」は配列の最大値
    /// </summary>
    private const float _movePuyoY = 12;
    private const int _movePuyoX = 2;
    private const int _nextFirstX = 7;
    private const int _nextFirstY = 10;
    private const int _nextSecondX = 8;
    private const float _nextSecondY = 7.5f;
    private const int _outX = 2;
    private const int _outY = 11;
    private const int _maxX = 6;
    private const int _maxY = 13;
    /// <summary>
    /// 「_fieldPuyoData」は置かれているぷよぷよの配列
    /// 「puyos」は4色のぷよを格納する
    /// </summary>
    public GameObject[,] _fieldPuyoData;
    public GameObject[] puyos;

    GameObject _movePuyo;
    GameObject _nextPuyoFirst;
    GameObject _nextPuyoSecond;

    ///<summary>
    ///動かすふたつのぷよの親になるPrefabを入れる
    ///ゲームオーバーになった時に表示させるUIを入れる
    ///</summary>
    [Header("動かすぷよの親のPrefabを挿入")]
    [SerializeField] private GameObject _movePuyoPrefab = null;
    [SerializeField] private GameObject _textUi = default;
    private Text _gameOver = default;
    List<GameObject> checkZumiFieldBlocks = new List<GameObject>();

    private void Awake()
    {
        StartCleatePuyo();
        _gameOver = _textUi.GetComponent<Text>();
        _fieldPuyoData = new GameObject[_maxX, _maxY ];
    }
    private void Update()
    {
        //「R」キーを押されたらやりなおし
        if( Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene("PuyoPuyo");
        }
    }
    // ぷよが置かれた＆消されたときに落下させる
    public void Drop()
    {
        //「nullCount」は何マス分落下させるかの変数
        int nullCount = 0;
        for (int x = 0; x < _maxX; x++)
        {
            for (int y = 0; y < _maxY; y++)
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

        //同色4つ以上つながっていたら消す
        //連鎖が終了すると再度ぷよを生成
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

    //同色4つ以上つながっていたら消す
    bool RenketuAri()
    {
        for (int x = 0; x < _maxX; x++)
        {
            for (int y = 0; y < _maxY; y++)
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
        for (int x = 0; x < _maxX; x++)
        {
            for (int y = 0; y < _maxY; y++)
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

    //それぞれの色の連結数を調べる
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
    /// ゲームがスタートしたら
    /// 動かす「_movePuyo」
    /// ネクストの「_nextPuyoFirst」
    /// 2つめのネクストの「_nextPuyoSecond」
    /// を生成
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

    /// <summary>
    /// ネクストのぷよを動かすぷよへ取り出す
    /// 2つめのネクストのぷよをネクストへ取り出す
    /// 2つめのネクストのぷよを新しく生成する
    /// </summary>
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
