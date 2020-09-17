using System;


public static class Utilities
{
	public static T SingletonPattern< T >( T @this, T instance ) where T : class
	{
		if (instance != null)
			throw new Exception( $"Singleton pattern violation: instance of class { @this.GetType().Name } already exists!" );
		
		return @this;
	}
}

