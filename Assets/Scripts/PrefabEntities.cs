using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


public class PrefabEntities : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
	public static Entity	entityPrefab_Lego;

	public GameObject		goPrefab_Lego;


	public void DeclareReferencedPrefabs( List< GameObject > referencedPrefabs )
	=>
		referencedPrefabs.Add( goPrefab_Lego );


	public void Convert( Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem )
	=>
		entityPrefab_Lego		= conversionSystem.GetPrimaryEntity( goPrefab_Lego );
}

