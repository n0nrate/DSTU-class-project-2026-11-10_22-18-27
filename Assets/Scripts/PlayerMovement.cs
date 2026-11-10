using UnityEngine;

namespace MetroPlatform
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 6f;

        [Header("Jump Settings")]
        [SerializeField] private float _jumpForce = 9f;
        [SerializeField] private LayerMask _jumpLayerMask;
        [SerializeField] private float _jumpTimeTotal = 0.15f;
        [SerializeField] private float _inputBufferJumpTotal = 0.15f;
        [SerializeField] private float _coyoteTimeTotal = 0.15f;

        [Header("Inner Workings")]
        [SerializeField] private float _moveHorizontal;
        [SerializeField] private bool _canJump;
        [SerializeField] private bool _grounded;
        [SerializeField] private Collider2D _groundHit;
        [SerializeField] private float _moveVertical;
        [SerializeField] private bool _jumping;
        [SerializeField] private float _jumpCounter;
        [SerializeField] private float _inputBufferJumpCounter;
        [SerializeField] private float _coyoteTimeCounter;

        private float _inputAxisX => _playerInput.InputAxisX;
        private bool _inputJump => _playerInput.InputJump;

        [Header("Refs")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private BoxCollider2D _groundCheckColl;

        private void Awake()
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody2D>();

            if (_playerInput == null)
                _playerInput = GetComponent<PlayerInput>();
        }

        private void Update()
        {
            InputBufferJump(_inputJump);
            CanJump();
            OnJump(_inputBufferJumpCounter > 0f);

            if (_jumping)
            {
                _jumpCounter -= Time.deltaTime;

                if (_jumpCounter <= 0f)
                    _jumping = false;
            }
        }

        private void InputBufferJump(bool inputJump)
        {
            if (inputJump)
                _inputBufferJumpCounter = _inputBufferJumpTotal;
            else if (_inputBufferJumpCounter > 0f)
                _inputBufferJumpCounter -= Time.deltaTime;
        }

        private void CanJump()
        {
            _grounded = Grounded();

            if (_grounded)
                _coyoteTimeCounter = _coyoteTimeTotal;
            else if (_coyoteTimeCounter > 0f)
                _coyoteTimeCounter -= Time.deltaTime;

            _canJump = _coyoteTimeCounter > 0f;
        }

        private void FixedUpdate()
        {
            _moveHorizontal = FixedMove(_inputAxisX);
            _moveVertical = FixedJump(_jumping);

            _rigidbody.linearVelocity = new Vector2(_moveHorizontal, _moveVertical);
        }

        private float FixedMove(float inputAxisX)
        {
            return inputAxisX * _walkSpeed;
        }

        private float FixedJump(bool jumping)
        {
            if (jumping)
                return _jumpForce;

            return 0f;
        }

        private void OnJump(bool inputJump)
        {
            if (inputJump && _canJump)
            {
                _jumping = true;
                _jumpCounter = _jumpTimeTotal;
                _inputBufferJumpCounter = 0f;
                _coyoteTimeCounter = 0f;
            }
        }

        private bool Grounded()
        {
            RaycastHit2D boxCastHit = Physics2D.BoxCast(
                _groundCheckColl.bounds.center,
                _groundCheckColl.bounds.size,
                0f,
                Vector2.down,
                0.1f,
                _jumpLayerMask);

            _groundHit = boxCastHit.collider;

            return _groundHit != null;
        }
    }
}
