//  works for FTL
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using System.IO;
using UnityEngine.InputSystem;

public class RigidPlanner : Agent
{

    [SerializeField] private Transform targetTransform;
    private Vector3 startingTargetPos;
    //[SerializeField] private GameObject verticalPlane;
    public GameObject PhantomTransform;
    public GameObject Plane;
    private string PathFilename = "C:/Users/ATLASM2/Desktop/ChiaraFiles/Chiara Cath/Assets/centerline.txt";
    private string PathFilename1 = "C:/Users/ATLASM2/Desktop/ChiaraFiles/Chiara Cath/Assets/OutPath/old1/ep0.txt";
    ArrayList traj_pos = new ArrayList();
    ArrayList traj_rot = new ArrayList();
    private List<Vector3> trajectory_pos;
    private List<Quaternion> trajectory_rot;
    //long model
    //private string PathFilename = "C:/Users/ATLASM2/Desktop/ChiaraFiles/Chiara Cath/Assets/long model/centerline_09_11_2022 15_59_44.txt";
    private int centerlineNumber = 102;
    //private int centerlineNumber = 302;
    ArrayList planned_path = new ArrayList();
    public GameObject centerlinePointPrefab;
    private List<GameObject> centerline;
    private GameObject centerlinePoint;
    private string[] text_divided;
    private Vector3 relative_position;
    private GameObject point;
    public GameObject tail;


    PlayerControls controls;
    Vector2 move_xy;
    Vector2 move_z;
    Vector2 rotate;

    int total_steps;
    public GameObject state;
    public Snake snake;

    private int time_offset = 25000;
    private float lesson = 240f;


    public CatheterCollision collisionCath;

    //////////////////////////////////////////////////
    //UNCOMMENT FOR WRITING FILE
    private StreamWriter sw;
    private StreamWriter sw1;
    private StreamWriter sw2;
    private StreamWriter sw3;
    private StreamWriter sw4;
    private StreamWriter sw5;
    private StreamWriter sw6;
    private string sw_path;
    int buffer = 0;
    int buffer1 = 0;
    private string OutputPath;
    private int episodeCounter = 0;
    //////////////////////////////////////////////////
    private float target_error;
    private int collision_amount;

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

        ////////////////////////////
        sw_path = "Assets/OutPath/ppo_only/";
        sw3 = new StreamWriter(sw_path + "target_error.txt"); //this is not episodic, has to be one for all the test
        sw5 = new StreamWriter(sw_path + "distance.txt");
        sw6 = new StreamWriter(sw_path + "collision_amount.txt");
        /////////////////////////////////*/

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

        //read trajectory
        StreamReader reader1 = new StreamReader(PathFilename1);
        string text1 = reader1.ReadToEnd();
        reader1.Close();

        //convert string to float array
        string[] text_divided1 = text1.Split(separators, System.StringSplitOptions.None);
        Vector3 tmp1 = new Vector3(0f, 0f, 0f);
        Quaternion tmp2 = new Quaternion(0f, 0f, 0f, 0f);
        //trajectory = new List<Transform>();
        for (int i = 0; i < 330 * 7; i += 7)
        {
            tmp1.x = float.Parse(text_divided1[i]);
            tmp1.y = float.Parse(text_divided1[i + 1]);
            tmp1.z = float.Parse(text_divided1[i + 2]);

            tmp2.x = float.Parse(text_divided1[i + 3]);
            tmp2.y = float.Parse(text_divided1[i + 4]);
            tmp2.z = float.Parse(text_divided1[i + 5]);
            tmp2.w = float.Parse(text_divided1[i + 6]);
            //Debug.Log(tmp);
            //tmp1 = tmp1 + relative_position;
            traj_pos.Add(tmp1);
            traj_rot.Add(tmp2);
        }

    }


    public override void OnEpisodeBegin()
    {
        //transform.localPosition = new Vector3(20.4f,-446.2f,-62.9f);
        //transform.localPosition = new Vector3(19.1805973f, -492.16925f, -70.1118317f);
        //transform.rotation = Quaternion.identity;

        float randomStart = Random.Range(0f, 1f);
        if (randomStart <= 0.35f)
        {
            transform.localPosition = new Vector3(26.3999996f, -423.700012f, -82.4000015f);// * (1f - 0.01f * randomStart);
            transform.rotation = Quaternion.identity;
            transform.Rotate(new Vector3(357.199036f, 359.862061f, 5.50155973f));
        }
        else if (randomStart > 0.35f && randomStart < 0.75f)
        {
            transform.localPosition = new Vector3(24.7000008f, -458.5f, -70.1118317f);// * (1f - 0.01f * randomStart);
            transform.rotation = Quaternion.identity;
        }
        else if (randomStart > 0.75f)
        {
            transform.localPosition = new Vector3(21.5f, -476f, -70.1118317f);// * (1f - 0.01f * randomStart);
            transform.rotation = Quaternion.identity;
        }

        //start around trajectory
        /*int randomPos = Mathf.RoundToInt(randomStart * 330);
        Vector3 subtract = new Vector3(randomStart*1.5f, randomStart * 1.5f, randomStart * 1.5f);
        transform.localPosition = (Vector3)traj_pos[randomPos] - subtract;
        transform.rotation = (Quaternion)traj_rot[randomPos];*/

        //long model
        //transform.localPosition = new Vector3(9.10000038f, -496.269012f, -127f);
        //transform.rotation = Quaternion.identity;

        //transform.Rotate(-90.0f,0.0f,0.0f);
        tail.transform.localPosition = transform.localPosition;
        tail.transform.rotation = Quaternion.identity;


        double randomGoal = Random.Range(0f, 1f);
        if (randomGoal <= 0.2f)
        {
            //targetTransform.localPosition = new Vector3(-32.5999985f,-456.600006f,-106.411827f);
            targetTransform.localPosition = new Vector3(-43f, -454f, -117.5f);// * (1f - 0.1f * randomStart);
            startingTargetPos = new Vector3(-43f, -454f, -117.5f);
        }
        else if (randomGoal > 0.2f && randomGoal <= 0.4f)
        {
            //targetTransform.localPosition = new Vector3(-33.7999992f,-458f,-117f);
            targetTransform.localPosition = new Vector3(-45.4000015f, -440.399994f, -101.900002f);// * (1f - 0.1f * randomStart);
            startingTargetPos = new Vector3(-45.4000015f, -440.399994f, -101.900002f);
        }
        else if (randomGoal > 0.4f && randomGoal <= 0.6f)
        {
            //targetTransform.localPosition = new Vector3(-45.7000008f,-436.700012f,-105.900002f);
            targetTransform.localPosition = new Vector3(-32f, -473.700012f, -101.900002f);// * (1f - 0.1f * randomStart);
            startingTargetPos = new Vector3(-32f, -473.700012f, -101.900002f);
        }
        else if (randomGoal > 0.6f && randomGoal <= 0.8f)
        {
            //targetTransform.localPosition = new Vector3(-36f,-454.700012f,-105.900002f);*/
            targetTransform.localPosition = new Vector3(-43.2000008f, -450.299988f, -112.900002f);// * (1f - 0.1f * randomStart);
            startingTargetPos = new Vector3(-43.2000008f, -450.299988f, -112.900002f);

        }
        else if (randomGoal > 0.8f)
        {
            //targetTansform.localPosition = new Vector3(-32.2999992f,-453.600006f,-102.699997f);
            targetTransform.localPosition = new Vector3(-43.1f, -450.299988f, -109.300003f);// * (1f - 0.1f * randomStart);
            startingTargetPos = new Vector3(-43.1f, -450.299988f, -109.300003f);
        }

        for (int i = 0; i < centerlineNumber; i++)
        {
            GameObject point = centerline[i];
            if (point.activeSelf == false)
            {
                point.SetActive(true);
            }
        }
        //start.SetActive(true);
        //////////////////////////////////////////////////
        //UNCOMMENT FOR WRITING FILE
        sw = new StreamWriter(sw_path + "ep" + episodeCounter + ".txt");
        sw1 = new StreamWriter(sw_path + "speed" + episodeCounter + ".txt");
        sw2 = new StreamWriter(sw_path + "position" + episodeCounter + ".txt");
        sw4 = new StreamWriter(sw_path + "target_position_err" + episodeCounter + ".txt");
        
        buffer = 0;
        /////////////////////////////////////////*/
        target_error = Vector3.Distance(transform.position, targetTransform.position);
        collision_amount = 0;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation);
        //sensor.AddObservation(targetTransform.localPosition);
        Vector3 direction = (targetTransform.position - transform.position).normalized;
        float normalizedDistance = Vector3.Distance(transform.position, targetTransform.position) / 50f;
        float distance = normalizedDistance * 50f;
        if (distance < target_error)
        {
            target_error = distance;
        }
        sensor.AddObservation(direction);
        sensor.AddObservation(normalizedDistance);

    }

    void FixedUpdate()
    {
        AddReward(-0.00001f);

        if (collisionCath.collisionValue == 1)
        {
            collision_amount += 1;
            //AddReward(-0.001f);
            Debug.Log("aorta collision body");
            //EndEpisode();
            //state.SetActive(false);
            collisionCath.collisionValue = 0;
        }

        //////////////////////////////////////////////////
        //UNCOMMENT FOR WRITING FILE
        buffer += 1;
        buffer1 += 1;
        if(buffer == 50){
            Vector3 pos = this.transform.localPosition;
            string position = pos[0].ToString("F3") + " " + pos[1].ToString("F3") + " " + pos[2].ToString("F3") + " ";
            Quaternion quat = this.transform.localRotation;
            string quaternion = quat[0].ToString("F3") + " " + quat[1].ToString("F3") + " " + quat[2].ToString("F3") + " " + quat[3].ToString("F3");
            sw.WriteLine(position + quaternion);
            sw2.WriteLine(position);
            sw4.WriteLine(Vector3.Distance(targetTransform.position,startingTargetPos));
            buffer = 0;
        }
        //////////////////////////////////////////////*/

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        // read joystick cmds:
        float rotateX = actions.ContinuousActions[0]; // right stick x 
        float rotateZ = actions.ContinuousActions[1]; // right stick y
        //float rotateY = actions.ContinuousActions[2]; // left stick x
        float moveY = Mathf.Abs(actions.ContinuousActions[2]); // D-pad y

        float rotateY = 0f;
        float moveSpeed = 5f; // 3cm per second; in general: unit/second, the units can be mm, cm ...
        //float moveSpeed = 5f;
        float rotateYAngle = 0f;// let's say for a Time.deltaTime, it can rotate 5 degrees maximal.

        float delta_l = Time.deltaTime * Mathf.Abs(moveY) * moveSpeed; //time*speed = space
        //float delta_l = Time.fixedDeltaTime * Mathf.Abs(moveY) * moveSpeed;
        //float delta_l = Time.fixedDeltaTime * 1f ; //fixed insertion speed

        //float bend_max = 280f; //24
        float bend_max = Academy.Instance.EnvironmentParameters.GetWithDefault("pos", lesson);
        float tip_l = 50f;
        // compute rotate_max
        float rotate_max = bend_max * delta_l / tip_l;
        float rotateXZAngle = rotate_max;


        /*if(Vector3.Distance(transform.localPosition,tail.transform.localPosition)<3f){
            transform.Rotate(new Vector3(-rotateX*rotateXZAngle , -rotateY*rotateYAngle,-rotateZ*rotateXZAngle));
        }
        else{
            if(snake.centralAngle < 55f || snake.centralAngle > 125f){*/
        transform.Rotate(new Vector3(-rotateX * rotateXZAngle, rotateY * rotateYAngle, -rotateZ * rotateXZAngle));
        if (Mathf.Abs(rotateX) > 0.5f || Mathf.Abs(rotateZ) > 0.5f)
        {
            AddReward(+0.00001f);
        }

        //transform.position += moveY * transform.forward * delta_l; //raycast needs z to be the moving direction
        transform.localPosition += transform.up * delta_l;
        string insertion = moveY.ToString("F3");
        string steer = rotateX.ToString("F3") + " " + rotateZ.ToString("F3");

        //////////////////////////////////////////
        /*if(buffer1 == 50){
            sw1.WriteLine(insertion);
            buffer1 = 0;
        }
        ///////////////////////////////////////*/
        //sw2.WriteLine(steer);
        //transform.position += transform.up * delta_l;

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
            AddReward(+1f);
            total_steps = this.StepCount;
            Debug.Log("total steps " + total_steps);
            state.SetActive(false);
            //////////////////////////////////////////////////
            //UNCOMMENT FOR WRITING FILE
            sw.Close();
            sw1.Close();
            sw2.Close();
            sw4.Close();
            float finalDist = Vector3.Distance(transform.position, targetTransform.position);
            string finalDistance = finalDist.ToString("F3");
            sw3.WriteLine(target_error);
            sw3.Flush();
            sw5.WriteLine(this.StepCount);
            sw5.Flush();
            sw6.WriteLine(collision_amount);
            sw6.Flush();
            episodeCounter += 1;
            ///////////////////////////////////////////////

            //Debug.Log(targetTransform.position);
            EndEpisode();
        }
        if (collision.collider.name == "Centerline(Clone)")
        {
            AddReward(+0.005f);
            GameObject centerlineCollision = collision.collider.gameObject;
            centerlineCollision.SetActive(false);
            //Debug.Log(Vector3.Distance(transform.position, centerlineCollision.transform.position));
        }

        if (collision.collider.tag == "Plane")
        {
            AddReward(-1f);
            state.SetActive(false);
            //////////////////////////////////////////////////
            //UNCOMMENT FOR WRITING FILE
            //episodeCounter +=1;
            /*sw.Close();
            sw1.Close();
            sw2.Close();*/
            //////////////////////////////////////////////*/
            EndEpisode();
        }
        if (collision.collider.name == "Aorta")
        {
            AddReward(-1f);
            Debug.Log("aorta collision");
            state.SetActive(false);
            //////////////////////////////////////////////////
            //UNCOMMENT FOR WRITING FILE
            //episodeCounter +=1;
            sw.Close();
            sw1.Close();
            sw2.Close();
            sw4.Close();
            float finalDist = Vector3.Distance(transform.position, targetTransform.position);
            string finalDistance = finalDist.ToString("F3");
            sw3.WriteLine(target_error);
            sw3.Flush();
            sw5.WriteLine(this.StepCount);
            sw5.Flush();
            sw6.WriteLine(collision_amount);
            sw6.Flush();
            episodeCounter += 1;
            /////////////////////////////////////////////////*/
            EndEpisode();
        }


    }


}
