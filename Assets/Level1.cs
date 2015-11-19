using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class Level1 : LevelInterface
{
	public static readonly Dictionary<int, string> tileMapping = new Dictionary<int, string>
	{
		{ 0, "InterBox" },
		{ 1, "crystalC" },
		{ 2, "InterBox" },
	};


	int[,,,] getLevel()
	{
		return { getState1(), getState2(), getState3() };
	}

	int[,,] getState1()
	{

	}

	int[,,] getState2()
	{

	}

	int[,,] getState3()
	{
		
	}
}