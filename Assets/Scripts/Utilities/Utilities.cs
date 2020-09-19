using System;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;


public static class Utilities
{
#region Math

	// https://stackoverflow.com/questions/1082917/mod-of-negative-number-is-melting-my-brain
	public static float		NegativeSafeMod( this float x, float m )			=> (x % m + m) % m;
	public static float2	NegativeSafeMod( this float2 x, float m )			=> (x % m + m) % m;

#endregion
#region Other

	public static T SingletonPattern< T >( T @this, T instance ) where T : class
	{
		if (instance != null)
			throw new Exception( $"Singleton pattern violation: instance of class { @this.GetType().Name } already exists!" );
		
		return @this;
	}


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


	public static string Log(
			string text		= "",

			[CallerFilePath]	string	file		= "",
			[CallerMemberName]	string	member		= "",
			[CallerLineNumber]	int		line		= 0
		)
	{
		// https://stackoverflow.com/a/16295463/4830242

		text		= $"{Path.GetFileName(file)}_{member}({line}): {text}";

		Debug.LogFormat( text );

		return text;
	}


	public static void DestroyHierarchy( Entity entity )
	{
		var ecb			= World.DefaultGameObjectInjectionWorld
												.GetOrCreateSystem< EndSimulationEntityCommandBufferSystem >().CreateCommandBuffer();

		EntityManager entityManager			= World.DefaultGameObjectInjectionWorld.EntityManager;

		if (entityManager.HasComponent< Child >( entity ))
		{
			DynamicBuffer< Child > children		= entityManager.GetBuffer< Child >( entity );

			for (int i = 0; i < children.Length; i ++)
			{
				Entity child		= children[ i ].Value;
				DestroyHierarchy( child );
				ecb.DestroyEntity( child );
			}
		}

		ecb.DestroyEntity( entity );
	}


	public static Vector2	Mouse_w			=> Camera.main.ScreenToWorldPoint( Input.mousePosition );


#endregion
}

