using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class RouletteSpinController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform wheel;     
    [SerializeField] private RectTransform ball;         
    [SerializeField] private List<RectTransform> ballStopPoints; 

    [Header("Settings")]
    [SerializeField] private float fastSpinTime = 2.5f;    
    [SerializeField] private float inwardMoveTime = 1.2f; 
    [SerializeField] private float wheelSpinTime = 4f;     
    [SerializeField] private float extraWheelRotations = 4f; 
    [SerializeField] private float ballExtraSpin = 1440f;    

    private readonly string[] numberOrder = {
        "0","28","9","26","30","11","7","20","32","17","5","22","34","15","3","24",
        "36","13","1","00","27","10","25","29","12","8","19","31","18","6","21","33",
        "16","4","23","35","14","2"
    };

    private int GetIndex(string num)
    {
        for (int i = 0; i < numberOrder.Length; i++)
            if (numberOrder[i] == num)
                return i;
        return -1;
    }


    internal void Spin(string winningNumber)
    {
        int index = GetIndex(winningNumber);
        if (index < 0)
        {
            Debug.LogError("Invalid winning number: " + winningNumber);
            return;
        }

        RectTransform finalStop = ballStopPoints[index];

        ball.anchoredPosition = new Vector2(220f, 0);  // Adjust outer radius

        ball.DOLocalRotate(new Vector3(0, 0, -ballExtraSpin), fastSpinTime, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                MoveBallInward(finalStop);
            });

        RotateWheelTo(index);
    }



    private void MoveBallInward(RectTransform targetStop)
    {
        ball.DOAnchorPos(targetStop.anchoredPosition, inwardMoveTime)
            .SetEase(Ease.OutCubic)
            .OnComplete(() =>
            {
                // Final lock
                ball.anchoredPosition = targetStop.anchoredPosition;
            });
    }

    private void RotateWheelTo(int index)
    {
        float slice = 360f / numberOrder.Length;
        float targetAngle = -(index * slice);
        float finalAngle = (extraWheelRotations * 360f) + targetAngle;

        Vector3 startRot = wheel.localEulerAngles;

        wheel.DOLocalRotate(
            new Vector3(startRot.x, startRot.y, finalAngle),
            wheelSpinTime,
            RotateMode.FastBeyond360
        )
        .SetEase(Ease.OutCubic);
    }

}
