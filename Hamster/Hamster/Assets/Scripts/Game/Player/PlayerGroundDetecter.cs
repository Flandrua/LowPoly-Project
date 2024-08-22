using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerGroundDetecter : MonoBehaviour
{
    public bool isGrounded;
    public List<GameObject> toIgnores;
    public PlayerJump jump;
    List<GameObject> _currentGrounds = new List<GameObject>();
    public PlayerHealthBehaviour health;

    public Vector3 colsPosOffset;
    public float radius;

    List<Collider> _cols;

    private void Awake()
    {
        health = GetComponentInParent<PlayerHealthBehaviour>();
        _cols = new List<Collider>();
    }
    private void Update()
    {

    }

    private void FixedUpdate()
    {
        Vector3 colsPos = transform.position + colsPosOffset;
        var cols = Physics.OverlapSphere(colsPos, radius);
        Debug.DrawLine(colsPos + Vector3.up * radius, colsPos - Vector3.up * radius, Color.red); // 绘制一条红色的线
        Debug.DrawLine(colsPos + Vector3.left * radius, colsPos - Vector3.left * radius, Color.red); // 绘制一条红色的线
        Debug.DrawLine(colsPos + Vector3.forward * radius, colsPos - Vector3.forward * radius, Color.red); // 绘制一条红色的线
        foreach (var col in cols)
        {
            if (toIgnores.Contains(col.gameObject))
                continue;

            if (!_cols.Contains(col))
            {
                Debug.Log(col.gameObject);
                OnEnter(col);
                _cols.Add(col);
            }
        }
        for (var i = _cols.Count - 1; i >= 0; i--)
        {
            var col = _cols[i];
            if (col == null)
            {
                _cols.Remove(col);
                continue;
            }

            if (!cols.Contains(col))
            {
                OnExit(col);
                _cols.Remove(col);
            }
        }
        
    }
    private void OnEnter(Collider col)
    {
        //if (col.tag == "Kill")
        //{
        //    health.Die(true);
        //    return;
        //}

        var colEnemy = col.gameObject.GetComponent<EnemyBehaviour>();
        //head kick
        if (col.isTrigger)
            return;
        //Debug.Log(collision.contacts.Length);
        if (!_currentGrounds.Contains(col.gameObject))
            _currentGrounds.Add(col.gameObject);
        isGrounded = _currentGrounds.Count > 0;
        PlayerBehaviour.Instance.SetBool("onGround", isGrounded);
        jump.OnGrounded();
    }

    private void OnExit(Collider col)
    {
        //Debug.Log("OnCollisionExit2D " + collision.gameObject);
        if (_currentGrounds.Contains(col.gameObject))
            _currentGrounds.Remove(col.gameObject);
        isGrounded = _currentGrounds.Count > 0;
        if (!isGrounded)
        {
            PlayerBehaviour.Instance.SetBool("onGround", false);
        }
    }


}
