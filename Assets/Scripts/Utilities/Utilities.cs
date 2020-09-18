using System;
using Unity.Mathematics;


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
}

