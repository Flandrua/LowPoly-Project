//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: An area that the player can teleport to
//
//=============================================================================

using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Valve.VR.InteractionSystem
{
	//-------------------------------------------------------------------------
	public class TeleportArea : TeleportMarkerBase
	{
		//Public properties
		public Bounds meshBounds { get; private set; }

		//Private data
		private MeshRenderer areaMesh;
		private int tintColorId = 0;
		private Color visibleTintColor = Color.clear;
		private Color highlightedTintColor = Color.clear;
		private Color lockedTintColor = Color.clear;
		private bool highlighted = false;
		private float currentAlphaPercent = 1.0f;
		private Material runtimeVisibleMaterial;
		private Material runtimeHighlightedMaterial;
		private Material runtimeLockedMaterial;
		private Mesh corridorSourceMesh;
		private Mesh corridorTiledMesh;
		private bool corridorVisualsConfigured;
		public bool showPlayAreaPreview = true;

		//-------------------------------------------------
		public void Awake()
		{
			areaMesh = GetComponent<MeshRenderer>();

#if UNITY_URP
			tintColorId = Shader.PropertyToID( "_BaseColor" );
#else
			tintColorId = Shader.PropertyToID("_TintColor");
#endif

			CalculateBounds();
		}


		//-------------------------------------------------
		public void Start()
		{
			visibleTintColor = Teleport.instance.areaVisibleMaterial.GetColor( tintColorId );
			highlightedTintColor = Teleport.instance.areaHighlightedMaterial.GetColor( tintColorId );
			lockedTintColor = Teleport.instance.areaLockedMaterial.GetColor( tintColorId );
			CacheRuntimeMaterials();
			UpdateVisuals();
		}


		//-------------------------------------------------
		private void OnDestroy()
		{
			DestroyRuntimeMaterial( ref runtimeVisibleMaterial );
			DestroyRuntimeMaterial( ref runtimeHighlightedMaterial );
			DestroyRuntimeMaterial( ref runtimeLockedMaterial );
			DestroyRuntimeMesh( ref corridorTiledMesh );
		}


		//-------------------------------------------------
		public override bool ShouldActivate( Vector3 playerPosition )
		{
			return true;
		}


		//-------------------------------------------------
		public override bool ShouldMovePlayer()
		{
			return true;
		}


		//-------------------------------------------------
		public override void SetPointerVisible( bool visible )
		{
			EnsureAreaMesh();
			if ( areaMesh != null )
			{
				areaMesh.enabled = visible;
			}
		}


		//-------------------------------------------------
		public override void Highlight( bool highlight )
		{
			if ( !locked )
			{
				highlighted = highlight;
				ApplyActiveRuntimeMaterial();
				ApplyTint( currentAlphaPercent );
			}
		}


		//-------------------------------------------------
		public override void SetAlpha( float tintAlpha, float alphaPercent )
		{
			currentAlphaPercent = alphaPercent;
			ApplyTint( alphaPercent );
		}


		//-------------------------------------------------
		public override void UpdateVisuals()
		{
			ApplyActiveRuntimeMaterial();
			ApplyTint( currentAlphaPercent );
		}


		//-------------------------------------------------
		public void UpdateVisualsInEditor()
		{
            if (Teleport.instance == null)
                return;

			areaMesh = GetComponent<MeshRenderer>();

			if ( locked )
			{
				areaMesh.sharedMaterial = Teleport.instance.areaLockedMaterial;
			}
			else
			{
				areaMesh.sharedMaterial = Teleport.instance.areaVisibleMaterial;
			}
		}


		//-------------------------------------------------
		private void EnsureAreaMesh()
		{
			if ( areaMesh == null )
			{
				areaMesh = GetComponent<MeshRenderer>();
			}
		}


		//-------------------------------------------------
		public void ConfigureCorridorVisuals( Vector3 referenceLocalScale )
		{
			showPlayAreaPreview = false;
			CacheRuntimeMaterials();
			ApplyCorridorGridTiling( referenceLocalScale );
			UpdateVisuals();
		}


		//-------------------------------------------------
		private void ApplyCorridorGridTiling( Vector3 referenceLocalScale )
		{
			if ( corridorVisualsConfigured )
			{
				return;
			}

			MeshFilter meshFilter = GetComponent<MeshFilter>();
			if ( meshFilter == null )
			{
				return;
			}

			if ( corridorSourceMesh == null )
			{
				corridorSourceMesh = meshFilter.sharedMesh;
			}

			if ( corridorSourceMesh == null )
			{
				return;
			}

			float referenceX = Mathf.Abs( referenceLocalScale.x );
			float referenceZ = Mathf.Abs( referenceLocalScale.z );
			if ( referenceX < 0.0001f )
			{
				referenceX = 0.3315342f;
			}
			if ( referenceZ < 0.0001f )
			{
				referenceZ = 0.13949926f;
			}

			Vector2 tiling = new Vector2(
				Mathf.Max( 0.01f, Mathf.Abs( transform.localScale.x ) / referenceX ),
				Mathf.Max( 0.01f, Mathf.Abs( transform.localScale.z ) / referenceZ )
			);

			corridorTiledMesh = Instantiate( corridorSourceMesh );
			corridorTiledMesh.name = corridorSourceMesh.name + "_CorridorGrid";
			Vector2[] uvs = corridorTiledMesh.uv;
			if ( uvs == null || uvs.Length == 0 )
			{
				DestroyRuntimeMesh( ref corridorTiledMesh );
				return;
			}

			for ( int i = 0; i < uvs.Length; i++ )
			{
				uvs[i] = new Vector2( uvs[i].x * tiling.x, uvs[i].y * tiling.y );
			}

			corridorTiledMesh.uv = uvs;
			meshFilter.sharedMesh = corridorTiledMesh;
			corridorVisualsConfigured = true;
		}


		//-------------------------------------------------
		private void CacheRuntimeMaterials()
		{
			if ( runtimeVisibleMaterial != null || Teleport.instance == null )
			{
				return;
			}

			runtimeVisibleMaterial = new Material( Teleport.instance.areaVisibleMaterial );
			runtimeHighlightedMaterial = new Material( Teleport.instance.areaHighlightedMaterial );
			runtimeLockedMaterial = new Material( Teleport.instance.areaLockedMaterial );
		}


		//-------------------------------------------------
		private Material GetActiveRuntimeMaterial()
		{
			CacheRuntimeMaterials();
			if ( locked )
			{
				return runtimeLockedMaterial;
			}

			return highlighted ? runtimeHighlightedMaterial : runtimeVisibleMaterial;
		}


		//-------------------------------------------------
		private void ApplyActiveRuntimeMaterial()
		{
			EnsureAreaMesh();
			Material material = GetActiveRuntimeMaterial();
			if ( areaMesh == null || material == null )
			{
				return;
			}

			areaMesh.SetPropertyBlock( null );
			if ( areaMesh.sharedMaterial != material )
			{
				areaMesh.sharedMaterial = material;
			}
		}


		//-------------------------------------------------
		private void ApplyTint( float alphaPercent )
		{
			Material material = GetActiveRuntimeMaterial();
			if ( material == null )
			{
				return;
			}

			Color tintedColor = GetTintColor();
			tintedColor.a *= alphaPercent;
			material.SetColor( tintColorId, tintedColor );
		}


		//-------------------------------------------------
		private static void DestroyRuntimeMaterial( ref Material material )
		{
			if ( material == null )
			{
				return;
			}

			if ( Application.isPlaying )
			{
				Destroy( material );
			}
			else
			{
				DestroyImmediate( material );
			}

			material = null;
		}


		//-------------------------------------------------
		private static void DestroyRuntimeMesh( ref Mesh mesh )
		{
			if ( mesh == null )
			{
				return;
			}

			if ( Application.isPlaying )
			{
				Destroy( mesh );
			}
			else
			{
				DestroyImmediate( mesh );
			}

			mesh = null;
		}


		//-------------------------------------------------
		private bool CalculateBounds()
		{
			MeshFilter meshFilter = GetComponent<MeshFilter>();
			if ( meshFilter == null )
			{
				return false;
			}

			Mesh mesh = meshFilter.sharedMesh;
			if ( mesh == null )
			{
				return false;
			}

			meshBounds = mesh.bounds;
			return true;
		}


		//-------------------------------------------------
		private Color GetTintColor()
		{
			if ( locked )
			{
				return lockedTintColor;
			}
			else
			{
				if ( highlighted )
				{
					return highlightedTintColor;
				}
				else
				{
					return visibleTintColor;
				}
			}
		}
	}


#if UNITY_EDITOR
	//-------------------------------------------------------------------------
	[CustomEditor( typeof( TeleportArea ) )]
	public class TeleportAreaEditor : Editor
	{
		//-------------------------------------------------
		void OnEnable()
		{
			if ( Selection.activeTransform != null )
			{
				TeleportArea teleportArea = Selection.activeTransform.GetComponent<TeleportArea>();
				if ( teleportArea != null )
				{
					teleportArea.UpdateVisualsInEditor();
				}
			}
		}


		//-------------------------------------------------
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();

			if ( Selection.activeTransform != null )
			{
				TeleportArea teleportArea = Selection.activeTransform.GetComponent<TeleportArea>();
				if ( GUI.changed && teleportArea != null )
				{
					teleportArea.UpdateVisualsInEditor();
				}
			}
		}
	}
#endif
}
