using DG.Tweening;
using UnityEngine;

public class ToyReturnState : ToyBaseState
{
    private readonly int CloseHash = Animator.StringToHash("Close");
    private readonly int OpenHash = Animator.StringToHash("Open");
    private readonly int IdleHash = Animator.StringToHash("Idle"); // Tambahkan hash untuk state Idle

    private Sequence returnSequence;

    public ToyReturnState(ToyStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        returnSequence = DOTween.Sequence();

        // 1. Trigger animasi ngebuka saat terlepas
        returnSequence.AppendCallback(() => stateMachine.Animator.CrossFadeInFixedTime(OpenHash, 0.1f));

        // 2. Delay ngebuka
        returnSequence.AppendInterval(0.15f);

        // 3. Bergerak kembali ke posisi dan rotasi awal
        returnSequence.Append(stateMachine.transform.DOMove(stateMachine.InitialPosition, 0.5f).SetEase(Ease.OutBack));
        returnSequence.Join(stateMachine.transform.DORotateQuaternion(stateMachine.InitialRotation, 0.5f).SetEase(Ease.OutBack));

        // 4. Trigger nutup setelah sampai
        returnSequence.AppendCallback(() => stateMachine.Animator.CrossFadeInFixedTime(CloseHash, 0.1f));

        // 5. Beri waktu agar animasi nutup selesai dimainkan (sesuaikan durasi ini dengan panjang klip Close-mu)
        returnSequence.AppendInterval(0.4f);

        // 6. Kembalikan animator ke animasi Idle
        returnSequence.AppendCallback(() => stateMachine.Animator.CrossFadeInFixedTime(IdleHash, 0.1f));

        returnSequence.OnComplete(() =>
        {
            // State machine script kembali ke ToyIdleState
            AudioEvents.TriggerPlayCustomSFX(Modules.SoundSystems.AudioKey.SFX_Assemble);
            stateMachine.SwitchState(new ToyIdleState(stateMachine));
        });
    }

    public override void Tick(float deltaTime) { }

    public override void Exit()
    {
        returnSequence?.Kill();
    }
}