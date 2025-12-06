using UnityEngine;
using DG.Tweening;

public class BallScript : MonoBehaviour
{
    [SerializeField] internal Transform parent_Transform;
    [SerializeField] private SpinManager spinManager;
    private Transform trargetPosition;
    internal bool ballStopped = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Stopper"))
            return;

        Debug.Log("Ball Triggered Stopper!");

        // ballScale = transform.localScale;
        // transform.SetParent(parent_Transform , true);
        // spinManager.parentDone = true;
        // Start where collision happened
        // Vector3 startPos = transform.localPosition;
        // transform.localScale = ballScale;
        // Animate to zero (slot center)
        // transform.DOLocalMove(collision.transform.localPosition, 3.55f)
        //     .SetEase(Ease.OutQuad)
        //     .OnComplete(() =>
        //     {
        //         StartCoroutine(spinManager.StopAtNumber());
        //     });
        trargetPosition = collision.transform.parent;
        StartCoroutine(spinManager.StopAtNumber());

    }
    private void Update()
    {
        if (trargetPosition != null)
        {
            float speed = 6f;

            transform.position = Vector3.Lerp(
                transform.position,
                trargetPosition.position,
                speed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, trargetPosition.position) < 0.001f)
            {
                transform.position = trargetPosition.position;
                ballStopped = true;
                trargetPosition = null;
            }
        }
    }

}
