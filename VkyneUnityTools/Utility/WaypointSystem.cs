using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VkyneTools.Utility
{
    public class WaypointSystem
    {
        List<Transform> _waypoints = new List<Transform>();
        List<float> _distances = new List<float>();
        float _totalDistance;
        bool _setup = false;

        bool _debug = true;

        public Action<float, int> MappedDataAction;

        public void SetUpWayPoints(Transform startPos, List<Transform> wayPoints)
        {
            _waypoints.Clear();
            _distances.Clear();
            _totalDistance = 0;

            _waypoints.Add(startPos);
            _waypoints.AddRange(wayPoints);

            _distances.Add(Vector3.Distance(startPos.position, wayPoints[0].position));

            for (int i = 0; i < wayPoints.Count - 1; i++)
            {
                float dist = Vector3.Distance(wayPoints[i].position, wayPoints[i + 1].position);
                _distances.Add(dist);
            }

            _totalDistance = _distances.Sum();
            _setup = true;
        }

        public bool SetPosition(float lerpValue, out Vector3 position, out Quaternion rotation)
        {
            lerpValue = Mathf.Clamp01(lerpValue);

            if (lerpValue == 1)
            {
                position = _waypoints.Last().position;
                rotation = _waypoints.Last().rotation;
                return true;
            }
            if (lerpValue == 0)
            {
                position = _waypoints.First().position;
                rotation = _waypoints.First().rotation;
                return true;
            }

            float distanceTravel = lerpValue * _totalDistance;
            int baseIndex = 0;

            for (int i = 0; i < _distances.Count; i++)
            {
                distanceTravel -= _distances[i];
                if (distanceTravel <= 0)
                {
                    baseIndex = i;
                    break;
                }
            }

            float percentOfIndexedDistance = (_distances[baseIndex] + distanceTravel) / _distances[baseIndex];
            position = Vector3.Lerp(_waypoints[baseIndex].position, _waypoints[baseIndex + 1].position, percentOfIndexedDistance);
            rotation = Quaternion.Lerp(_waypoints[baseIndex].rotation, _waypoints[baseIndex + 1].rotation, percentOfIndexedDistance);

            MappedDataAction?.Invoke(percentOfIndexedDistance, baseIndex);

            return percentOfIndexedDistance <= 0.1;
        }

        public void CalculatePositionFromMappedData(float lerp, int baseIndex, out Vector3 pos, out Quaternion rot)
        {
            pos = Vector3.Lerp(_waypoints[baseIndex].position, _waypoints[baseIndex + 1].position, lerp);
            rot = Quaternion.Lerp(_waypoints[baseIndex].rotation, _waypoints[baseIndex + 1].rotation, lerp);
        }

        public void DrawGizmos()
        {
            if (_debug && _setup)
            {
                for (int i = 0; i < _waypoints.Count - 1; i++)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(_waypoints[i].position, _waypoints[i + 1].position);
                    Gizmos.DrawSphere(_waypoints[i].position, 0.5f);
                }
            }
        }

    }
}


