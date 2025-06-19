using System.Collections.Generic;
using UnityEngine;

public class TrajectoryLineRenderer : MonoBehaviour
{
    [Header("References")]
    public LineRenderer trajectoryLine;

    [Header("Bezier parameters")]
    public int totalBezierPointsCount = 25;
    public int renderingBezierPointsCount = 14;

    public void CreateTrajectoryLine(Vector3 p1, Vector3 p2, Vector3 p3)
    {
        var pointList = new List<Vector3>();

        for (float i = 0; i < totalBezierPointsCount; i++)
        {
            var t1 = Vector3.Lerp(p1, p2, i / totalBezierPointsCount);
            var t2 = Vector3.Lerp(p2, p3, i / totalBezierPointsCount);
            var curve = Vector3.Lerp(t1, t2, i / totalBezierPointsCount);

            if (i < renderingBezierPointsCount)
            {
                pointList.Add(curve);
            }
        }

        trajectoryLine.positionCount = pointList.Count;
        trajectoryLine.SetPositions(pointList.ToArray());
    }
}
