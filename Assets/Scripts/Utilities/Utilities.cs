using System;
using Unity.Mathematics;
using UnityEngine;


public static class Utilities
{
	public static T SingletonPattern< T >( T @this, T instance ) where T : class
	{
		if (instance != null)
			throw new Exception( $"Singleton pattern violation: instance of class { @this.GetType().Name } already exists!" );
		
		return @this;
	}


#region Math

	// https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
	public static float		NegativeSafeMod( this float x, float m )			=> (x % m + m) % m;
	public static float2	NegativeSafeMod( this float2 x, float m )			=> (x % m + m) % m;

#endregion


	public static Rect GetWorldRect( this RectTransform rt )
	{
		Vector3[] corners		= new Vector3[ 4 ];

		rt.GetWorldCorners( corners );

		Vector2 min		= Vector2.positiveInfinity;
		Vector2 max		= Vector2.negativeInfinity;

		foreach (var corner in corners)
		{
			min			= Vector2.Min( min, corner );
			max			= Vector2.Max( max, corner );
		}

		return new Rect( min, max - min );
	}
}

