using DG.Tweening;
using UnityEngine;

namespace Components.Character
{
    public class CameraRecoilComponent
    {
        private readonly Transform _cameraTarget;
        private readonly float _recoilAmountMultiplier;
        private readonly float _recoilSpeedMultiplier;

        public CameraRecoilComponent(Transform cameraTarget, float amountMultiplier, float speedMultiplier)
        {
            _cameraTarget = cameraTarget;
            _recoilAmountMultiplier = amountMultiplier;
            _recoilSpeedMultiplier = speedMultiplier;
        }

        public void ApplyRecoil(Vector3 recoilAmount, float recoilSpeed)
        {
            if (_cameraTarget == null || recoilAmount == Vector3.zero) return;

            _cameraTarget.DOKill();

            float newRotationX = _cameraTarget.localEulerAngles.x - (recoilAmount.y * _recoilAmountMultiplier);

            _cameraTarget.DOLocalRotate(
                new Vector3(newRotationX, _cameraTarget.localEulerAngles.y, _cameraTarget.localEulerAngles.z),
                recoilSpeed * _recoilSpeedMultiplier).SetEase(Ease.OutQuad);
        }
    }
}