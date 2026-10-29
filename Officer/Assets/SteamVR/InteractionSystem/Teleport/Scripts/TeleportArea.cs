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
		}


		//-------------------------------------------------
		private void OnDestroy()
		{
			DestroyRuntimeMaterial( ref runtimeVisibleMaterial );
			DestroyRuntimeMaterial( ref runtimeHighlightedMaterial );
			DestroyRuntimeMaterial( ref runtimeLockedMaterial );
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
			if ( locked )
			{
				return;
			}

			if ( highlighted == highlight && areaMesh != null && areaMesh.sharedMaterial == GetActiveRuntimeMaterial() )
			{
				return;
			}

			highlighted = highlight;
			ApplyActiveRuntimeMaterial();
			ApplyTint( currentAlphaPercent );
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
			if ( locked )
			{
				highlighted = false;
			}

			ApplyActiveRuntimeMaterial();
			ApplyTint( currentAlphaPercent );
		}


		//-------------------------------------------------
		public void UpdateVisualsInEditor()
		{
            if ( Application.isPlaying || Teleport.instance == null )
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
		private void CacheRuntimeMaterials()
		{
			if ( runtimeVisibleMaterial != null || Teleport.instance == null )
			{
				return;
			}

			runtimeVisibleMaterial = CreateRuntimeMaterial( Teleport.instance.areaVisibleMaterial );
			runtimeHighlightedMaterial = CreateRuntimeMaterial( Teleport.instance.areaHighlightedMaterial );
			runtimeLockedMaterial = CreateRuntimeMaterial( Teleport.instance.areaLockedMaterial );
		}


		//-------------------------------------------------
		private static Material CreateRuntimeMaterial( Material source )
		{
			if ( source == null )
			{
				return null;
			}

			Material material = new Material( source );
			material.enableInstancing = false;
			return material;
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
