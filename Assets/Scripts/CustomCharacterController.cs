using ECM.Common;
using ECM.Controllers;
using UnityEngine;
using Rewired;
using TMPro;
using Doozy.Engine;
using Doozy.Engine.UI;

public class CustomCharacterController : BaseCharacterController
{
    #region EDITOR EXPOSED FIELDS
        
        private FallingFloorAgent agentScript;
        private Player player;
        [HideInInspector] public Rigidbody myRigidbody;
        private Animator myAnimator;
        [HideInInspector] public Transform myCamera;
        [HideInInspector] public ThirdPersonCamera tPersonCam;
        private PlayerData myData;


        [Header("Extras")]
        [SerializeField] private int playerID;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI scoreText;
        public Transform myHead;
        [SerializeField] private GameObject confetties;

        [Header("Custom Controller")]
        [Tooltip("The character's walk speed.")]
        [SerializeField]
        private float _walkSpeed = 2.5f;

        [Tooltip("The character's run speed.")]
        [SerializeField]
        private float _runSpeed = 5.0f;

        #endregion

        #region PROPERTIES

        public PlayerData Data { set { myData = value; } }

        public int Rank { get { return myData.RuntimeRank; } }

        public string Gamertag { get { return myData.RuntimeGamertag; } }

        public float Score
        {
            get { return myData.RuntimeScore; }
            set { myData.RuntimeScore = Mathf.Clamp(value, 0f, 1000.00f); }
        }
        
        private bool _jumpPressed = false;
        public bool JumpPressed
        {
            set { _jumpPressed = value; }
            get 
            {
                if(_jumpPressed){
                    _jumpPressed = false;
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// The character's walk speed.
        /// </summary>

        public float walkSpeed
        {
            get { return _walkSpeed; }
            set { _walkSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// The character's run speed.
        /// </summary>

        public float runSpeed
        {
            get { return _runSpeed; }
            set { _runSpeed = Mathf.Max(0.0f, value); }
        }

        /// <summary>
        /// Walk input command.
        /// </summary>

        public bool walk { get; private set; }

        #endregion

        #region METHODS

        /// <summary>
        /// Get target speed based on character state (eg: running, walking, etc).
        /// </summary>

        private float GetTargetSpeed()
        {
            return walk ? walkSpeed : runSpeed;
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' CalcDesiredVelocity method to handle different speeds,
        /// eg: running, walking, etc.
        /// </summary>

        protected override Vector3 CalcDesiredVelocity()
        {
            // Set 'BaseCharacterController' speed property based on this character state

            speed = GetTargetSpeed();

            // Return desired velocity vector

            return base.CalcDesiredVelocity();
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' Animate method.
        /// 
        /// This shows how to handle your characters' animation states using the Animate method.
        /// The use of this method is optional, for example you can use a separate script to manage your
        /// animations completely separate of movement controller.
        /// 
        /// </summary>

        protected override void Animate()
        {
            // If no animator, return

            if (animator == null)
                return;

            // Compute move vector in local space

            var move = transform.InverseTransformDirection(moveDirection);

            // Update the animator parameters

            var forwardAmount = animator.applyRootMotion
                ? move.z
                : Mathf.InverseLerp(0.0f, runSpeed, movement.forwardSpeed);

            animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
            animator.SetFloat("Turn", Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);

            animator.SetBool("OnGround", movement.isGrounded);

            if (!movement.isGrounded)
                animator.SetFloat("Jump", movement.velocity.y, 0.1f, Time.deltaTime);
        }

        /// <summary>
        /// Overrides 'BaseCharacterController' HandleInput,
        /// to perform custom controller input.
        /// </summary>

        protected override void HandleInput()
        {
            // Handle your custom input here...

            if(GameManager.Instance.gameState.Equals(GameManager.GameState.GameOver)) { return; }

            moveDirection = new Vector3
            {
                x = 0.0f,
                y = 0.0f,
                z = agentScript == null ? (player.GetButton("Move Vertical") || isJumping ? 1 : 0) : (agentScript.vertical)
            };

            jump = agentScript == null ? (player.GetButtonUp("Jump") || JumpPressed) : agentScript.jump;

            // Transform moveDirection vector to be relative to camera view direction

            moveDirection = moveDirection.relativeTo(myCamera);
        }

        public void GameOver()
        {
            myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
            tPersonCam.FocusCharacter();
            
            if(Rank == 1)
                SetRank("Victory");
            else
                SetRank("Defeat");

            this.enabled = false;
        }

        private void SetRank(string value) {
            myAnimator.Play(value, 0, 0f);

            if(agentScript == null){
                UIPopup popup = UIPopup.GetPopup(value);
                if(popup != null)
                    popup.Show();
            }

            if(value.Equals("Victory")) {
                var x = Instantiate(confetties, transform.position, Quaternion.identity).transform;
                x.SetParent(transform);
                x.rotation = Quaternion.Euler(Vector3.zero);
            }
        }

        #endregion

        #region MONOBEHAVIOUR

        /// <summary>
        /// Overrides 'BaseCharacterController' OnValidate method,
        /// to perform this class editor exposed fields validation.
        /// </summary>

        public override void OnValidate()
        {
            // Validate 'BaseCharacterController' editor exposed fields

            base.OnValidate();

            // Validate this editor exposed fields

            walkSpeed = _walkSpeed;
            runSpeed = _runSpeed;
        }

        /// <summary>
        /// Initialize this component.
        /// 
        /// NOTE: If you override this, it is important to call the parent class' version of method,
        /// (eg: base.Awake) in the derived class method implementation, in order to fully initialize the class. 
        /// </summary>

        public override void Awake() 
        {
            base.Awake();

            player = ReInput.players.GetPlayer(playerID);
            myRigidbody = GetComponent<Rigidbody>();
            myAnimator = GetComponentInChildren<Animator>();
            agentScript = GetComponent<FallingFloorAgent>();
        }

        public void Start() 
        {
            nameText.SetText(Gamertag);
            gameObject.name = Gamertag;
            
            tPersonCam = myCamera.GetComponent<ThirdPersonCamera>();
            tPersonCam.player = player;
            tPersonCam.agentScript = agentScript;
            tPersonCam.target = transform.Find("LookTarget");
            tPersonCam.focusPoint = transform.Find("FocusPoint");
        }

        public override void Update()
        {
            base.Update();

            if(isGrounded && GameManager.Instance.gameState.Equals(GameManager.GameState.Playing))
            {
                Score += Time.deltaTime;
                scoreText.SetText(Score.ToString("F2"));
            }
        }

        private void OnTriggerEnter(Collider other) 
        {
            if(other.CompareTag("Death Zone"))
            {
                Score -= 5;
                scoreText.SetText(Score.ToString("F2"));

                myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
                GameEventMessage.SendEvent("Respawn", gameObject);
            }
        }

    #endregion
}
