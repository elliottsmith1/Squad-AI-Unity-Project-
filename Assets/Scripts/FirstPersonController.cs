using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [RequireComponent(typeof (CharacterController))]
    [RequireComponent(typeof (AudioSource))]
    public class FirstPersonController : MonoBehaviour
    {
        [SerializeField] private bool m_IsWalking;
        [SerializeField] private float m_WalkSpeed;
        [SerializeField] private float m_RunSpeed;
        [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
        [SerializeField] private float m_JumpSpeed;
        [SerializeField] private float m_StickToGroundForce;
        [SerializeField] private float m_GravityMultiplier;
        [SerializeField] private MouseLook m_MouseLook;
        [SerializeField] private bool m_UseFovKick;
        [SerializeField] private FOVKick m_FovKick = new FOVKick();
        [SerializeField] private bool m_UseHeadBob;
        [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
        [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();
        [SerializeField] private float m_StepInterval;
        [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
        [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
        [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

        private Camera m_Camera;

        [SerializeField] GameObject allyPrefab;

        public List<GameObject> allies = new List<GameObject>();

        public GameObject selectedAlly;

        public GameObject selectedCover = null;

        private GameObject allyLookedAt;

        private bool allSelected = false;

		private bool followMode = false;

		private bool canSpawn = false;

		private int maxAllies = 6;

		[SerializeField] Text spawnTextRef;
		[SerializeField] Text coverTextRef;

		private Score score_ref;

        private GameObject pointer;

		//movement controls 
        private bool m_Jump;
        private float m_YRotation;
        private Vector2 m_Input;
        private Vector3 m_MoveDir = Vector3.zero;
        private CharacterController m_CharacterController;
        private CollisionFlags m_CollisionFlags;
        private bool m_PreviouslyGrounded;
        private Vector3 m_OriginalCameraPosition;
        private float m_StepCycle;
        private float m_NextStep;
        private bool m_Jumping;
        private AudioSource m_AudioSource;
        private float selectionTimer = 0.0f;
        private float selectionTimerLeft = 0.0f;

        // Use this for initialization
        private void Start()
        {
            m_CharacterController = GetComponent<CharacterController>();
            m_Camera = Camera.main;
            m_OriginalCameraPosition = m_Camera.transform.localPosition;
            m_FovKick.Setup(m_Camera);
            m_HeadBob.Setup(m_Camera, m_StepInterval);
            m_StepCycle = 0f;
            m_NextStep = m_StepCycle/2f;
            m_Jumping = false;
            m_AudioSource = GetComponent<AudioSource>();
			m_MouseLook.Init(transform , m_Camera.transform);

            foreach (GameObject ally in GameObject.FindGameObjectsWithTag("Ally"))
            {
                allies.Add(ally);
            }

            pointer = GameObject.FindGameObjectWithTag("Pointer");

			score_ref = GetComponent<Score> ();
        }


        // Update is called once per frame
        private void Update()
        {
            allies.RemoveAll(item => item == null);

            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                StartCoroutine(m_JumpBob.DoBobCycle());
                PlayLandingSound();
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;

            LookingAt();

            CheckKeys();

            CheckMouse();
        }


        private void PlayLandingSound()
        {
            m_AudioSource.clip = m_LandSound;
            //m_AudioSource.Play();
            m_NextStep = m_StepCycle + .5f;
        }

        private void AlliesSelected()
        {
            foreach (GameObject ally in allies)
            {
                ally.GetComponentInChildren<Light>().enabled = true;              
            }
        }

        private void AlliesDeselected()
        {
            allSelected = false;

            foreach (GameObject ally in allies)
            {
                ally.GetComponentInChildren<Light>().enabled = false;
            }
        }

        private void CheckMouse()
        {
            if (Input.GetMouseButton(1))
            {
                selectionTimer += Time.deltaTime;

                if (selectionTimer > 1)
                {                    
                    selectedAlly = null;
                    allSelected = true;

					coverTextRef.enabled = false;

                    AlliesSelected();
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                selectionTimer = 0.0f;
            }

            if (Input.GetMouseButtonDown(1))
            {
                RaycastHit hit;

				if (Physics.SphereCast(transform.position, 2, transform.forward, out hit))
                {
                    if (hit.transform.tag == "Ally")
                    {
                        AlliesDeselected();

                        selectedAlly = hit.transform.gameObject;

						coverTextRef.enabled = true;

                        selectedAlly.GetComponentInChildren<Light>().enabled = true;
                    }
                }
            }

            if (Input.GetMouseButton(0))
            {
                selectionTimerLeft += Time.deltaTime;

                if (selectionTimerLeft > 1)
                {
                    if (selectedAlly)
                    {
						if (score_ref.GetScore() >= 250)
						{
							if (pointer.GetComponent<pointerMovement> ().GetEmptySpace ()) 
							{
								score_ref.UpdateScore (-250);

								selectedAlly.GetComponent<AllyBehaviour> ().newPosition (pointer.transform.position);

								selectedAlly.GetComponent<AllyBehaviour> ().state = AllyState.BUILDING;

								selectionTimerLeft = -1000;
							}
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                selectionTimerLeft = 0.0f;
            }

            if (Input.GetMouseButtonDown(0))
            {
                if ((!selectedAlly) && (!allSelected))
                {
                    return;
                }

                if (selectedCover)
                {
                    if (allSelected)
                    {
                        foreach (GameObject ally in allies)
                        {
                            ally.GetComponent<Fighter>().NoEnemy();

                            ally.GetComponent<AllyBehaviour>().MoveToCover(selectedCover);
                        }
                    }

                    else if (selectedAlly)
                    {
                        selectedAlly.GetComponent<Fighter>().NoEnemy();

                        selectedAlly.GetComponent<AllyBehaviour>().MoveToCover(selectedCover);
                    }
                }

                else
                {
                    if (allSelected)
                    {
                        foreach (GameObject ally in allies)
                        {
                            if (!ally.GetComponent<AllyBehaviour>().movingToCover)
                            {
								if (ally.GetComponent<AllyBehaviour> ().state != AllyState.BUILDING) 
								{
									ally.GetComponent<Fighter> ().NoEnemy ();

									ally.GetComponent<AllyBehaviour> ().state = AllyState.MOVING;

									ally.GetComponent<AllyBehaviour> ().newPosition (pointer.transform.position);
								}
                            }
                        }
                    }

                    else if (selectedAlly)
                    {
                        if (!selectedAlly.GetComponent<AllyBehaviour>().movingToCover) 
                        {
							if (selectedAlly.GetComponent<AllyBehaviour> ().state != AllyState.BUILDING) 
							{
								selectedAlly.GetComponent<Fighter> ().NoEnemy ();

								selectedAlly.GetComponent<AllyBehaviour> ().state = AllyState.MOVING;

								selectedAlly.GetComponent<AllyBehaviour> ().newPosition (pointer.transform.position);
							}
                        }
                    }
                }
            }
        }

        private void CheckKeys()
        {
			if (canSpawn) 
			{
				if (allies.Count < maxAllies) 
				{
					if (CrossPlatformInputManager.GetButtonDown ("Point")) 
					{					
						if (score_ref.GetScore () >= 100) 
						{
							score_ref.UpdateScore (-100);

							Vector3 spawnPos = transform.position;

							spawnPos.x += Random.Range (-10, 10);
							spawnPos.z += Random.Range (-10, 10);

							GameObject obj = Instantiate (allyPrefab, spawnPos, Quaternion.identity);

							foreach (GameObject ally in allies) {
								ally.GetComponent<AllyBehaviour> ().NewAlly (obj);
							}

							allies.Add (obj);    
						}
					}
				} 

				else 
				{
					spawnTextRef.enabled = false;
				}
			}

            if (Input.GetButtonDown("Follow"))
            {
				if (followMode) 
				{
					foreach (GameObject ally in allies) 
					{
						ally.GetComponent<AllyBehaviour> ().following = false;
					}
				} 

				else 
				{
					foreach (GameObject ally in allies) 
					{
						ally.GetComponent<AllyBehaviour> ().following = false;
					}
				}
            }

        }

        private void LookingAt()
        {
            RaycastHit hit2;

			if (Physics.SphereCast(transform.position, 2, transform.forward, out hit2))
            {
                if (hit2.transform.tag == "Cover")
                {
                    if ((hit2.transform.gameObject != selectedCover) || (selectedCover == null))
                    {
                        if (selectedCover)
                        {
                            selectedCover.transform.Find("CoverCircle").gameObject.GetComponent<Renderer>().enabled = false;
                        }

                        selectedCover = hit2.transform.gameObject;
                        selectedCover.transform.Find("CoverCircle").gameObject.GetComponent<Renderer>().enabled = true;
                    }
                }

                else if (hit2.transform.tag == "Ally")
                {
                    if ((hit2.transform.gameObject != allyLookedAt) || (selectedCover == null))
                    {
                        if (allyLookedAt)
                        {
                            allyLookedAt.transform.Find("AllyCircle").gameObject.GetComponent<Renderer>().enabled = false;
                        }

                        allyLookedAt = hit2.transform.gameObject;
                        allyLookedAt.transform.Find("AllyCircle").gameObject.GetComponent<Renderer>().enabled = true;
                    }
                }
            }
        }

        private void FixedUpdate()
        {
            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x*speed;
            m_MoveDir.z = desiredMove.z*speed;


            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);

            m_MouseLook.UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            //m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }
            // pick & play a random footstep sound from the array,
            // excluding sound at index 0
            int n = Random.Range(1, m_FootstepSounds.Length);
            m_AudioSource.clip = m_FootstepSounds[n];
           // m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            m_FootstepSounds[n] = m_FootstepSounds[0];
            m_FootstepSounds[0] = m_AudioSource.clip;
        }


        private void UpdateCameraPosition(float speed)
        {
            Vector3 newCameraPosition;
            if (!m_UseHeadBob)
            {
                return;
            }
            if (m_CharacterController.velocity.magnitude > 0 && m_CharacterController.isGrounded)
            {
                m_Camera.transform.localPosition =
                    m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                                      (speed*(m_IsWalking ? 1f : m_RunstepLenghten)));
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
            }
            else
            {
                newCameraPosition = m_Camera.transform.localPosition;
                newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
            }
            m_Camera.transform.localPosition = newCameraPosition;
        }


        private void GetInput(out float speed)
        {
            // Read input
            float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            float vertical = CrossPlatformInputManager.GetAxis("Vertical");

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }

            // handle speed change to give an fov kick
            // only if the player is going to a run, is running and the fovkick is to be used
            if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0)
            {
                StopAllCoroutines();
                StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            }
        }


        private void RotateView()
        {
            m_MouseLook.LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }

		public void SetCanSpawn(bool _spawn)
		{
			canSpawn = _spawn;

			if (canSpawn) 
			{
				if (allies.Count < maxAllies) 
				{
					spawnTextRef.enabled = true;
				}
			} 

			else 
			{
				spawnTextRef.enabled = false;
			}
		}
    }
}
