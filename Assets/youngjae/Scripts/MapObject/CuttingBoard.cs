using System.Threading;
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
        PlayerIngredient boardIngredient;

        Collider[] overlapColliders = new Collider[5];

        CancellationTokenSource cancellationTokenSource;

        void Start()
        {
            player = PlayerManager.Instance.player;

            knifeInitialPosition = knifeInteractable.transform.position;
            knifeInitialRotation = knifeInteractable.transform.rotation;

            knife.canCutting = false;
        }

        async UniTask CheckPlayerPosition(CancellationToken cancellationToken)
        {
            try
            {
                // 플레이어가 텔레포트되는데 시간이 걸려서 붙는 거 대기
                await UniTask.WaitWhile(() => Vector3.Distance(player.transform.position, transform.position) > positionedRange, cancellationToken: cancellationToken);

                if (cancellationToken.IsCancellationRequested) return;

                debugObject.SetActive(true);

                await UniTask.Delay(2000, cancellationToken: cancellationToken);

                if (cancellationToken.IsCancellationRequested) return;

                // 플레이어가 보드에서 멀어지면 리셋
                await UniTask.WaitWhile(() => Vector3.Distance(player.transform.position, transform.position) < positionedRange, cancellationToken: cancellationToken);

                if (cancellationToken.IsCancellationRequested) return;

                EndCutting(); // 정상적으로 플레이어가 멀어진 경우
            }
            catch (System.OperationCanceledException)
            {
                Debug.Log("CheckPlayerPosition was canceled.");
                // EndCutting()은 외부에서 호출될 때 이미 실행되므로 여기서 중복 호출할 필요는 없음.
                // 필요한 경우 추가적인 취소 관련 정리 작업 수행
            }
        }

        private void EndCutting()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }

            isPositioned = false;
            knifeInteractable.interactionManager.CancelInteractableSelection((IXRSelectInteractable)knifeInteractable);
            knifeInteractable.transform.SetPositionAndRotation(knifeInitialPosition, knifeInitialRotation);

            if (boardIngredient != null)
            {
                boardIngredient.SetInteractable(true);
            }

            knife.canCutting = false;
            debugObject.SetActive(false);
            teleportationAnchor.enabled = true;
        }

        public void OnTeleportAnchorTeleported()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
            }
            cancellationTokenSource = new CancellationTokenSource();

            // grabbed.transform.SetPositionAndRotation(transform.position + Vector3.up, Quaternion.Euler(0, 0, 0));
            isPositioned = true;
            teleportationAnchor.enabled = false;

            if (PlayerManager.Instance.GetSelectedIngredientCount() > 0)
            {
                boardIngredient = PlayerManager.Instance.GetFirstSelectedIngredientWithItemDeselect(deselectOther: true);
            }
            else
            {
                int count = Physics.OverlapSphereNonAlloc(transform.position, positionedRange, overlapColliders);
                if (count > 0)
                {
                    for (int i = 0; i < overlapColliders.Length; i++)
                    {
                        if (overlapColliders[i] == null)
                            continue;

                        if (overlapColliders[i].gameObject.TryGetComponent(out PlayerIngredient ingredient))
                        {
                            boardIngredient = ingredient;
                            break;
                        }
                    }
                }

            }

            boardIngredient.SetPosition(transform.position + Vector3.up * 0.1f, true);
            boardIngredient.OnIngredientSelected += OnIngredientSelected;

            knife.canCutting = true;

            CheckPlayerPosition(cancellationTokenSource.Token).Forget();
        }

        public void GrabKnife()
        {
            if (!isPositioned)
            {
                teleportationAnchor.RequestTeleport();
            }
        }

        void OnIngredientSelected(PlayerIngredient ingredient)
        {
            ingredient.OnIngredientSelected -= OnIngredientSelected;
            EndCutting();
        }

        void OnDestroy()
        {
            if (cancellationTokenSource != null)
            {
                cancellationTokenSource.Cancel();
                cancellationTokenSource.Dispose();
                cancellationTokenSource = null;
            }
        }
    }

}
