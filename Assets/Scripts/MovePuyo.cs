using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePuyo : MonoBehaviour
{
    /// <summary>
    /// 「_minX」「_maxX」はX座標の最小値、最大値
    /// 「_minY」はY座標の最小値
    /// 「_puyoFallTime」は自動落下させるためのタイム変数
    /// </summary>
    private int _minX = 0;
    private int _maxX = 6;
    private float _minY = 0;
    private float _puyoFallTime = 0;
    private void Update()
    {
        ///<summary>
        ///一秒経過するごとに自動的に1マス分落下する
        ///</summary>
        _puyoFallTime += Time.deltaTime;
        if (_puyoFallTime > 1)
        {
            this.gameObject.transform.position += new Vector3(0, -1, 0);
            if (!CanMove())
            {
                this.gameObject.transform.position += new Vector3(0, 1, 0);
                LandingPuyo();
                this.gameObject.transform.DetachChildren();
                FindObjectOfType<FieldArrayData>().Drop();
                Destroy(this.gameObject, 10f);
                this.enabled = false;
            }
            _puyoFallTime = 0;
        }
        //「→」キーを押すと右に移動する
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.gameObject.transform.position += new Vector3(1, 0, 0);
            if (!CanMove())
            {
                this.gameObject.transform.position += new Vector3(-1, 0, 0);
            }            
        }
        //「←」キーを押すと左に移動する
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.gameObject.transform.position += new Vector3(-1, 0, 0);
            if (!CanMove())
            {
                this.gameObject.transform.position += new Vector3(1, 0, 0);
            }
        }
        //「↓」キーを押すと下に移動する
        //移動した際は自動落下の秒数をリセット
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            this.gameObject.transform.position += new Vector3(0, -1, 0);
            if (!CanMove())
            {
                this.gameObject.transform.position += new Vector3(0, 1, 0);
            }
            else
            {
                _puyoFallTime = 0;
            }
        }
        //「z」キーを押すと左方向に回転する
        if (Input.GetKeyDown(KeyCode.Z))
        {
            this.gameObject.transform.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
            foreach (Transform childPuyo in transform)
            {
                childPuyo.transform.RotateAround(childPuyo.transform.position, new Vector3(0, 0, 1), -90);
            }
            if (!CanMove())
            {
                this.gameObject.transform.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
                foreach (Transform childPuyo in transform)
                {
                    childPuyo.transform.RotateAround(childPuyo.transform.position, new Vector3(0, 0, 1), 90);
                }
            }
        }
        //「x」キーを押すと右方向に回転する
        if (Input.GetKeyDown(KeyCode.X))
        {
            this.gameObject.transform.RotateAround(transform.position, new Vector3(0, 0, 1), -90);
            foreach (Transform childPuyo in transform)
            {
                childPuyo.transform.RotateAround(childPuyo.transform.position, new Vector3(0, 0, 1), 90);
            }
            if (!CanMove())
            {
                this.gameObject.transform.RotateAround(transform.position, new Vector3(0, 0, 1), 90);
                foreach (Transform childPuyo in transform)
                {
                    childPuyo.transform.RotateAround(childPuyo.transform.position, new Vector3(0, 0, 1), -90);
                }
            }
        }
    }

    //壁側にいるときや床面にいるときの一部条件で回転できない＆移動できないようにする
    //また置かれているぷよに対しても回転＆移動できないようにする
    bool CanMove()
    {
        foreach (Transform childPuyo in transform)
        {
            int _childX = Mathf.RoundToInt(childPuyo.transform.position.x);
            int _childY = Mathf.RoundToInt(childPuyo.transform.position.y);

            if ( _childX < _minX || _childX >= _maxX || _childY < _minY)
            {
                return false;               
            }
            if(FindObjectOfType<FieldArrayData>()._fieldPuyoData[_childX, _childY] != null)
            {
                return false;
            }
        }
        return true;
    }
    //ぷよが置かれたときに配列の中に格納する
    public void LandingPuyo()
    {
        foreach(Transform ChildPuyo in transform)
        {
            int _childX = Mathf.RoundToInt(ChildPuyo.transform.position.x);
            int _childY = Mathf.RoundToInt(ChildPuyo.transform.position.y);
            FindObjectOfType<FieldArrayData>()._fieldPuyoData[_childX, _childY] = ChildPuyo.gameObject;
        }
        
    }
}
