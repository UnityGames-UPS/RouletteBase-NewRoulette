using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BallScript : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private UIManager uiManager;
    [SerializeField] private AudioController audioController;
    [SerializeField] private SocketIOManager socketManager;
    [Header("Transforms")]
    [SerializeField] private Transform parent_Transform;
    [SerializeField] private Transform Ball_Transform;
    [SerializeField] private Transform Roulette_BallContainer;
    [SerializeField] private Transform Roulette_NumberCircle;
    [SerializeField] private Transform[] BallStopPoint;
    [SerializeField] private Vector3 initialBallPosition;
    [Header("Prefabs")]
    [SerializeField] private GameObject Stopper_pref;

    internal bool isSpinning = false; 

    private GameObject Stopper;
    private Tweener ballMovement = null;
    private Tweener OuterRouletteMovement = null;
    private int numberAnnounced;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("triggered");

        this.gameObject.transform.SetParent(parent_Transform);
        this.gameObject.transform.localPosition = new Vector2(0, 0);
        // StartCoroutine(uiManager.StopAtNumber());
    }

    internal IEnumerator StartSpinning()
    {
        isSpinning = true;
        audioController.PlayWLAudio("spin");
        if (Ball_Transform) Ball_Transform.SetParent(Roulette_BallContainer);
        if (Ball_Transform) Ball_Transform.localPosition = initialBallPosition;
        if (Roulette_NumberCircle) Roulette_NumberCircle.localEulerAngles = new Vector3(0, 0, 359);
        if (Roulette_NumberCircle) OuterRouletteMovement = Roulette_NumberCircle.DORotate(new Vector3(0, 0, 0), 2, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        if (Roulette_BallContainer) ballMovement = Roulette_BallContainer.DORotate(new Vector3(0, 0, 359), 1, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1);
        numberAnnounced = socketManager.resultData.payload.winningNumber;
        Stopper = Instantiate(Stopper_pref, BallStopPoint[numberAnnounced]);
        Stopper.transform.localPosition = Vector2.zero;
        isSpinning = false;
        yield return null;
    }
}
