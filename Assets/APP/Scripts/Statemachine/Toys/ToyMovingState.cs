using UnityEngine;
using DG.Tweening;

public class ToyMovingState : ToyBaseState
{
    private readonly int CloseHash = Animator.StringToHash("Close");
    private readonly int OpenHash = Animator.StringToHash("Open");
    private readonly int IdleHash = Animator.StringToHash("Idle"); // Tambahkan hash untuk state Idle

    private readonly Transform targetTransform;
    private Sequence moveSequence;

    public ToyMovingState(ToyStateMachine stateMachine, Transform targetPos) : base(stateMachine)
    {
        targetTransform = targetPos;
    }

    public override void Enter()
    {
        moveSequence = DOTween.Sequence();

        // 1. Trigger animasi ngebuka
        moveSequence.AppendCallback(() => stateMachine.Animator.CrossFadeInFixedTime(OpenHash, 0.1f));

        // 2. Beri delay sedikit agar animasi buka terlihat sebelum mulai jalan (silakan tweak nilainya)
        moveSequence.AppendInterval(0.15f);

        // 3. Bergerak menuju slot (Gunakan Append agar jalan SETELAH interval selesai)
        moveSequence.Append(stateMachine.transform.DOMove(targetTransform.position, 0.4f).SetEase(Ease.OutBack));
        moveSequence.Join(stateMachine.transform.DORotateQuaternion(targetTransform.rotation, 0.4f).SetEase(Ease.OutBack));

        // 4. Trigger animasi nutup setelah sampai
        moveSequence.AppendCallback(() => stateMachine.Animator.CrossFadeInFixedTime(CloseHash, 0.1f));

        // 5. Beri waktu agar animasi nutup selesai dimainkan sebelum pindah ke Idle
        moveSequence.AppendInterval(0.4f);

        // 6. Kembalikan animator ke animasi Idle
        moveSequence.AppendCallback(() => stateMachine.Animator.CrossFadeInFixedTime(IdleHash, 0.1f));

        moveSequence.OnComplete(() =>
        {
            AudioEvents.TriggerPlayCustomSFX(Modules.SoundSystems.AudioKey.SFX_Assemble);
            stateMachine.SwitchState(new ToyAssembledState(stateMachine));
        });
    }

    public override void Tick(float deltaTime) { }

    public override void Exit()
    {
        moveSequence?.Kill();
    }
}