using System.Collections.Generic;

public class PQ
{
	public List<Node> queue;
	
	public PQ()
	{
		queue = new List<Node>();
	}
	
	public bool isEmpty()
	{
		return queue.Count <= 0;
	}
	
	public void insert (Node newNode)
	{
		foreach (Node n in queue)
		{
			if (newNode.equals(n))
			{
				if (newNode.f < n.f)
				{
					n.f = newNode.f;
					n.g = newNode.g;
					n.path = newNode.path;
				}
				return;
			}
		}
		
		queue.Add(newNode);
	}
	
	public Node pop()
	{
		float min_f = queue[0].f;
		Node min = queue[0];
		
		foreach (Node n in queue)
		{
			if (n.f < min_f)
			{
				min_f = n.f;
				min = n;
			}
		}
		
		queue.Remove(min);
		return min;
	}
}