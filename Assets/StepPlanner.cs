//  works for FTL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.IO;
using UnityEngine.InputSystem;


public class StepPlanner : Agent
{

    [SerializeField] private Transform targetTransform;
    //[SerializeField] private GameObject verticalPlane;
    public GameObject PhantomTransform;
    public GameObject Plane;
    private string PathFilename = "C:/Users/ATLASM2/Desktop/ChiaraFiles/Chiara Cath/Assets/centerline.txt";
    private int centerlineNumber = 102;
    ArrayList planned_path = new ArrayList();
    public GameObject centerlinePointPrefab;
    private List<GameObject> centerline;
    private GameObject centerlinePoint;
    private string[] text_divided;
    private Vector3 relative_position;
    private GameObject point;
    public GameObject tail;
    public GameObject targetRegion;


    PlayerControls controls;
    Vector2 move_xy;
    Vector2 move_z;
    Vector2 rotate;

    private float moveY;

    int total_steps;
    public GameObject state;
    public Snake snake;

    public NVIDIA.Flex.Particles particles;
    public GameObject start;

    private int time_offset = 15000;

    #region funcs for setting ps4 controller
    void Awake()
    {
        controls = new PlayerControls();

        controls.Gameplay.Move_xy.performed += ctx => move_xy = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move_xy.canceled += ctx => move_xy = Vector2.zero;

        //left stick
        controls.Gameplay.Move_z.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        controls.Gameplay.Move_z.canceled += ctx => rotate = Vector2.zero;

        //controls.Gameplay.Rotate.performed += ctx => move_z = ctx.ReadValue<Vector2>();
        //controls.Gameplay.Rotate.canceled += ctx => move_z = Vector2.zero;

        controls.Gameplay.Enable();
    }

    #endregion

    void Start()
    {
        Vector3 relative_position = PhantomTransform.transform.position;

        //Read the text and create string
        StreamReader reader = new StreamReader(PathFilename);
        string text = reader.ReadToEnd();
        reader.Close();

        //convert string to float array
        string[] separators = { " ", "\n" };
        string[] text_divided = text.Split(separators, System.StringSplitOptions.None);
        Vector3 tmp = new Vector3(0f, 0f, 0f);
        centerline = new List<GameObject>();
        for (int i = 0; i < centerlineNumber * 3; i += 3)
        {
            tmp.x = float.Parse(text_divided[i]);
            tmp.z = float.Parse(text_divided[i + 1]);
            tmp.y = float.Parse(text_divided[i + 2]);
            tmp = tmp + relative_position;
            //Debug.Log(tmp);
            planned_path.Add(tmp);
            GameObject centerlinePoint = Instantiate(centerlinePointPrefab, tmp, Quaternion.identity) as GameObject;
            centerlinePoint.SetActive(true);
            //Debug.Log(centerlinePoint);
            centerline.Add(centerlinePoint);
        }

    }


    public override void OnEpisodeBegin()
    {
        //transform.localPosition = new Vector3(20.4f, -446.2f, -62.9f);
        transform.localPosition = new Vector3(19.1806f, -492.1692f, -70.111824f);
        transform.rotation = Quaternion.identity;
        //transform.Rotate(-90.0f,0.0f,0.0f);
        tail.transform.localPosition = transform.localPosition;
        tail.transform.rotation = Quaternion.identity;
        //tail.transform.Rotate(-90.0f,0.0f,0.0f);


        //targetTransform.localPosition = new Vector3(Random.Range(-19.6f,-34.7f),Random.Range(-410f,-420f),Random.Range(-93.5f,-106.4f));
        targetRegion.transform.localPosition = targetTransform.localPosition;
        //targetTransform.localPosition = new Vector3(18.6000004f,-353.399994f,-77f); //TRY THIS
        //targetTransform.localPosition = new Vector3(23.3999996f,-374.600006f,-72.9000015f); //NEARGOAL11
        //targetTransform.localPosition = new Vector3(12.8999996f,-352.799988f,-84.5999985f); //NEARGOAL2_2
        //targetTransform.localPosition = new Vector3(-2.4000001f,-349.200012f,-93.0999985f); //NEARGOAL3
        //targetTransform.localPosition = new Vector3(-24.7000008f, -357.5f, -103.599998f); //NEARGOAL4
        //targetTransform.localPosition = new Vector3(-36.5f,-381f,-103.599998f); //NEARGOAL5
        //targetTransform.localPosition = new Vector3(-36.2000008f, -388.200012f, -103.599998f);
        //targetTransform.localPosition = new Vector3(-23.8f,-415.1f,-91.5f);
        //targetTransform.localPosition = new Vector3(36.4193993f, -438.269012f, 110.811821f); //AORTA RF

        //targetTransform.localPosition = new Vector3(-1.79999995f, -395f, -107.099998f); //AORTA RF STEP1
        targetTransform.localPosition = new Vector3(-36.4193993f, -419.200012f, -105.800003f); //step2 RF
        //targetTransform.localPosition = new Vector3(-40.5f, -441.299988f, -107.099998f);

        for (int i = 0; i < centerlineNumber; i++)
        {
            GameObject point = centerline[i];
            if (point.activeSelf == false)
            {
                point.SetActive(true);
            }
        }
        start.SetActive(true);
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        sensor.AddObservation(targetTransform.localPosition);

    }

    void Update()
    {

        /*if (this.StepCount < Academy.Instance.EnvironmentParameters.GetWithDefault("time",time_offset)){
             AddReward(-0.0001f);
         }
         else{
            AddReward(-0.05f);
        }*/
        if (this.StepCount > 10)
        {
            start.SetActive(false);
        }
        
        AddReward(-0.0001f);
        targetTransform.localPosition += particles.targetDisplacement;
        particles.targetDisplacement = new Vector3(0f, 0f, 0f);
        targetRegion.transform.localPosition = targetTransform.localPosition;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {

        // read joystick cmds:
        float rotateX = actions.ContinuousActions[0]; // right stick x 
        float rotateZ = actions.ContinuousActions[1]; // right stick y
        //float rotateY = actions.ContinuousActions[2]; // left stick x
        moveY = Mathf.Abs(actions.ContinuousActions[2]); // D-pad y

        float rotateY = 0f;
        float moveSpeed = 5f; // 3cm per second; in general: unit/second, the units can be mm, cm ...
        float rotateYAngle = 0f;// let's say for a Time.deltaTime, it can rotate 5 degrees maximal.

        float delta_l = Time.deltaTime * Mathf.Abs(moveY) * moveSpeed;
        //float delta_l = Time.deltaTime * Mathf.Abs(moveY) * rotationSpeed ;

        float bend_max = 20f;
        float tip_l = 7.5f; // 75mm, written it in [m]
        // compute rotate_max
        float rotate_max = bend_max * delta_l / tip_l;
        float rotateXZAngle = rotate_max;

        
        if (Vector3.Distance(transform.localPosition, tail.transform.localPosition) < 3f)
        {
            transform.Rotate(new Vector3(-rotateX * rotateXZAngle, -rotateY * rotateYAngle, rotateZ * rotateXZAngle));
        }
        else
        {
            if (snake.centralAngle < 65f || snake.centralAngle > 115f)
            {
                transform.Rotate(new Vector3(-rotateX * rotateXZAngle, rotateY * rotateYAngle, -rotateZ * rotateXZAngle));
            }
            else { Debug.Log("bending exceeded"); }
        }
        //transform.position += moveY * transform.forward * delta_l; //raycast needs z to be the moving direction
        transform.position += moveY * transform.up * delta_l;

    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;

        // ps4 for the snake robot, to control the moving direction actually in x-y-z
        continuousActions[0] = move_xy.y; //[-1, 1]
        continuousActions[1] = move_xy.x;
        //continuousActions[2] = move_z.y;
        continuousActions[2] = rotate.y;
        //continuousActions[4] = rotate.y;

        // use rotation+insertion
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Target")
        {
            SetReward(+1f);
            total_steps = this.StepCount;
            EndEpisode();
            Debug.Log(total_steps);
            state.SetActive(false);
        }
        if (collision.collider.tag == "Centerline")
        {
            AddReward(+0.005f);
            GameObject centerlineCollision = collision.collider.gameObject;
            centerlineCollision.SetActive(false);
            //Debug.Log(Vector3.Distance(transform.position, centerlineCollision.transform.position));
        }

        if (collision.collider.name == "Wall")
        {
            SetReward(-1f);
            EndEpisode();
            state.SetActive(false);
        }
        if (collision.collider.name == "Aorta")
        {
            SetReward(-1f);
            Debug.Log("aorta collision");
            EndEpisode();
            state.SetActive(false);
        }

    }


}
