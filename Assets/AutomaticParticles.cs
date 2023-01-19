using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NVIDIA.Flex;

namespace NVIDIA.Flex
{
    [RequireComponent(typeof(FlexSoftActor))]
    public class AutomaticParticles : MonoBehaviour
    {
        private int[] _idxCluster;
        private int[][] _idx;
        private int _indexCount;
        private GameObject[] _fixedCubes;
        private int index_offset;
        //private GameObject[] _fixedCubesTot;

        FlexSoftActor m_actor;
        public GameObject cubePrefab;

        //lines added for the particle interaction integration (also those with comment //!!)
        private int total_num_particles;
        public Transform agent_collider_T;
        //public Transform tail_collider_T;
        private float[] distances;
        private int[] indices;
        //public int[] collision_flags;
        public int collision_sum;
        public GameObject state;

        //private int[][] _totalIdx;
        private int counter = 0;

        //CUSTOM RAYCAST VARIABLES
        public Planner agent;
        public float[] ray_dist;

        //target displacement variables
        public Vector3 agentDisplacement = new Vector3(0f, 0f, 0f);
        public GameObject Target;
        public GameObject Start;
        public Vector3 targetDisplacement = new Vector3(0f, 0f, 0f);
        private Vector3 oldPos;
        private Vector3 newPos;
        private float minTargetDistance;
        private float curTargetDistance;
        private int closestPartIdx;

        #region Functions

        void Awake()
        {
            ray_dist = new float[16];
            for (int j = 0; j < 16; j++)
            {
                ray_dist[j] = 1f;
            }
        }

        void OnEnable()
        {
            // initialization actor
            m_actor = GetComponent<FlexSoftActor>();

            List<int> listOfidxCluster = new List<int>() { 0, 3, 4, 23, 24, 25, 71, 72, 73, 74, 76, 77, 78, 79, 81, 82, 86, 91, 93, 95, 96, 97, 98, 99, 101, 108, 110, 111, 120, 126, 129, 134, 136, 137, 145, 149, 151, 152, 154, 155, 156, 158, 162, 164, 166, 171, 172, 173, 176, 184, 185, 187, 189, 190, 191, 198, 199, 208, 210, 219, 222, 223, 229, 230, 232, 234, 236, 239, 240, 241, 242, 243, 246, 247, 248, 249, 256, 257, 258, 262, 263, 269, 270, 271, 272, 273, 275, 277, 281, 284, 290, 291, 292, 293, 297, 301, 305, 310, 311, 317, 318, 324, 325, 331, 335, 336, 337, 338, 345, 346, 350, 355, 362, 363, 366, 368, 370, 377, 378, 379, 382, 387, 389, 392, 393, 394, 404, 408, 409, 410, 412, 413, 414, 425, 435, 438, 440, 444, 448, 454, 455, 460, 461, 462, 465, 466, 468, 475, 483, 487, 489 };
            _idxCluster = listOfidxCluster.ToArray();


            if (m_actor)
            {
                m_actor.onFlexUpdate += OnFlexUpdate;
            }

        }

        void OnDisable()
        {
            if (m_actor)
            {
                m_actor.onFlexUpdate -= OnFlexUpdate;
                m_actor = null;
            }
        }

        void OnFlexUpdate(FlexContainer.ParticleData _particleData)
        {
            if (counter < 5)
            {
                index_offset = m_actor.indices[0];
                m_actor.asset.ClearFixedParticles();
                FlexExt.Instance instance = m_actor.handle.instance;

                total_num_particles = instance.numParticles;
                distances = new float[instance.numParticles];

                indices = new int[instance.numParticles];
                if (instance.numParticles > 0) FlexUtils.FastCopy(instance.particleIndices, indices);

                _idx = new int[_idxCluster.Length][];
                _indexCount = 0;
                for (int i = 0; i < _idxCluster.Length; i++)
                {
                    int j = _idxCluster[i];
                    int start = j == 0 ? 0 : m_actor.asset.shapeOffsets[j - 1];
                    int count = m_actor.asset.shapeOffsets[j] - start;
                    _idx[i] = new int[count];
                    for (int k = 0; k < count; ++k)
                    {
                        _idx[i][k] = indices[m_actor.asset.shapeIndices[start + k]];
                    }
                    // update the number of fixed particles
                    _indexCount += count;
                }

                _fixedCubes = new GameObject[_indexCount];
                int current_cube_idx = 0;
                for (int i = 0; i < _idx.Length; i++)
                {
                    for (int j = 0; j < _idx[i].Length; j++)
                    {
                        m_actor.asset.FixedParticle(_idx[i][j] - index_offset, true);
                        current_cube_idx += 1;
                    }
                }

                counter += 1;
            }




            // double check if the particles are fixed or not
            //Debug.Log("The number of fixed particles: " + m_actor.asset.fixedParticles.Length.ToString());
            //Debug.Log("The number of cubes: " + _fixedCubes.Length.ToString());

            //!!
            float threshold = 1.15f; //0.001f too small
            float raycast_th = 0.8f; //1.0
            collision_sum = 0;

            //raycast vector
            Vector3[] raycast_vec = agent.raycast;

            for (int j = 0; j < 16; j++)
            {
                ray_dist[j] = 1f;
            }

            for (int i = 0; i < total_num_particles; i++)
            {
                Vector3 particle_position = (Vector3)_particleData.GetParticle(indices[i]);
                if (state.activeSelf == true)
                {
                    distances[i] = Vector3.Distance(particle_position, agent_collider_T.position);
                    //double tail_distance = Vector3.Distance(particle_position, tail_collider_T.position);
                    if (distances[i] < threshold)
                    {
                        //collision_flags[i] = 1;
                        collision_sum += 1;
                    }
                    /*if(tail_distance < threshold){
                        collision_sum +=1;
                    }*/
                }
                for (int j = 0; j < 16; j++)
                {
                    float rayj_dist = Vector3.Distance(particle_position, raycast_vec[j]);
                    if (rayj_dist < raycast_th)
                    {
                        ray_dist[j] = rayj_dist;
                    }
                }

                if (Start.activeSelf == true)
                {
                    if (i == 407)
                    {
                        minTargetDistance = Vector3.Distance(particle_position, Target.transform.position);
                        oldPos = particle_position;
                    }
                    /*curTargetDistance = Vector3.Distance(particle_position, Target.transform.position);
                    newPos = particle_position;
                    if (Mathf.Abs(curTargetDistance) < Mathf.Abs(minTargetDistance))
                    {
                        oldPos = newPos;
                        minTargetDistance = curTargetDistance;
                        closestPartIdx = i;
                        //Debug.Log("position " + newPos + " distance " + minTargetDistance);
                    }*/
                }

            }
            //Debug.Log(collision_sum);
            //distances[0] = Vector3.Distance((Vector3)_particleData.GetParticle(0), cube_collider_T.position);

            if (Start.activeSelf == false)
            {
                //Vector3 closestPartPos = (Vector3)_particleData.GetParticle(indices[closestPartIdx]);
                Vector3 closestPartPos = (Vector3)_particleData.GetParticle(indices[407]); //random particle not fixed
                float displacementDistance = Vector3.Distance(closestPartPos, Target.transform.position);
                targetDisplacement = new Vector3(0f, 0f, 0f);
                //Debug.Log(displacementDistance);
                /*if (displacementDistance == minTargetDistance)
                {

                }
                else if (displacementDistance > minTargetDistance)
                {*/
                    targetDisplacement += oldPos - closestPartPos;
                    oldPos = closestPartPos;
                    minTargetDistance = displacementDistance;
                    //Debug.Log(targetDisplacement);
                /*}
                else if (displacementDistance < minTargetDistance)
                {
                    targetDisplacement += closestPartPos - oldPos;
                    oldPos = closestPartPos;
                    minTargetDistance = displacementDistance;
                    //Debug.Log("target should be moving");
                }*/
            }
        }


        #endregion
    }
}