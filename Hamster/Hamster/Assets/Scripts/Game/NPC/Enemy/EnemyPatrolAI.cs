using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class EnemyPatrol : MonoBehaviour
{
    // Start is called before the first frame update
    public List<GameObject> patrolPoints;
    [Header("------debug------")]
    [SerializeField] private int pointIndex;
    [SerializeField] private GameObject curTargerPoint;
    [SerializeField] private bool isPatrol = true;
    [SerializeField] private bool canMove = true;


    [Header("------ debug ------")] 
    public float speed = 1.0f;
    private Rigidbody rb;
    public float rotateSpeed = 1.0f;
    public float raycastDistance = 10f; // 射线检测的距离
    public GameObject rayStart;
    public Vector3 rayPosOffset;
    public Vector3 rayRoateOffset;
    //让AI在固定的点位巡逻，如果看到玩家，就去追
    //玩家被Wall tag物体挡住后，AI回去巡逻，检测最近的点，并且打一条射线，如果中间没有障碍物，就从那个点开始重新巡逻
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pointIndex = 0;
        curTargerPoint = patrolPoints[pointIndex];
    }

    // Update is called once per frame
    void Update()
    {

        RayDectecter();
        PatrolCheck();
        FaceToTarget(curTargerPoint);
    }
    private void LateUpdate()
    {
        if (canMove)
            MoveForward();
    }
    private void MoveForward()
    {
        Vector3 direction = curTargerPoint.transform.position - transform.position;
        Vector3 velocity = direction.normalized * speed;
        velocity.y = 0;
        rb.velocity = velocity;
    }
    private void PatrolCheck()
    {
        if (isPatrol)
        {
            float shortestDistance = Vector3.Distance(curTargerPoint.transform.position, transform.position);
            //Debug.Log(shortestDistance);
            if (shortestDistance < 0.4f)
            {
                pointIndex++;
                if (pointIndex > patrolPoints.Count - 1) pointIndex = 0;
                curTargerPoint = patrolPoints[pointIndex];
            }
        }
    }

    private void RayDectecter()
    {
        //在巡逻的时候，用射线检测碰到的第一个box，如果是玩家，isPatrol=false;
        //在chase的时候，如果玩家和AI之间有wall物体存在，判定丢失视野，计算合适的点位后isPatrol=true，回去巡逻
        Ray ray = new Ray(rayStart.transform.position + rayPosOffset, transform.forward + rayRoateOffset);
        // 在场景中绘制射线
        Debug.DrawRay(ray.origin, ray.direction * raycastDistance, Color.red);

        if (isPatrol)
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                GameObject go = hit.collider.gameObject;
                //Debug.Log(go.name);
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    curTargerPoint = go;
                    isPatrol = false;
                }
            }
        }
        else
        {
            //发现玩家，需要对raycast打到的物体计算，相对位置，如果有walltag的物体比到玩家的距离短，判断为视野丢失，回去巡逻
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {
                RaycastHit closestHit = hits[0];
                float shortestDistance = Vector3.Distance(closestHit.point, transform.position);
                foreach (RaycastHit hit in hits)
                {
                    float distanceToCharacter = Vector3.Distance(hit.point, transform.position);

                    if (distanceToCharacter < shortestDistance)
                    {
                        shortestDistance = distanceToCharacter;
                        closestHit = hit;
                    }
                }
                if (closestHit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
                {
                    isPatrol = true;//回去巡逻
                    //得到一个最近的点位
                    List<GameObject> listGo = patrolPoints;
                    while (listGo.Count > 0)//update内调用，注意死机问题
                    {
                        GameObject target = ClosePoint(listGo);
                        //如果最近点位之间有遮挡，从列表里寻找其他点位
                        GameObject blocker = CheckBlocked(target);
                        if (blocker != target)
                        {
                            listGo.Remove(target);
                        }
                        else
                        {
                            pointIndex = patrolPoints.IndexOf(target);
                            curTargerPoint = target;
                            break;
                        }
                    }
                    if (listGo.Count == 0)
                        Debug.LogWarning("可能没有找到最近可行的点位");


                }
            }
        }
    }


    private GameObject CheckBlocked(GameObject target)
    {
        // 计算从角色到目标的方向
        Vector3 direction = target.transform.position - transform.position;
        float distance = Vector3.Distance(target.transform.position, transform.position);
        // 创建射线
        Ray ray = new Ray(transform.position, direction);
        // 用于存储第一个碰撞信息
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            float blockDistance = Vector3.Distance(hit.collider.transform.position, transform.position);
            if (blockDistance < distance)
            {
                Debug.Log($"patrol point is blocked by {hit.collider.name}");
                return hit.collider.gameObject;//中间有遮挡物
            }
        }
        return target;//中间无遮挡物
    }
    private GameObject ClosePoint(List<GameObject> listGo)
    {
        GameObject closestPoint = null;
        float shortestDistance = float.MaxValue;

        foreach (GameObject point in listGo)
        {
            float distanceToCharacter = Vector3.Distance(point.transform.position, transform.position);

            if (distanceToCharacter < shortestDistance)
            {
                shortestDistance = distanceToCharacter;
                closestPoint = point;
            }
        }
        return closestPoint;
    }


    public void AddInputMovement(Vector3 mm)
    {
        var v = rb.velocity;
        v.z = -mm.z;
        rb.velocity = v * speed;
    }

    public void RotateSelf(float roate)
    {
        transform.Rotate(0, Time.deltaTime * roate * rotateSpeed, 0);
    }

    public void FaceToTarget(GameObject target)
    {
        Vector3 directionToTarget = target.transform.position - transform.position;
        // 计算当前角色的前方方向和目标方向之间的夹角
        float angle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        if (angle < -5f || angle > 5f)
            RotateSelf(angle);
    }
}
