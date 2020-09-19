using System;
using System.Collections.Generic;
using Unity.Mathematics;


[Serializable]
public class Painting
{
	public int2		gridSize;

	public Dictionary< int2, List< int2 > > blocks		= new Dictionary<int2, List<int2>>{};
}

