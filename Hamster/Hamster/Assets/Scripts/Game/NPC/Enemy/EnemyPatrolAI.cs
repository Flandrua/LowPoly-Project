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
    public float raycastDistance = 10f; // ���߼��ľ���
    public GameObject rayStart;
    public Vector3 rayPosOffset;
    public Vector3 rayRoateOffset;
    //��AI�ڹ̶��ĵ�λѲ�ߣ����������ң���ȥ׷
    //��ұ�Wall tag���嵲ס��AI��ȥѲ�ߣ��������ĵ㣬���Ҵ�һ�����ߣ�����м�û���ϰ���ʹ��Ǹ��㿪ʼ����Ѳ��
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
        //��Ѳ�ߵ�ʱ�������߼�������ĵ�һ��box���������ң�isPatrol=false;
        //��chase��ʱ�������Һ�AI֮����wall������ڣ��ж���ʧ��Ұ��������ʵĵ�λ��isPatrol=true����ȥѲ��
        Ray ray = new Ray(rayStart.transform.position + rayPosOffset, transform.forward + rayRoateOffset);
        // �ڳ����л�������
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
            //������ң���Ҫ��raycast�򵽵�������㣬���λ�ã������walltag������ȵ���ҵľ���̣��ж�Ϊ��Ұ��ʧ����ȥѲ��
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
                    isPatrol = true;//��ȥѲ��
                    //�õ�һ������ĵ�λ
                    List<GameObject> listGo = patrolPoints;
                    while (listGo.Count > 0)//update�ڵ��ã�ע����������
                    {
                        GameObject target = ClosePoint(listGo);
                        //��������λ֮�����ڵ������б���Ѱ��������λ
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
                        Debug.LogWarning("����û���ҵ�������еĵ�λ");


                }
            }
        }
    }


    private GameObject CheckBlocked(GameObject target)
    {
        // ����ӽ�ɫ��Ŀ��ķ���
        Vector3 direction = target.transform.position - transform.position;
        float distance = Vector3.Distance(target.transform.position, transform.position);
        // ��������
        Ray ray = new Ray(transform.position, direction);
        // ���ڴ洢��һ����ײ��Ϣ
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance))
        {
            float blockDistance = Vector3.Distance(hit.collider.transform.position, transform.position);
            if (blockDistance < distance)
            {
                Debug.Log($"patrol point is blocked by {hit.collider.name}");
                return hit.collider.gameObject;//�м����ڵ���
            }
        }
        return target;//�м����ڵ���
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
        // ���㵱ǰ��ɫ��ǰ�������Ŀ�귽��֮��ļн�
        float angle = Vector3.SignedAngle(transform.forward, directionToTarget, Vector3.up);
        if (angle < -5f || angle > 5f)
            RotateSelf(angle);
    }
}
