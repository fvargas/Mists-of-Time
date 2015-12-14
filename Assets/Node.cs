using UnityEngine;
using System.Collections.Generic;

public class Node
{
	public float g;
	public float h;
	public float f;
	public List<Vector4> path;
	public int x;
	public int y;
	public int z;
	public int w;

	private static readonly int EPSILON = 5;
	
	public Node(int x, int y, int z, int w, float g, float h, List<Vector4> path)
	{
		this.x = x;
		this.y = y;
		this.z = z;
		this.w = w;
		this.g = g;
		this.h = h;
		this.f = g + h;
		this.path = path;
	}
	
	public bool equals(Node n) {
		return x == n.x && y == n.y && z == n.z && w == n.w;
	}
	
	public static float euclideanDistance(int start_x, int start_y, int start_z, int start_w,
	                                      int target_x, int target_y, int target_z, int target_w)
	{
		return EPSILON * Vector4.Distance (new Vector4(start_x, start_y, start_z, start_w), new Vector4(target_x, target_y, target_z, target_w));
	}
}