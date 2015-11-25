using UnityEngine;
using System.Collections.Generic;

public class PathPlanning
{

	public static List<Vector4> Plan (Vector4 start, Vector4 target, Map m, int weight)
	{
		PQ pq = new PQ();
		HashSet<Vector4> visited = new HashSet<Vector4>();
		int target_x = m.getRowNumber(target.x);
		int target_y = m.getVerNumber(target.y);
		int target_z = m.getColNumber(target.z);
		int target_w = (int)target.w;
		int start_x = m.getRowNumber(start.x);
		int start_y = m.getVerNumber(start.y);
		int start_z = m.getColNumber(start.z);
		int start_w = (int)start.w;
		
		float eDist = Node.euclideanDistance(start_x, start_y, start_z, start_w, target_x, target_y, target_z, target_w);
		Node init = new Node(start_x, start_y, start_z, start_w, 0, eDist, new List<Vector4>());
		pq.insert(init);

		int[][,,] inf_map = m.getInfluenceMapCopy ();
		Map.subtractInfluenceMap(start, weight, inf_map);

		while (!pq.isEmpty())
		{
			Node n = pq.pop();
			visited.Add(new Vector4(n.x, n.y, n.z, n.w));
			if (n.x == target_x && n.y == target_y && n.z == target_z && n.w == target_w)
			{
				return n.path;
			}

			processNextActions(pq, visited, n, target_x, target_y, target_z, target_w, m, inf_map);
		}

		return new List<Vector4> ();
	}

	private static void processNextActions(PQ pq, HashSet<Vector4> visited, Node n, int target_x, int target_y, int target_z, int target_w, Map m, int[][,,] inf_map)
	{
		bool atLadder = m.getGridValue(n.w, n.x, n.y, n.z) == Map.GO_LADDER;

		// Set of actions for 6-way connected grid
		if (isValid(n.x - 1, n.y, n.z, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x - 1, n.y, n.z, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x - 1, n.y, n.z, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z)];
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z)];
			//}
			Node newNode = new Node(n.x - 1, n.y, n.z, n.w, cost, h, newPath);
			pq.insert(newNode);
		}
		if (isValid(n.x + 1, n.y, n.z, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x + 1, n.y, n.z, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x + 1, n.y, n.z, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z)];
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z)];
			//}
			Node newNode = new Node(n.x + 1, n.y, n.z, n.w, cost, h, newPath);
			pq.insert(newNode);
		}
		if (atLadder && isValid(n.x, n.y - 1, n.z, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x, n.y - 1, n.z, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x, n.y - 1, n.z, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x, n.y - 1, n.z)];
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x, n.y - 1, n.z)];
			//}
			Node newNode = new Node(n.x, n.y - 1, n.z, n.w, cost, h, newPath);
			pq.insert(newNode);
		}
		if (atLadder && isValid(n.x, n.y + 1, n.z, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x, n.y + 1, n.z, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x, n.y + 1, n.z, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x, n.y + 1, n.z)];
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x, n.y + 1, n.z)];
			//}
			Node newNode = new Node(n.x, n.y + 1, n.z, n.w, cost, h, newPath);
			pq.insert(newNode);
		}
		if (isValid(n.x, n.y, n.z - 1, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x, n.y, n.z - 1, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x, n.y, n.z - 1, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x, n.y, n.z - 1)];
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x, n.y, n.z - 1)];
			//}
			Node newNode = new Node(n.x, n.y, n.z - 1, n.w, cost, h, newPath);
			pq.insert(newNode);
		}
		if (isValid(n.x, n.y, n.z + 1, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x, n.y, n.z + 1, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x, n.y, n.z + 1, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x, n.y, n.z + 1)];
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x, n.y, n.z + 1)];
			//}
			Node newNode = new Node(n.x, n.y, n.z + 1, n.w, cost, h, newPath);
			pq.insert(newNode);
		}
		
		// Additional set of actions for 10-way connected grid
		if (isValid(n.x - 1, n.y, n.z - 1, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x - 1, n.y, n.z - 1, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x - 1, n.y, n.z - 1, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z - 1)] * 1.4f;
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z - 1)] * 1.4f;
			//}
			Node newNode = new Node(n.x - 1, n.y, n.z - 1, n.w, cost, h, newPath);
			pq.insert(newNode);
		}
		if (isValid(n.x - 1, n.y, n.z + 1, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x - 1, n.y, n.z + 1, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x - 1, n.y, n.z + 1, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z + 1)] * 1.4f;
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z + 1)] * 1.4f;
			//}
			Node newNode = new Node(n.x - 1, n.y, n.z + 1, n.w, cost, h, newPath);
			pq.insert(newNode);
		}
		if (isValid(n.x + 1, n.y, n.z - 1, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x + 1, n.y, n.z - 1, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x + 1, n.y, n.z - 1, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z - 1)] * 1.4f;
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z - 1)] * 1.4f;
			//}
			Node newNode = new Node(n.x + 1, n.y, n.z - 1, n.w, cost, h, newPath);
			pq.insert(newNode);
		}
		if (isValid(n.x + 1, n.y, n.z + 1, n.w, visited,m))
		{
			float h = Node.euclideanDistance(n.x + 1, n.y, n.z + 1, n.w, target_x, target_y, target_z, target_w);
			List<Vector4> newPath = new List<Vector4>(n.path);
			Vector4 waypoint = new Vector4(n.x + 1, n.y, n.z + 1, n.w);
			newPath.Add(waypoint);
			
			float cost;
			/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z + 1)] * 1.4f;
				} else {*/
			cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(n.w, n.x + 1, n.y, n.z + 1)] * 1.4f;
			//}
			Node newNode = new Node(n.x + 1, n.y, n.z + 1, n.w, cost, h, newPath);
			pq.insert(newNode);
		}

		// Set of actions for time travel
		int numLevels = m.getNStates();
		if (numLevels >= 2) {
			int w = (n.w + 1) % numLevels;
			if (isValid(n.x, n.y, n.z, w, visited,m))
			{
				float h = Node.euclideanDistance(n.x, n.y, n.z, w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				Vector4 waypoint = new Vector4(n.x, n.y, n.z, w);
				newPath.Add(waypoint);
				
				float cost;
				/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z)];
				} else {*/
				cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(w, n.x, n.y, n.z)];
				//}
				Node newNode = new Node(n.x, n.y, n.z, w, cost, h, newPath);
				pq.insert(newNode);
			}
		}
		if (numLevels >= 3) {
			int w = (n.w - 1 + numLevels) % numLevels;
			if (isValid(n.x, n.y, n.z, w, visited,m))
			{
				float h = Node.euclideanDistance(n.x, n.y, n.z, w, target_x, target_y, target_z, target_w);
				List<Vector4> newPath = new List<Vector4>(n.path);
				Vector4 waypoint = new Vector4(n.x, n.y, n.z, w);
				newPath.Add(waypoint);
				
				float cost;
				/*if (this.tag == "Dragon") {
					cost = n.g + GameManager.dragonGridValueMap[m.getGridValue(n.w, n.x - 1, n.y, n.z)];
				} else {*/
				cost = n.g + Map.getInfluenceMapValue(waypoint, inf_map);//GameManager.gridValueMap[m.getGridValue(w, n.x, n.y, n.z)];
				//}
				Node newNode = new Node(n.x, n.y, n.z, w, cost, h, newPath);
				pq.insert(newNode);
			}
		}
	}

	private static bool isValid(int x, int y, int z, int w, HashSet<Vector4> visited, Map m)
	{
		int numRows = m.getNRows();
		int numCols = m.getNCols();
		int numVers = m.getNVers();
		int numLevels = m.getNStates ();
		
		bool squareInBounds = 0 <= w && w < numLevels && 0 <= x && x < numRows && 0 <= y && y < numVers && 0 <= z && z < numCols;
		bool haveVisited = visited.Contains(new Vector4(x, y, z, w));
		bool ret = squareInBounds && !haveVisited && GameManager.gridValueMap[m.getGridValue(w, x, y, z)] >= 0;
		
		return ret;
	}
}