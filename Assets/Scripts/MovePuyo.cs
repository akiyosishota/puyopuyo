using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePuyo : MonoBehaviour
{
    private int _moveX = 0;
    private int _moveY = 0;
    private int _minX = -3;
    private int _maxX = 3;
    private int _minY = -6;
    private int _maxY = 6;
    private bool _inputState = false;
    private Transform _thisTransform = null;

    [SerializeField] private GameState _gameState = GameState.START;
    /// <summary>
    /// 「_minX」「_minY」「_maxX」「_maxY」で
    /// 動かすぷよの移動させる座標の上限値、下限値を設定
    /// </summary>
    private void Start()
    {
        //このオブジェクトのTransformコンポーネント取得
        Transform _thisTransform = this.GetComponent<Transform>();
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
                if (Input.GetKeyDown(KeyCode.LeftArrow) && !_inputState)
                {
                    
                }
                //Transform _thisTransform = 
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
}
