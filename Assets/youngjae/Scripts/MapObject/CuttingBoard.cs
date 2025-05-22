using Cysharp.Threading.Tasks;
using MapObject.Ingredients;
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
        public Knife knife;

        Vector3 knifeInitialPosition;
        Quaternion knifeInitialRotation;

        XROrigin player;

        public GameObject debugObject;
        Ingredient boardIngredient;

        void Start()
        {
            player = PlayerManager.Instance.player;

            knifeInitialPosition = knifeInteractable.transform.position;
            knifeInitialRotation = knifeInteractable.transform.rotation;

            knife.canCutting = false;
        }

        async UniTask CheckPlayerPosition()
        {
            // 플레이어가 텔레포트되는데 시간이 걸려서 붙는 거 대기
            await UniTask.WaitWhile(() => Vector3.Distance(player.transform.position, transform.position) > positionedRange);

            debugObject.SetActive(true);

            await UniTask.Delay(2000);

            // 플레이어가 보드에서 멀어지면 리셋
            await UniTask.WaitWhile(() => Vector3.Distance(player.transform.position, transform.position) < positionedRange);

            isPositioned = false;
            knifeInteractable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)knifeInteractable);
            knifeInteractable.transform.SetPositionAndRotation(knifeInitialPosition, knifeInitialRotation);

            if (boardIngredient != null)
            {
                boardIngredient.ReEnable();
            }

            knife.canCutting = false;
            debugObject.SetActive(false);
            teleportationAnchor.enabled = true;
        }

        public void OnTeleportAnchorTeleported()
        {
            // grabbed.transform.SetPositionAndRotation(transform.position + Vector3.up, Quaternion.Euler(0, 0, 0));
            isPositioned = true;
            teleportationAnchor.enabled = false;

            if (PlayerManager.Instance.selectIngredient != null)
            {
                boardIngredient = PlayerManager.Instance.selectIngredient;
                boardIngredient.SetPosition(transform.position + Vector3.up * 0.1f);
            }
            else
            {
                var ingredient = FindFirstObjectByType<Ingredient>();
                if (ingredient == null)
                    return;
                if (Vector3.Distance(ingredient.transform.position, transform.position) < positionedRange)
                {
                    ingredient.SetPosition(transform.position + Vector3.up * 0.1f);
                    boardIngredient = ingredient;
                }
            }

            knife.canCutting = true;

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
