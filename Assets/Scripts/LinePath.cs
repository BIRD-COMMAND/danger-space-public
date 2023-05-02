using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// The LinePath class represents a sequence of connected line segments defined by a set of nodes.<br/>
/// It provides various utility functions for working with paths, such as calculating distances,<br/>
/// finding the closest point on the path, and drawing the path in the scene view.
/// </summary>
[Serializable]
public class LinePath
{

	/// <summary>
	/// An array of Vector3 nodes representing the points that make up the path.
	/// </summary>
	public Vector3[] nodes;

	/// <summary>
	/// The maximum distance between the first and last nodes of the path.
	/// </summary>
	[NonSerialized]
    public float maxDist;

	/// <summary>
	/// An array of cumulative distances between each consecutive pair of nodes in the path.
	/// </summary>
	[NonSerialized]
    public float[] distances;

    /// <summary>
    /// Indexer declaration.
    /// </summary>
    public Vector3 this[int i]
    {
        get { return nodes[i]; }
        set { nodes[i] = value; CalcDistances(); }
    }

    /// <summary>
    /// Returns true if the path is empty
    /// </summary>
    public bool Empty => nodes.Length == 0;
    /// <summary>
    /// Returns the number of nodes in the path
    /// </summary>
    public int Length => nodes.Length;
    /// <summary>
    /// Returns the first node in the path
    /// </summary>
    public Vector3 First => nodes[0];
    /// <summary>
    /// Returns the last node in the path
    /// </summary>
    public Vector3 Last => nodes[nodes.Length - 1];

    /// <summary>
    /// This function creates a path of line segments
    /// </summary>
    public LinePath(IEnumerable<Vector3> nodes) { this.nodes = nodes.ToArray(); CalcDistances(); }
	/// <summary>
	/// This function creates a path of line segments
	/// </summary>
	public LinePath(params Vector3[] args) { nodes = args; CalcDistances(); }

    /// <summary>
    /// Clears the path
    /// </summary>
    public void Clear() { nodes = new Vector3[0]; distances = new float[0]; }

    /// <summary>
    /// Loops through the path's nodes and determines how far each node in
    /// the path is from the starting node.
    /// </summary>
    public void CalcDistances()
    {
        distances = new float[nodes.Length];
        distances[0] = 0;

        for (var i = 0; i < nodes.Length - 1; i++) {
            distances[i + 1] = distances[i] + Vector3.Distance(nodes[i], nodes[i + 1]);
        }

        maxDist = distances[distances.Length - 1];
    }

    /// <summary>
    /// Draws the path in the scene view
    /// </summary>
    public void Draw()
    {
        for (int i = 0; i < nodes.Length - 1; i++) {
            Debug.DrawLine(nodes[i], nodes[i + 1], Color.cyan, 0.0f, false);
        }
    }

    /// <summary>
    /// Gets the param for the closest point on the path given a position
    /// </summary>
    public float GetParam(Vector3 position, Agent agent)
    {
        int closestSegment = GetClosestSegment(position);
        return distances[closestSegment] + GetParamForSegment(position, nodes[closestSegment], nodes[closestSegment + 1], agent);
    }

	/// <summary>
	/// Find the first point in the closest line segment to the specified position
	/// </summary>
	public int GetClosestSegment(Vector3 position)
    {
        /* Find the first point in the closest line segment to the path */
        float closestDist = DistToSegment(position, nodes[0], nodes[1]);
        int closestSegment = 0;

        for (int i = 1; i < nodes.Length - 1; i++)
        {
            float dist = DistToSegment(position, nodes[i], nodes[i + 1]);
            if (dist <= closestDist) { closestDist = dist; closestSegment = i; }
        }

        return closestSegment;
    }

    /// <summary>
    /// Given a param it gets the position on the path
    /// </summary>
    public Vector3 GetPosition(float param, bool pathLoop = false)
    {
        /* Make sure the param is not past the beginning or end of the path */
        if (param < 0) { param = (pathLoop) ? param + maxDist : 0; }
        else if (param > maxDist) { param = (pathLoop) ? param - maxDist : maxDist; }

        /* Find the first node that is farther than given param */
        int i = 0;
        for (; i < distances.Length; i++) {
            if (distances[i] > param) { break; }
        }

        /* Convert it to the first node of the line segment that the param is in */
        if (i > distances.Length - 2) { i = distances.Length - 2; }
        else { i -= 1; }

        /* Get how far along the line segment the param is */
        float t = (param - distances[i]) / Vector3.Distance(nodes[i], nodes[i + 1]);

        /* Get the position of the param */
        return Vector3.Lerp(nodes[i], nodes[i + 1], t);
    }

    /// <summary>
    /// Gives the distance of a point to a line segment.
    /// p is the point, v and w are the two points of the line segment
    /// </summary>
    float DistToSegment(Vector3 p, Vector3 v, Vector3 w)
    {
        Vector3 vw = w - v;

        float l2 = Vector3.Dot(vw, vw);

        if (l2 == 0) { return Vector3.Distance(p, v); }

        float t = Vector3.Dot(p - v, vw) / l2;

        if (t < 0) { return Vector3.Distance(p, v); }

        if (t > 1) { return Vector3.Distance(p, w); }

        Vector3 closestPoint = Vector3.Lerp(v, w, t);

        return Vector3.Distance(p, closestPoint);
    }

    /// <summary>
    /// Finds the param for the closest point on the segment vw given the point p
    /// </summary>
    /// <returns>The parameter for segment.</returns>
    float GetParamForSegment(Vector3 p, Vector3 v, Vector3 w, Agent agent)
    {
        Vector3 vw = w - v;

        float l2 = Vector3.Dot(vw, vw);

        if (l2 == 0) { return 0; }

        float t = Vector3.Dot(p - v, vw) / l2;

        if (t < 0) { t = 0; }
        else if (t > 1) { t = 1; }

        /* Multiple by (v - w).magnitude instead of Sqrt(l2) because we want the magnitude of the full 3D line segment */
        return t * (v - w).magnitude;
    }

    /// <summary>
    /// Removes the specified node from the path
    /// </summary>
    /// <param name="i">Index of the node to remove</param>
    public void RemoveNode(int i)
    {
        Vector3[] newNodes = new Vector3[nodes.Length - 1];

        int newNodesIndex = 0;
        for (int j = 0; j < newNodes.Length; j++)
        {
            if (j != i) {
                newNodes[newNodesIndex] = nodes[j];
                newNodesIndex++;
            }
        }

        nodes = newNodes;
        CalcDistances();
    }

    /// <summary>
    /// Reverses the path
    /// </summary>
    public void ReversePath()
    {
        Array.Reverse(nodes);
        CalcDistances();
    }

}