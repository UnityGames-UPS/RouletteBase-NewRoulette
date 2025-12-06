using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SpinManager : MonoBehaviour
{

    [SerializeField]
    private Transform Roulette_OuterCircle;
    [SerializeField]
    private Transform Roulette_BallContainer;
    [SerializeField]
    private Transform Ball_Transform;

    [Header("Lists & Arrays")]
    [SerializeField]
    private Transform[] BallStopPoint;

    [Header("Managers")]
    [SerializeField]
    private BallScript ballScript;

    [SerializeField]
    private Vector3 initialBallPosition;

    private int numberAnnounced = 15;
    private Tweener ballMovement = null;
    private Tweener OuterRouletteMovement = null;
    private Tweener InnerRouletteMovement = null;

    [SerializeField] private GameObject Stopper_pref;
    private GameObject Stopper;
    internal bool parentDone = false;

    [SerializeField] private AudioController audioController;

    private void Awake()
    {
        initialBallPosition = Ball_Transform.localPosition;
    }



    internal void StartSpinning(int num)
    {
        parentDone = false;
        ballScript.ballStopped = false;
        if (Ball_Transform) Ball_Transform.SetParent(Roulette_BallContainer);
        if (Ball_Transform) Ball_Transform.localPosition = initialBallPosition;
        audioController.PlayBallRolling();
        if (Roulette_OuterCircle) Roulette_OuterCircle.localEulerAngles = new Vector3(0, 0, 359);
        if (Roulette_OuterCircle) OuterRouletteMovement = Roulette_OuterCircle.DORotate(new Vector3(0, 0, 0), 2, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
            // .OnUpdate(() =>
            // {
            //     if (parentDone){ 
            //     Ball_Transform.localRotation = Quaternion.Inverse(Roulette_OuterCircle.localRotation);
            //     }
            // });
        if (Roulette_BallContainer) Roulette_BallContainer.localEulerAngles = new Vector3(0, 0, 0);
        if (Roulette_BallContainer) ballMovement = Roulette_BallContainer.DORotate(new Vector3(0, 0, 359), 1, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1)
            .OnUpdate(() =>
            {
                // if (parentDone) return;
                Ball_Transform.localRotation = Quaternion.Inverse(Roulette_BallContainer.localRotation);
            });

        if( num == 00 ) numberAnnounced = 37;
        else numberAnnounced = num;
        Stopper = Instantiate(Stopper_pref, BallStopPoint[numberAnnounced]);
        Stopper.transform.localPosition = Vector2.zero;
        StartCoroutine(BallRevolution(numberAnnounced));
    }

    private IEnumerator BallRevolution(int number)
    {
        yield return new WaitForSecondsRealtime(5);

        while (Ball_Transform.localPosition.x > 145 && Ball_Transform.localPosition.y > 30)
        {
            Ball_Transform.localPosition = new Vector2(Ball_Transform.localPosition.x - 2f, Ball_Transform.localPosition.y - 1f);
            ballMovement.timeScale -= 0.05f;
            OuterRouletteMovement.timeScale -= 0.025f;

            ballMovement.timeScale = Mathf.Max(ballMovement.timeScale, 0.1f);
            OuterRouletteMovement.timeScale = Mathf.Max(OuterRouletteMovement.timeScale, 0.1f);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        ballScript.parent_Transform = BallStopPoint[number];

        Stopper.GetComponent<BoxCollider2D>().enabled = true;
        print("triggered");
    }


    internal IEnumerator StopAtNumber()
    {

        audioController.PlayBallStop();
        ballMovement.Kill();

        yield return new WaitForSecondsRealtime(3);

        Stopper.GetComponent<BoxCollider2D>().enabled = false;
        Destroy(Stopper);
        Stopper = null;

        OuterRouletteMovement.Kill();
        OuterRouletteMovement = null;

        ballMovement = null;
    }

}
