using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class PrefabEntities : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
	public static Entity	prefab_Block;

	public GameObject		goPrefab_Block;


	public void DeclareReferencedPrefabs( List< GameObject > referencedPrefabs )
	=>
		referencedPrefabs.Add( goPrefab_Block );


	public void Convert( Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem )
	=>
		prefab_Block		= conversionSystem.GetPrimaryEntity( goPrefab_Block );
}

