using Cysharp.Threading.Tasks;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

namespace MapObject
{
    public class CuttingBoard : MonoBehaviour
    {
        public XRGrabInteractable knifeInteractable;
        public TeleportationAnchor teleportationAnchor;

        public bool isPositioned = false;
        public float positionedRange = 1f;

        Vector3 knifeInitialPosition;
        Quaternion knifeInitialRotation;

        public XROrigin player;

        public GameObject debugObject;

        public CuttingIngredient currentIngredient;


        void Start()
        {
            player = FindFirstObjectByType<XROrigin>();

            knifeInitialPosition = knifeInteractable.transform.position;
            knifeInitialRotation = knifeInteractable.transform.rotation;
        }

        async UniTask CheckPlayerPosition()
        {
            // 플레이어가 텔레포트되는데 시간이 걸려서 붙는 거 대기
            await UniTask.WaitWhile(() => Vector3.Distance(player.transform.position, transform.position) > positionedRange);

            debugObject.SetActive(true);

            // 플레이어가 보드에서 멀어지면 리셋
            await UniTask.WaitWhile(() => Vector3.Distance(player.transform.position, transform.position) < positionedRange);

            isPositioned = false;
            knifeInteractable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)knifeInteractable);
            knifeInteractable.transform.SetPositionAndRotation(knifeInitialPosition, knifeInitialRotation);

            debugObject.SetActive(false);
            teleportationAnchor.enabled = true;
        }

        public void OnTeleportAnchorTeleported()
        {
            // grabbed.transform.SetPositionAndRotation(transform.position + Vector3.up, Quaternion.Euler(0, 0, 0));
            isPositioned = true;
            teleportationAnchor.enabled = false;

            if (currentIngredient != null)
            {
                currentIngredient.SetPosition(transform.position + Vector3.up * 0.1f);
            }

            CheckPlayerPosition().Forget();
        }

        public void GrabKnife()
        {
            if (!isPositioned)
            {
                teleportationAnchor.RequestTeleport();
            }
        }
    }

}
