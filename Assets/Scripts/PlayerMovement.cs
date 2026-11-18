using System;
using System.Collections.Generic;
using UnityEngine;

namespace MetroPlatform
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement Settings")]
        [SerializeField] private float _walkSpeed = 6f;

        [Header("Jump Settings")]
        [SerializeField] private float _jumpForce = 18f;
        [SerializeField] private LayerMask _jumpLayerMask;
        [SerializeField] private float _jumpTimeTotal = 0.35f;
        [SerializeField] private float _jumpFallAcceleration = 0.3f;
        [SerializeField] private float _inputBufferJumpTotal = 0.15f;
        [SerializeField] private float _coyoteTimeTotal = 0.15f;

        [Header("Inner Workings")]
        [SerializeField] private float _moveHorizontal;
        [SerializeField] private bool _canJump;
        [SerializeField] private bool _grounded;
        [SerializeField] private Collider2D _groundHit;
        [SerializeField] private float _moveVertical;
        [SerializeField] private float _jumpFallForceActual;
        [SerializeField] private float _inputBufferJumpCounter;
        [SerializeField] private float _coyoteTimeCounter;

        private float _inputAxisX => _playerInput.InputAxisX;
        private bool _inputJump => _playerInput.InputJump;

        [Header("Timers")]
        [SerializeField] private TimerCountdown _timerJump;
        [SerializeField] private TimerCountdown _timerFall;
        [SerializeField] private TimerCountdown _timerCoyoteTime;
        [SerializeField] private List<TimerCountdown> _timers;

        [Header("Refs")]
        [SerializeField] private PlayerInput _playerInput;
        [SerializeField] private Transform _visuals;
        [SerializeField] private Animator _animator;
        [SerializeField] private Rigidbody2D _rigidbody;
        [SerializeField] private BoxCollider2D _groundCheckColl;

        private static readonly int AnimVelocityX = Animator.StringToHash("VelocityX");
        private static readonly int AnimJumping = Animator.StringToHash("Jumping");
        private static readonly int AnimFalling = Animator.StringToHash("Falling");

        private void Awake()
        {
            if (_rigidbody == null)
                _rigidbody = GetComponent<Rigidbody2D>();

            if (_animator == null)
                _animator = _visuals.GetComponent<Animator>();

            if (_playerInput == null)
                _playerInput = GetComponent<PlayerInput>();

            InitializeTimers();
        }

        private void InitializeTimers()
        {
            _timerJump = new TimerCountdown(_jumpTimeTotal, "_timerJump");
            _timerJump.Debugging = true;
            _timerFall = new TimerCountdown(999f, "_timerFall");
            _timerCoyoteTime = new TimerCountdown(_coyoteTimeTotal, "_timerCoyoteTime");

            _timerJump.TimerStart += () =>
                _animator.SetBool(AnimJumping, _timerJump.IsRunning);
            _timerJump.TimerStop += () =>
            {
                _animator.SetBool(AnimJumping, _timerJump.IsRunning);
                _timerFall.Start();
            };
            _timerFall.TimerStart += () =>
            {
                _jumpFallForceActual = 0;
                _animator.SetBool(AnimFalling, _timerFall.IsRunning);
            };
            _timerFall.TimerStop += () =>
                _animator.SetBool(AnimFalling, _timerFall.IsRunning);

            _timers = new List<TimerCountdown>() { _timerJump, _timerFall, _timerCoyoteTime };
        }

        private void Update()
        {
            InputBufferJump(_inputJump);
            OnJump(_inputBufferJumpCounter > 0f);

            _canJump = CanJump();

            UpdateTimers();

            Animate();
        }

        private void UpdateTimers()
        {
            foreach (var timer in _timers)
            {
                if (timer.IsRunning)
                {
                    Debug.Log($"Timer {timer.Name} is Running");
                    timer.Tick(Time.deltaTime);
                }
            }
        }

        private void InputBufferJump(bool inputJump)
        {
            if (inputJump)
                _inputBufferJumpCounter = _inputBufferJumpTotal;
            else if (_inputBufferJumpCounter > 0f)
                _inputBufferJumpCounter -= Time.deltaTime;
        }

        private bool CanJump()
        {
            _grounded = Grounded();

            if (_grounded)
                _coyoteTimeCounter = _coyoteTimeTotal;
            else if (_coyoteTimeCounter > 0f)
                _coyoteTimeCounter -= Time.deltaTime;

            return _coyoteTimeCounter > 0f;
        }

        private void FixedUpdate()
        {
            _moveHorizontal = FixedMove(_inputAxisX);

            if (_timerJump.IsRunning)
                _moveVertical = FixedJump();
            else if (_timerFall.IsRunning)
                _moveVertical = FixedFall();

            ChangeTimersState();

            _rigidbody.linearVelocity = new Vector2(_moveHorizontal, _moveVertical);
        }

        private float FixedMove(float inputAxisX)
        {
            return inputAxisX * _walkSpeed;
        }

        private float FixedJump()
        {
            return _jumpForce * _timerJump.ProgressReversed;
        }

        private float FixedFall()
        {
            _jumpFallForceActual -= _jumpFallAcceleration;

            return _jumpFallForceActual;
        }

        private void OnJump(bool inputJump)
        {
            if (inputJump && _canJump && !_timerJump.IsRunning)
                _timerJump.Start(_jumpTimeTotal);
        }

        private void ChangeTimersState()
        {
            if (_timerFall.IsRunning && _grounded)
                _timerFall.Stop();

            if (_timerJump.IsRunning && _grounded && _timerJump.Progress > 0.5f)
                _timerJump.Stop();
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

        private void Animate()
        {
            _animator.SetFloat(AnimVelocityX, Math.Abs(_inputAxisX));
        }
    }
}
