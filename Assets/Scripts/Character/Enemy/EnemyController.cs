using Assets.Scripts.Character.Enemy.States;
using Assets.Scripts.EventBus;
using Assets.Scripts.FSM;
using UnityEngine;

namespace Assets.Scripts.Character.Enemy
{
  public class EnemyController : MonoBehaviour
  {
    public CharacterAnimationHandler CharacterAnimationHandler { get; private set; }

    public FiniteStateMachine FSM { get; private set; }

    private EventBinding<Events.CharacterDead> _characterDead;

    private void Awake()
    {
      CharacterAnimationHandler = GetComponent<CharacterAnimationHandler>();

      FSM = new FiniteStateMachine();
    }

    private void OnEnable()
    {
      _characterDead = new EventBinding<Events.CharacterDead>(OnCharacterDead);
      EventBus<Events.CharacterDead>.Register(_characterDead);
    }

    private void OnDisable()
    {
      EventBus<Events.CharacterDead>.Unregister(_characterDead);
    }

    private void Start()
    {
      FSM.Init(new EnemyMovementState(this));
    }

    private void Update()
    {
      FSM.CurrentState.Update();
    }

    private void OnCharacterDead(Events.CharacterDead characterDead)
    {
      if (characterDead.ID != transform.GetInstanceID() && FSM.CurrentState is not EnemyDeathState)
      {
        return;
      }

      FSM.ChangeState(new EnemyDeathState(this));
    }
  }
}