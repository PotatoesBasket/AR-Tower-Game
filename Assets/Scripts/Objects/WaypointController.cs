using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class WaypointController : MonoBehaviour
{
    public List<Waypoint> waypoints;
    public bool reverseAtEnd = false;
    public float sphereDisplaySize = 0.2f;

    int currentWaypoint = 0;
    int nextWaypoint = 1;
    float timer;
    bool reverse = false;

    private void Update()
    {
        if (timer >= waypoints[currentWaypoint].timeToNext) //current to next point lerp finished
        {
            timer = 0; //reset timer

            if (reverseAtEnd) //points set to reverse at end of list
            {
                if (!reverse) //not currently reversing list
                {
                    ++currentWaypoint; //increment current point

                    if (currentWaypoint == waypoints.Count - 1) //end of list, set reverse and decrement next point
                    {
                        reverse = true;
                        --nextWaypoint;
                    }
                    else //not at end of list, increment next point
                        ++nextWaypoint;
                }
                else //currently reversing list
                {
                    --currentWaypoint; //decrement current point

                    if (currentWaypoint == 0) //beginning of list, set reverse and increment next point
                    {
                        reverse = false;
                        ++nextWaypoint;
                    }
                    else //not at beginning of list, decrement next point
                        --nextWaypoint;
                }
            }
            else //points set to loop at end of list
            {
                //increment points
                ++currentWaypoint;
                ++nextWaypoint;

                //rollover
                if (currentWaypoint == waypoints.Count)
                    currentWaypoint = 0;
                if (nextWaypoint == waypoints.Count)
                    nextWaypoint = 0;
            }
        }

        timer += Time.deltaTime;

        transform.position = Vector3.Lerp(
        waypoints[currentWaypoint].transform.position,
        waypoints[nextWaypoint].transform.position,
        timer / waypoints[currentWaypoint].timeToNext);
    }

    private void OnDrawGizmos()
    {
        foreach (Waypoint point in waypoints)
        {
            if (point == waypoints[0])
                Gizmos.color = Color.red;
            else if (point == waypoints[waypoints.Count - 1])
                Gizmos.color = Color.blue;
            else
                Gizmos.color = Color.white;

            Gizmos.DrawSphere(point.transform.position, sphereDisplaySize);
        }
    }
}