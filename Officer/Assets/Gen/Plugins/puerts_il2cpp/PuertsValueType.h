// Auto Gen

#if !__SNC__
#ifndef __has_feature 
#define __has_feature(x) 0 
#endif
#endif

#if _MSC_VER
typedef wchar_t Il2CppChar;
#elif __has_feature(cxx_unicode_literals)
typedef char16_t Il2CppChar;
#else
typedef uint16_t Il2CppChar;
#endif

namespace puerts
{

// StreamingContextStates
struct i4
{
    int32_t p0;
};
    
// StreamingContext
struct S_Oi4_
{
    Il2CppObject* p0;
    int32_t p1;
};
    
// RuntimeTypeHandle
struct S_p_
{
    void* p0;
};
    
// NativeOverlapped
struct S_ppi4i4p_
{
    void* p0;
    void* p1;
    int32_t p2;
    int32_t p3;
    void* p4;
};
    
// ProcessingMode
struct u1
{
    uint8_t p0;
};
    
// DateTime
struct S_u8_
{
    uint64_t p0;
};
    
// Guid
struct S_i4i2i2u1u1u1u1u1u1u1u1_
{
    int32_t p0;
    int16_t p1;
    int16_t p2;
    uint8_t p3;
    uint8_t p4;
    uint8_t p5;
    uint8_t p6;
    uint8_t p7;
    uint8_t p8;
    uint8_t p9;
    uint8_t p10;
};
    
// AnimatorClipInfo
struct S_i4r4_
{
    int32_t p0;
    float p1;
};
    
// KeyValuePair`2
struct S_oo_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
};
    
// ConstraintSource
struct S_or4_
{
    Il2CppObject* p0;
    float p1;
};
    
// T
struct S__
{
    union
    {
        struct
        {
        };
        uint8_t __padding[1];
    };
};
    
// Vector3
struct S_r4r4r4_
{
    float p0;
    float p1;
    float p2;
};
    
// ProfilerRecorderSample
struct S_i8i8i8_
{
    int64_t p0;
    int64_t p1;
    int64_t p2;
};
    
// Vector4
struct S_r4r4r4r4_
{
    float p0;
    float p1;
    float p2;
    float p3;
};
    
// Matrix4x4
struct S_r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4_
{
    float p0;
    float p1;
    float p2;
    float p3;
    float p4;
    float p5;
    float p6;
    float p7;
    float p8;
    float p9;
    float p10;
    float p11;
    float p12;
    float p13;
    float p14;
    float p15;
};
    
// AsyncGPUReadbackRequest
struct S_pi4_
{
    void* p0;
    int32_t p1;
};
    
// SphericalHarmonicsL2
struct S_r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4_
{
    float p0;
    float p1;
    float p2;
    float p3;
    float p4;
    float p5;
    float p6;
    float p7;
    float p8;
    float p9;
    float p10;
    float p11;
    float p12;
    float p13;
    float p14;
    float p15;
    float p16;
    float p17;
    float p18;
    float p19;
    float p20;
    float p21;
    float p22;
    float p23;
    float p24;
    float p25;
    float p26;
};
    
// VertexAttributeDescriptor
struct S_i4i4i4i4_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
};
    
// Color32
struct S_i4u1u1u1u1_
{
    int32_t p0;
    uint8_t p1;
    uint8_t p2;
    uint8_t p3;
    uint8_t p4;
};
    
// Vector2
struct S_r4r4_
{
    float p0;
    float p1;
};
    
// Bounds
struct S_S_r4r4r4_S_r4r4r4__
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
};
    
// SubMeshDescriptor
struct S_S_S_r4r4r4_S_r4r4r4__i4i4i4i4i4i4_
{
    struct S_S_r4r4r4_S_r4r4r4__ p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
};
    
// BoneWeight
struct S_r4r4r4r4i4i4i4i4_
{
    float p0;
    float p1;
    float p2;
    float p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
    int32_t p7;
};
    
// RenderRequest
struct S_i4oi4_
{
    int32_t p0;
    Il2CppObject* p1;
    int32_t p2;
};
    
// CullingGroupEvent
struct S_i4u1u1_
{
    int32_t p0;
    uint8_t p1;
    uint8_t p2;
};
    
// CustomRenderTextureUpdateZone
struct S_S_r4r4r4_S_r4r4r4_r4i4b_
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    float p2;
    int32_t p3;
    bool p4;
};
    
// TimeSpan
struct S_i8_
{
    int64_t p0;
};
    
// PhraseRecognizedEventArgs
struct S_i4osS_u8_S_i8__
{
    int32_t p0;
    Il2CppObject* p1;
    Il2CppString* p2;
    struct S_u8_ p3;
    struct S_i8_ p4;
};
    
// PhotoCaptureResult
struct S_i4i8_
{
    int32_t p0;
    int64_t p1;
};
    
// AtomicSafetyHandle
struct S_pi4i4_
{
    void* p0;
    int32_t p1;
    int32_t p2;
};
    
// NativeArray`1
struct S_Pvi4i4i4S_pi4i4_i4_
{
    void* p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    struct S_pi4i4_ p4;
    int32_t p5;
};
    
// LODParameters
struct S_i4S_r4r4r4_r4r4i4_
{
    int32_t p0;
    struct S_r4r4r4_ p1;
    float p2;
    float p3;
    int32_t p4;
};
    
// BatchCullingContext
struct S_S_Pvi4i4i4S_pi4i4_i4_S_Pvi4i4i4S_pi4i4_i4_S_i4S_r4r4r4_r4r4i4_S_r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4_i4i4i4S_u8_u4u8u1i4i4_
{
    struct S_Pvi4i4i4S_pi4i4_i4_ p0;
    struct S_Pvi4i4i4S_pi4i4_i4_ p1;
    struct S_i4S_r4r4r4_r4r4i4_ p2;
    struct S_r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4_ p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
    struct S_u8_ p7;
    uint32_t p8;
    uint64_t p9;
    uint8_t p10;
    int32_t p11;
    int32_t p12;
};
    
// BatchCullingOutput
struct S_S_Pvi4i4i4S_pi4i4_i4__
{
    struct S_Pvi4i4i4S_pi4i4_i4_ p0;
};
    
// RendererList
struct S_pu4u4u4_
{
    void* p0;
    uint32_t p1;
    uint32_t p2;
    uint32_t p3;
};
    
// PlayableGraph
struct S_pu4_
{
    void* p0;
    uint32_t p1;
};
    
// Range
struct S_i4i4_
{
    int32_t p0;
    int32_t p1;
};
    
// Particle
struct S_S_r4r4r4_S_r4r4r4_S_r4r4r4_S_r4r4r4_S_r4r4r4_S_r4r4r4_S_r4r4r4_S_r4r4r4_S_i4u1u1u1u1_u4u4r4r4i4r4r4u4_
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4r4_ p2;
    struct S_r4r4r4_ p3;
    struct S_r4r4r4_ p4;
    struct S_r4r4r4_ p5;
    struct S_r4r4r4_ p6;
    struct S_r4r4r4_ p7;
    struct S_i4u1u1u1u1_ p8;
    uint32_t p9;
    uint32_t p10;
    float p11;
    float p12;
    int32_t p13;
    float p14;
    float p15;
    uint32_t p16;
};
    
// ContactPoint
struct S_S_r4r4r4_S_r4r4r4_S_r4r4r4_i4i4r4_
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4r4_ p2;
    int32_t p3;
    int32_t p4;
    float p5;
};
    
// ContactPoint2D
struct S_S_r4r4_S_r4r4_S_r4r4_r4r4r4i4i4i4i4i4_
{
    struct S_r4r4_ p0;
    struct S_r4r4_ p1;
    struct S_r4r4_ p2;
    float p3;
    float p4;
    float p5;
    int32_t p6;
    int32_t p7;
    int32_t p8;
    int32_t p9;
    int32_t p10;
};
    
// RaycastHit2D
struct S_S_r4r4_S_r4r4_S_r4r4_r4r4i4_
{
    struct S_r4r4_ p0;
    struct S_r4r4_ p1;
    struct S_r4r4_ p2;
    float p3;
    float p4;
    int32_t p5;
};
    
// PhysicsShape2D
struct S_i4r4i4i4i4i4S_r4r4_S_r4r4__
{
    int32_t p0;
    float p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    struct S_r4r4_ p6;
    struct S_r4r4_ p7;
};
    
// PropertyPathPart
struct S_i4si4O_
{
    int32_t p0;
    Il2CppString* p1;
    int32_t p2;
    Il2CppObject* p3;
};
    
// GlyphMetrics
struct S_r4r4r4r4r4_
{
    float p0;
    float p1;
    float p2;
    float p3;
    float p4;
};
    
// MultipleSubstitutionRecord
struct S_u4o_
{
    uint32_t p0;
    Il2CppObject* p1;
};
    
// LigatureSubstitutionRecord
struct S_ou4_
{
    Il2CppObject* p0;
    uint32_t p1;
};
    
// GlyphAdjustmentRecord
struct S_u4S_r4r4r4r4__
{
    uint32_t p0;
    struct S_r4r4r4r4_ p1;
};
    
// GlyphPairAdjustmentRecord
struct S_S_u4S_r4r4r4r4__S_u4S_r4r4r4r4__i4_
{
    struct S_u4S_r4r4r4r4__ p0;
    struct S_u4S_r4r4r4r4__ p1;
    int32_t p2;
};
    
// MarkToBaseAdjustmentRecord
struct S_u4S_r4r4_u4S_r4r4__
{
    uint32_t p0;
    struct S_r4r4_ p1;
    uint32_t p2;
    struct S_r4r4_ p3;
};
    
// FontAssetCreationEditorSettings
struct S_si4i4i4i4i4i4i4i4i4sssi4r4i4b_
{
    Il2CppString* p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
    int32_t p7;
    int32_t p8;
    int32_t p9;
    Il2CppString* p10;
    Il2CppString* p11;
    Il2CppString* p12;
    int32_t p13;
    float p14;
    int32_t p15;
    bool p16;
};
    
// FaceInfo
struct S_i4ssi4r4i4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4_
{
    int32_t p0;
    Il2CppString* p1;
    Il2CppString* p2;
    int32_t p3;
    float p4;
    int32_t p5;
    float p6;
    float p7;
    float p8;
    float p9;
    float p10;
    float p11;
    float p12;
    float p13;
    float p14;
    float p15;
    float p16;
    float p17;
    float p18;
    float p19;
    float p20;
};
    
// UIVertex
struct S_S_r4r4r4_S_r4r4r4_S_r4r4r4r4_S_i4u1u1u1u1_S_r4r4r4r4_S_r4r4r4r4_S_r4r4r4r4_S_r4r4r4r4__
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4r4r4_ p2;
    struct S_i4u1u1u1u1_ p3;
    struct S_r4r4r4r4_ p4;
    struct S_r4r4r4r4_ p5;
    struct S_r4r4r4r4_ p6;
    struct S_r4r4r4r4_ p7;
};
    
// UICharInfo
struct S_S_r4r4_r4_
{
    struct S_r4r4_ p0;
    float p1;
};
    
// UILineInfo
struct S_i4i4r4r4_
{
    int32_t p0;
    int32_t p1;
    float p2;
    float p3;
};
    
// KeyValuePair`2
struct S_S_i4i4_O_
{
    struct S_i4i4_ p0;
    Il2CppObject* p1;
};
    
// TimeValue
struct S_r4i4_
{
    float p0;
    int32_t p1;
};
    
// StylePropertyName
struct S_i4s_
{
    int32_t p0;
    Il2CppString* p1;
};
    
// EasingFunction
struct S_i4_
{
    int32_t p0;
};
    
// StylePropertyValue
struct S_oS_i4i4__
{
    Il2CppObject* p0;
    struct S_i4i4_ p1;
};
    
// StyleVariable
struct S_soo_
{
    Il2CppString* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
};
    
// Page
struct S_S_Pvi4i4i4S_pi4i4_i4_i4_
{
    struct S_Pvi4i4i4S_pi4i4_i4_ p0;
    int32_t p1;
};
    
// NativeSlice`1
struct S_Pvi4i4i4i4S_pi4i4__
{
    void* p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    struct S_pi4i4_ p5;
};
    
// BMPAlloc
struct S_i4u2u1u1_
{
    int32_t p0;
    uint16_t p1;
    uint8_t p2;
    uint8_t p3;
};
    
// RenderChainVEData
struct S_ooooooi4i4i4u4oooobbbbbbi4i4i4booS_r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4_i4i4S_i4u2u1u1_S_i4u2u1u1_S_i4u2u1u1_S_i4u2u1u1_S_i4u2u1u1_S_i4u2u1u1_S_i4u2u1u1_S_i4u2u1u1_S_i4u2u1u1_S_i4u2u1u1_S_i4u2u1u1_r4S_r4r4r4r4_o_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    Il2CppObject* p3;
    Il2CppObject* p4;
    Il2CppObject* p5;
    int32_t p6;
    int32_t p7;
    int32_t p8;
    uint32_t p9;
    Il2CppObject* p10;
    Il2CppObject* p11;
    Il2CppObject* p12;
    Il2CppObject* p13;
    bool p14;
    bool p15;
    bool p16;
    bool p17;
    bool p18;
    bool p19;
    int32_t p20;
    int32_t p21;
    int32_t p22;
    bool p23;
    Il2CppObject* p24;
    Il2CppObject* p25;
    struct S_r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4_ p26;
    int32_t p27;
    int32_t p28;
    struct S_i4u2u1u1_ p29;
    struct S_i4u2u1u1_ p30;
    struct S_i4u2u1u1_ p31;
    struct S_i4u2u1u1_ p32;
    struct S_i4u2u1u1_ p33;
    struct S_i4u2u1u1_ p34;
    struct S_i4u2u1u1_ p35;
    struct S_i4u2u1u1_ p36;
    struct S_i4u2u1u1_ p37;
    struct S_i4u2u1u1_ p38;
    struct S_i4u2u1u1_ p39;
    float p40;
    struct S_r4r4r4r4_ p41;
    Il2CppObject* p42;
};
    
// StyleDataRef`1
struct S_o_
{
    Il2CppObject* p0;
};
    
// ComputedStyle
struct S_S_o_S_o_S_o_S_o_S_o_S_o_ooi8r4o_
{
    struct S_o_ p0;
    struct S_o_ p1;
    struct S_o_ p2;
    struct S_o_ p3;
    struct S_o_ p4;
    struct S_o_ p5;
    Il2CppObject* p6;
    Il2CppObject* p7;
    int64_t p8;
    float p9;
    Il2CppObject* p10;
};
    
// PanelClearSettings
struct S_bbS_r4r4r4r4__
{
    bool p0;
    bool p1;
    struct S_r4r4r4r4_ p2;
};
    
// UsingEntry
struct S_sso_
{
    Il2CppString* p0;
    Il2CppString* p1;
    Il2CppObject* p2;
};
    
// AttributeOverride
struct S_sss_
{
    Il2CppString* p0;
    Il2CppString* p1;
    Il2CppString* p2;
};
    
// SlotUsageEntry
struct S_si4_
{
    Il2CppString* p0;
    int32_t p1;
};
    
// UxmlObjectEntry
struct S_i4o_
{
    int32_t p0;
    Il2CppObject* p1;
};
    
// AssetEntry
struct S_ssoo_
{
    Il2CppString* p0;
    Il2CppString* p1;
    Il2CppObject* p2;
    Il2CppObject* p3;
};
    
// TimerState
struct S_i8i8_
{
    int64_t p0;
    int64_t p1;
};
    
// TextureInfo
struct S_obi4_
{
    Il2CppObject* p0;
    bool p1;
    int32_t p2;
};
    
// StyleValueManaged
struct S_i4i4O_
{
    int32_t p0;
    int32_t p1;
    Il2CppObject* p2;
};
    
// BackgroundPosition
struct S_i4S_r4i4__
{
    int32_t p0;
    struct S_r4i4_ p1;
};
    
// StyleValue
struct S_i4i4r4S_r4i4_S_r4r4r4r4_S_p_S_i4S_r4i4__S_i4i4__
{
    int32_t p0;
    int32_t p1;
    float p2;
    struct S_r4i4_ p3;
    struct S_r4r4r4r4_ p4;
    struct S_p_ p5;
    struct S_i4S_r4i4__ p6;
    struct S_i4i4_ p7;
};
    
// CanStartDragArgs
struct S_oi4o_
{
    Il2CppObject* p0;
    int32_t p1;
    Il2CppObject* p2;
};
    
// StartDragArgs
struct S_si4oo_
{
    Il2CppString* p0;
    int32_t p1;
    Il2CppObject* p2;
    Il2CppObject* p3;
};
    
// SetupDragAndDropArgs
struct S_ooS_si4oo__
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    struct S_si4oo_ p2;
};
    
// DragAndDropArgs
struct S_Oi4i4i4i4o_
{
    Il2CppObject* p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    Il2CppObject* p5;
};
    
// HandleDragAndDropArgs
struct S_S_Oi4i4i4i4o_S_r4r4__
{
    struct S_Oi4i4i4i4o_ p0;
    struct S_r4r4_ p1;
};
    
// TreeItem
struct S_i4i4o_
{
    int32_t p0;
    int32_t p1;
    Il2CppObject* p2;
};
    
// TreeViewItemWrapper
struct S_S_i4i4o_i4_
{
    struct S_i4i4o_ p0;
    int32_t p1;
};
    
// Background
struct S_oooo_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    Il2CppObject* p3;
};
    
// SortedColumnState
struct S_oi4_
{
    Il2CppObject* p0;
    int32_t p1;
};
    
// ColumnState
struct S_i4sr4r4b_
{
    int32_t p0;
    Il2CppString* p1;
    float p2;
    float p3;
    bool p4;
};
    
// ManipulatorActivationFilter
struct S_i4i4i4_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
};
    
// VFXExposedProperty
struct S_so_
{
    Il2CppString* p0;
    Il2CppObject* p1;
};
    
// InputDevice
struct S_u8b_
{
    uint64_t p0;
    bool p1;
};
    
// InputFeatureType
struct u4
{
    uint32_t p0;
};
    
// InputFeatureUsage
struct S_su4_
{
    Il2CppString* p0;
    uint32_t p1;
};
    
// Bone
struct S_u8u4_
{
    uint64_t p0;
    uint32_t p1;
};
    
// MeshId
struct S_u8u8_
{
    uint64_t p0;
    uint64_t p1;
};
    
// MeshInfo
struct S_S_u8u8_i4i4_
{
    struct S_u8u8_ p0;
    int32_t p1;
    int32_t p2;
};
    
// MeshGenerationResult
struct S_S_u8u8_ooi4i4u8S_r4r4r4_S_r4r4r4r4_S_r4r4r4__
{
    struct S_u8u8_ p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    int32_t p3;
    int32_t p4;
    uint64_t p5;
    struct S_r4r4r4_ p6;
    struct S_r4r4r4r4_ p7;
    struct S_r4r4r4_ p8;
};
    
// DataModeChangeEventArgs
struct S_i4b_
{
    int32_t p0;
    bool p1;
};
    
// EventInterests
struct S_bbb_
{
    bool p0;
    bool p1;
    bool p2;
};
    
// Nullable`1
struct N_bi4_
{
    bool hasValue;
    int32_t p1;
};
    
// MarkerFlags
struct u2
{
    uint16_t p0;
};
    
// MarkerInfo
struct S_i4u2u2so_
{
    int32_t p0;
    uint16_t p1;
    uint16_t p2;
    Il2CppString* p3;
    Il2CppObject* p4;
};
    
// ProfilerCategoryInfo
struct S_u2S_i4u1u1u1u1_su2_
{
    uint16_t p0;
    struct S_i4u1u1u1u1_ p1;
    Il2CppString* p2;
    uint16_t p3;
};
    
// ProfilerCounterData
struct S_ss_
{
    Il2CppString* p0;
    Il2CppString* p1;
};
    
// LabelLayoutData
struct S_S_r4r4r4r4_r4_
{
    struct S_r4r4r4r4_ p0;
    float p1;
};
    
// EditorCurveBinding
struct S_sosi4i4i4i4i4i4i4_
{
    Il2CppString* p0;
    Il2CppObject* p1;
    Il2CppString* p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
    int32_t p7;
    int32_t p8;
    int32_t p9;
};
    
// Nullable`1
struct N_bS_sosi4i4i4i4i4i4i4__
{
    bool hasValue;
    struct S_sosi4i4i4i4i4i4i4_ p1;
};
    
// Nullable`1
struct N_br4_
{
    bool hasValue;
    float p1;
};
    
// Keyframe
struct S_r4r4r4r4i4i4r4r4_
{
    float p0;
    float p1;
    float p2;
    float p3;
    int32_t p4;
    int32_t p5;
    float p6;
    float p7;
};
    
// DrawElement
struct S_S_r4r4r4r4_S_r4r4r4r4_o_
{
    struct S_r4r4r4r4_ p0;
    struct S_r4r4r4r4_ p1;
    Il2CppObject* p2;
};
    
// CameraMode
struct S_i4ss_
{
    int32_t p0;
    Il2CppString* p1;
    Il2CppString* p2;
};
    
// TextureResource
struct S_r4s_
{
    float p0;
    Il2CppString* p1;
};
    
// CommandHint
struct i8
{
    int64_t p0;
};
    
// ShortcutBindingChangedEventArgs
struct S_sS_o_S_o__
{
    Il2CppString* p0;
    struct S_o_ p1;
    struct S_o_ p2;
};
    
// GUID
struct S_u4u4u4u4_
{
    uint32_t p0;
    uint32_t p1;
    uint32_t p2;
    uint32_t p3;
};
    
// GlobalObjectId
struct S_S_u8u8_S_u4u4u4u4_i4_
{
    struct S_u8u8_ p0;
    struct S_u4u4u4u4_ p1;
    int32_t p2;
};
    
// ObjectSelectorTargetInfo
struct S_S_S_u8u8_S_u4u4u4u4_i4_oo_
{
    struct S_S_u8u8_S_u4u4u4u4_i4_ p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
};
    
// FlowEvent
struct S_i4u4u1_
{
    int32_t p0;
    uint32_t p1;
    uint8_t p2;
};
    
// BuildStartedMessageRaw
struct S_i4i4S_i4__
{
    int32_t p0;
    int32_t p1;
    struct S_i4_ p2;
};
    
// BuildStartedMessage
struct S_S_i4i4S_i4___
{
    struct S_i4i4S_i4__ p0;
};
    
// NodeInfoMessageRaw
struct S_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4__
{
    int32_t p0;
    struct S_i4_ p1;
    struct S_i4_ p2;
    struct S_i4_ p3;
    struct S_i4_ p4;
    struct S_i4_ p5;
    struct S_i4_ p6;
};
    
// NodeInfo
struct S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4___
{
    Il2CppObject* p0;
    struct S_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4__ p1;
};
    
// Nullable`1
struct N_bS_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4____
{
    bool hasValue;
    struct S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4___ p1;
};
    
// NodeEnqueuedMessage
struct S_S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4___N_bS_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4_____
{
    struct S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4___ p0;
    struct N_bS_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4____ p1;
};
    
// NodeStartedMessage
struct S_S_i4i4_S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4___i4_
{
    struct S_i4i4_ p0;
    struct S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4___ p1;
    int32_t p2;
};
    
// NodeUpToDateMessage
struct S_S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4____
{
    struct S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4___ p0;
};
    
// NodeFinishedMessageRaw
struct S_i4i4i4i4S_i4_S_i4__
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    struct S_i4_ p4;
    struct S_i4_ p5;
};
    
// NodeFinishedMessage
struct S_oS_i4i4i4i4S_i4_S_i4__S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4____
{
    Il2CppObject* p0;
    struct S_i4i4i4i4S_i4_S_i4__ p1;
    struct S_oS_i4S_i4_S_i4_S_i4_S_i4_S_i4_S_i4___ p2;
};
    
// BuildFinishedMessageRaw
struct S_i4S_i4__
{
    int32_t p0;
    struct S_i4_ p1;
};
    
// BuildFinishedMessage
struct S_oS_i4S_i4___
{
    Il2CppObject* p0;
    struct S_i4S_i4__ p1;
};
    
// RPCActionMessageRaw
struct S_i4S_i4_S_i4__
{
    int32_t p0;
    struct S_i4_ p1;
    struct S_i4_ p2;
};
    
// RPCActionMessage
struct S_oS_i4S_i4_S_i4___
{
    Il2CppObject* p0;
    struct S_i4S_i4_S_i4__ p1;
};
    
// UnityVersion
struct S_bi4i4i4i4i4s_
{
    bool p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    Il2CppString* p6;
};
    
// SemVersion
struct S_bi4i4i4ss_
{
    bool p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    Il2CppString* p4;
    Il2CppString* p5;
};
    
// SerializedDependency
struct S_sosi4_
{
    Il2CppString* p0;
    Il2CppObject* p1;
    Il2CppString* p2;
    int32_t p3;
};
    
// ObjectIdentifier
struct S_S_u4u4u4u4_i8i4s_
{
    struct S_u4u4u4u4_ p0;
    int64_t p1;
    int32_t p2;
    Il2CppString* p3;
};
    
// PostprocessorInfo
struct S_sob_
{
    Il2CppString* p0;
    Il2CppObject* p1;
    bool p2;
};
    
// ChildAnimatorState
struct S_oS_r4r4r4__
{
    Il2CppObject* p0;
    struct S_r4r4r4_ p1;
};
    
// TouchEvent
struct S_i4S_r4r4_i4_
{
    int32_t p0;
    struct S_r4r4_ p1;
    int32_t p2;
};
    
// NodeCreationContext
struct S_S_r4r4_oi4_
{
    struct S_r4r4_ p0;
    Il2CppObject* p1;
    int32_t p2;
};
    
// GraphViewChange
struct S_oooS_r4r4__
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    struct S_r4r4_ p3;
};
    
// Line2
struct S_S_r4r4_S_r4r4__
{
    struct S_r4r4_ p0;
    struct S_r4r4_ p1;
};
    
// GraphViewChoice
struct S_ooi4b_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    int32_t p2;
    bool p3;
};
    
// SearchExpressionContext
struct S_S_oooo_ooi4_
{
    struct S_oooo_ p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    int32_t p3;
};
    
// SearchColumnEventArgs
struct S_oooObS_r4r4r4r4_bb_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    Il2CppObject* p3;
    bool p4;
    struct S_r4r4r4r4_ p5;
    bool p6;
    bool p7;
};
    
// SearchColumnCompareArgs
struct S_S_oooObS_r4r4r4r4_bb_S_oooObS_r4r4r4r4_bb_b_
{
    struct S_oooObS_r4r4r4r4_bb_ p0;
    struct S_oooObS_r4r4r4r4_bb_ p1;
    bool p2;
};
    
// StringView
struct S_si4i4_
{
    Il2CppString* p0;
    int32_t p1;
    int32_t p2;
};
    
// SearchExpressionEvaluator
struct S_sssoi4_
{
    Il2CppString* p0;
    Il2CppString* p1;
    Il2CppString* p2;
    Il2CppObject* p3;
    int32_t p4;
};
    
// SearchSessionContext
struct S_ooS_u8u8__
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    struct S_u8u8_ p2;
};
    
// QueryFilterOperator
struct S_osi4o_
{
    Il2CppObject* p0;
    Il2CppString* p1;
    int32_t p2;
    Il2CppObject* p3;
};
    
// QueryTokenHandler
struct S_ooi4i4o_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    int32_t p2;
    int32_t p3;
    Il2CppObject* p4;
};
    
// SearchIndexEntry
struct S_i8i4u1i4r8i4o_
{
    int64_t p0;
    int32_t p1;
    uint8_t p2;
    int32_t p3;
    double p4;
    int32_t p5;
    Il2CppObject* p6;
};
    
// SearchDocument
struct S_si4ssi4_
{
    Il2CppString* p0;
    int32_t p1;
    Il2CppString* p2;
    Il2CppString* p3;
    int32_t p4;
};
    
// PropertyDatabaseRecordKey
struct S_u8S_u8u8__
{
    uint64_t p0;
    struct S_u8u8_ p1;
};
    
// PropertyDatabaseVolatileRecordValue
struct S_O_
{
    Il2CppObject* p0;
};
    
// PropertyDatabaseVolatileRecord
struct S_S_u8S_u8u8__bS_O__
{
    struct S_u8S_u8u8__ p0;
    bool p1;
    struct S_O_ p2;
};
    
// PropertyDatabaseRecordValue
struct S_u1u4u4u4u4u4u4u4u4_
{
    uint8_t p0;
    uint32_t p1;
    uint32_t p2;
    uint32_t p3;
    uint32_t p4;
    uint32_t p5;
    uint32_t p6;
    uint32_t p7;
    uint32_t p8;
};
    
// PropertyDatabaseRecord
struct S_S_u8S_u8u8__u1S_u1u4u4u4u4u4u4u4u4__
{
    struct S_u8S_u8u8__ p0;
    uint8_t p1;
    struct S_u1u4u4u4u4u4u4u4u4_ p2;
};
    
// PropertyStringTableHeader
struct S_i4i4i4i4i4_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
};
    
// ParsingState
struct S_oi4i4oboooi4i4oi4i4sobboi4bb_
{
    Il2CppObject* p0;
    int32_t p1;
    int32_t p2;
    Il2CppObject* p3;
    bool p4;
    Il2CppObject* p5;
    Il2CppObject* p6;
    Il2CppObject* p7;
    int32_t p8;
    int32_t p9;
    Il2CppObject* p10;
    int32_t p11;
    int32_t p12;
    Il2CppString* p13;
    Il2CppObject* p14;
    bool p15;
    bool p16;
    Il2CppObject* p17;
    int32_t p18;
    bool p19;
    bool p20;
};
    
// Memory`1
struct S_Oi4i4_
{
    Il2CppObject* p0;
    int32_t p1;
    int32_t p2;
};
    
// ArraySegment`1
struct S_oi4i4_
{
    Il2CppObject* p0;
    int32_t p1;
    int32_t p2;
};
    
// SslApplicationProtocol
struct S_S_Oi4i4__
{
    struct S_Oi4i4_ p0;
};
    
// Flags
struct u8
{
    uint64_t p0;
};
    
// AuthorizationState
struct S_obbi4_
{
    Il2CppObject* p0;
    bool p1;
    bool p2;
    int32_t p3;
};
    
// MetadataToken
struct S_u4_
{
    uint32_t p0;
};
    
// Navigation
struct S_i4boooo_
{
    int32_t p0;
    bool p1;
    Il2CppObject* p2;
    Il2CppObject* p3;
    Il2CppObject* p4;
    Il2CppObject* p5;
};
    
// ColorBlock
struct S_S_r4r4r4r4_S_r4r4r4r4_S_r4r4r4r4_S_r4r4r4r4_S_r4r4r4r4_r4r4_
{
    struct S_r4r4r4r4_ p0;
    struct S_r4r4r4r4_ p1;
    struct S_r4r4r4r4_ p2;
    struct S_r4r4r4r4_ p3;
    struct S_r4r4r4r4_ p4;
    float p5;
    float p6;
};
    
// RaycastResult
struct S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    float p2;
    float p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
    int32_t p7;
    int32_t p8;
    struct S_r4r4r4_ p9;
    struct S_r4r4r4_ p10;
    struct S_r4r4_ p11;
    int32_t p12;
};
    
// FontAssetCreationSettings
struct S_ssi4i4i4i4i4i4i4sssi4r4i4b_
{
    Il2CppString* p0;
    Il2CppString* p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
    int32_t p7;
    int32_t p8;
    Il2CppString* p9;
    Il2CppString* p10;
    Il2CppString* p11;
    int32_t p12;
    float p13;
    int32_t p14;
    bool p15;
};
    
// InteractionLayerMask
struct S_u4i4_
{
    uint32_t p0;
    int32_t p1;
};
    
// Pose
struct S_S_r4r4r4_S_r4r4r4r4__
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4r4_ p1;
};
    
// PointerEventArgs
struct S_i4u4r4o_
{
    int32_t p0;
    uint32_t p1;
    float p2;
    Il2CppObject* p3;
};
    
// RenderModel_ControllerMode_State_t
struct S_b_
{
    bool p0;
};
    
// HmdMatrix34_t
struct S_r4r4r4r4r4r4r4r4r4r4r4r4_
{
    float p0;
    float p1;
    float p2;
    float p3;
    float p4;
    float p5;
    float p6;
    float p7;
    float p8;
    float p9;
    float p10;
    float p11;
};
    
// RenderModel_ComponentState_t
struct S_S_r4r4r4r4r4r4r4r4r4r4r4r4_S_r4r4r4r4r4r4r4r4r4r4r4r4_u4_
{
    struct S_r4r4r4r4r4r4r4r4r4r4r4r4_ p0;
    struct S_r4r4r4r4r4r4r4r4r4r4r4r4_ p1;
    uint32_t p2;
};
    
// VRControllerState_t
struct S_u4u8u8S_r4r4_S_r4r4_S_r4r4_S_r4r4_S_r4r4__
{
    uint32_t p0;
    uint64_t p1;
    uint64_t p2;
    struct S_r4r4_ p3;
    struct S_r4r4_ p4;
    struct S_r4r4_ p5;
    struct S_r4r4_ p6;
    struct S_r4r4_ p7;
};
    
// AttachedObject
struct S_oooi4bbobi4i4S_r4r4r4_S_r4r4r4r4_ooS_r4r4r4_S_r4r4r4r4_r4o_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    int32_t p3;
    bool p4;
    bool p5;
    Il2CppObject* p6;
    bool p7;
    int32_t p8;
    int32_t p9;
    struct S_r4r4r4_ p10;
    struct S_r4r4r4r4_ p11;
    Il2CppObject* p12;
    Il2CppObject* p13;
    struct S_r4r4r4_ p14;
    struct S_r4r4r4r4_ p15;
    float p16;
    Il2CppObject* p17;
};
    
// Entry
struct S_i8i8o_
{
    int64_t p0;
    int64_t p1;
    Il2CppObject* p2;
};
    
// IntervalTreeNode
struct S_i8i4i4i4i4_
{
    int64_t p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
};
    
// Nullable`1
struct N_bb_
{
    bool hasValue;
    bool p1;
};
    
// MarkerList
struct S_oobb_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    bool p2;
    bool p3;
};
    
// InteractionState
struct S_r4bbb_
{
    float p0;
    bool p1;
    bool p2;
    bool p3;
};
    
// SamplePoint
struct S_S_r4r4r4_r4_
{
    struct S_r4r4r4_ p0;
    float p1;
};
    
// InputStateBlock
struct S_S_i4_u4u4u4_
{
    struct S_i4_ p0;
    uint32_t p1;
    uint32_t p2;
    uint32_t p3;
};
    
// PrimitiveValue
struct S_i4bcu1i1i2u2i4u4i8u8r4r8_
{
    int32_t p0;
    bool p1;
    Il2CppChar p2;
    uint8_t p3;
    int8_t p4;
    int16_t p5;
    uint16_t p6;
    int32_t p7;
    uint32_t p8;
    int64_t p9;
    uint64_t p10;
    float p11;
    double p12;
};
    
// NamedValue
struct S_sS_i4bcu1i1i2u2i4u4i8u8r4r8__
{
    Il2CppString* p0;
    struct S_i4bcu1i1i2u2i4u4i8u8r4r8_ p1;
};
    
// NameAndParameters
struct S_sS_oi4i4__
{
    Il2CppString* p0;
    struct S_oi4i4_ p1;
};
    
// InputBinding
struct S_sssssssi4sss_
{
    Il2CppString* p0;
    Il2CppString* p1;
    Il2CppString* p2;
    Il2CppString* p3;
    Il2CppString* p4;
    Il2CppString* p5;
    Il2CppString* p6;
    int32_t p7;
    Il2CppString* p8;
    Il2CppString* p9;
    Il2CppString* p10;
};
    
// InputDeviceDescription
struct S_sssssss_
{
    Il2CppString* p0;
    Il2CppString* p1;
    Il2CppString* p2;
    Il2CppString* p3;
    Il2CppString* p4;
    Il2CppString* p5;
    Il2CppString* p6;
};
    
// Nullable`1
struct N_bS_sssssssi4sss__
{
    bool hasValue;
    struct S_sssssssi4sss_ p1;
};
    
// InlinedArray`1
struct S_i4oo_
{
    int32_t p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
};
    
// CallbackArray`1
struct S_bS_i4oo_S_i4oo_S_i4oo__
{
    bool p0;
    struct S_i4oo_ p1;
    struct S_i4oo_ p2;
    struct S_i4oo_ p3;
};
    
// DeviceArray
struct S_bi4o_
{
    bool p0;
    int32_t p1;
    Il2CppObject* p2;
};
    
// ImplementationData
struct S_oobr4S_r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    bool p2;
    float p3;
    struct S_r4r4_ p4;
    struct S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ p5;
    Il2CppObject* p6;
    Il2CppObject* p7;
    Il2CppObject* p8;
};
    
// TouchModel
struct S_i4i4bS_r4r4_i4S_r4r4_S_oobr4S_r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo__
{
    int32_t p0;
    int32_t p1;
    bool p2;
    struct S_r4r4_ p3;
    int32_t p4;
    struct S_r4r4_ p5;
    struct S_oobr4S_r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo_ p6;
};
    
// RegisteredTouch
struct S_bi4S_i4i4bS_r4r4_i4S_r4r4_S_oobr4S_r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo___
{
    bool p0;
    int32_t p1;
    struct S_i4i4bS_r4r4_i4S_r4r4_S_oobr4S_r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo__ p2;
};
    
// ImplementationData
struct S_oobr4S_r4r4_S_r4r4_S_r4r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    bool p2;
    float p3;
    struct S_r4r4_ p4;
    struct S_r4r4_ p5;
    struct S_r4r4r4_ p6;
    struct S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ p7;
    Il2CppObject* p8;
    Il2CppObject* p9;
    Il2CppObject* p10;
};
    
// TrackedDeviceModel
struct S_S_oobr4S_r4r4_S_r4r4_S_r4r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo_i4bi4bS_r4r4r4_oS_r4r4r4r4_oS_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_i4S_i4_S_r4r4_r4i4obr4_
{
    struct S_oobr4S_r4r4_S_r4r4_S_r4r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo_ p0;
    int32_t p1;
    bool p2;
    int32_t p3;
    bool p4;
    struct S_r4r4r4_ p5;
    Il2CppObject* p6;
    struct S_r4r4r4r4_ p7;
    Il2CppObject* p8;
    struct S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ p9;
    int32_t p10;
    struct S_i4_ p11;
    struct S_r4r4_ p12;
    float p13;
    int32_t p14;
    Il2CppObject* p15;
    bool p16;
    float p17;
};
    
// RegisteredInteractor
struct S_oS_S_oobr4S_r4r4_S_r4r4_S_r4r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo_i4bi4bS_r4r4r4_oS_r4r4r4r4_oS_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_i4S_i4_S_r4r4_r4i4obr4__
{
    Il2CppObject* p0;
    struct S_S_oobr4S_r4r4_S_r4r4_S_r4r4r4_S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooo_i4bi4bS_r4r4r4_oS_r4r4r4r4_oS_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_i4S_i4_S_r4r4_r4i4obr4_ p1;
};
    
// RenderTargetIdentifier
struct S_i4i4i4pi4i4i4_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    void* p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
};
    
// DistortionCoordinates_t
struct S_r4r4r4r4r4r4_
{
    float p0;
    float p1;
    float p2;
    float p3;
    float p4;
    float p5;
};
    
// TrackedDevicePose_t
struct S_S_r4r4r4r4r4r4r4r4r4r4r4r4_S_r4r4r4_S_r4r4r4_i4bb_
{
    struct S_r4r4r4r4r4r4r4r4r4r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4r4_ p2;
    int32_t p3;
    bool p4;
    bool p5;
};
    
// VREvent_Reserved_t
struct S_u8u8u8u8u8u8_
{
    uint64_t p0;
    uint64_t p1;
    uint64_t p2;
    uint64_t p3;
    uint64_t p4;
    uint64_t p5;
};
    
// VREvent_Mouse_t
struct S_r4r4u4u4_
{
    float p0;
    float p1;
    uint32_t p2;
    uint32_t p3;
};
    
// VREvent_Scroll_t
struct S_r4r4u4r4u4_
{
    float p0;
    float p1;
    uint32_t p2;
    float p3;
    uint32_t p4;
};
    
// VREvent_Process_t
struct S_u4u4bb_
{
    uint32_t p0;
    uint32_t p1;
    bool p2;
    bool p3;
};
    
// VREvent_Overlay_t
struct S_u8u8u8u4_
{
    uint64_t p0;
    uint64_t p1;
    uint64_t p2;
    uint32_t p3;
};
    
// VREvent_Ipd_t
struct S_r4_
{
    float p0;
};
    
// VREvent_TouchPadMove_t
struct S_br4r4r4r4r4_
{
    bool p0;
    float p1;
    float p2;
    float p3;
    float p4;
    float p5;
};
    
// VREvent_Screenshot_t
struct S_u4u4_
{
    uint32_t p0;
    uint32_t p1;
};
    
// VREvent_Property_t
struct S_u8i4_
{
    uint64_t p0;
    int32_t p1;
};
    
// VREvent_HapticVibration_t
struct S_u8u8r4r4r4_
{
    uint64_t p0;
    uint64_t p1;
    float p2;
    float p3;
    float p4;
};
    
// VREvent_InputBindingLoad_t
struct S_u8u8u8u8_
{
    uint64_t p0;
    uint64_t p1;
    uint64_t p2;
    uint64_t p3;
};
    
// VREvent_ProgressUpdate_t
struct S_u8u8u8u8u8r4_
{
    uint64_t p0;
    uint64_t p1;
    uint64_t p2;
    uint64_t p3;
    uint64_t p4;
    float p5;
};
    
// VREvent_Keyboard_t
struct S_u1u1u1u1u1u1u1u1u8u8_
{
    uint8_t p0;
    uint8_t p1;
    uint8_t p2;
    uint8_t p3;
    uint8_t p4;
    uint8_t p5;
    uint8_t p6;
    uint8_t p7;
    uint64_t p8;
    uint64_t p9;
};
    
// VREvent_Data_t
struct S_S_u8u8u8u8u8u8_S_u4_S_r4r4u4u4_S_r4r4u4r4u4_S_u4u4bb_S_u8u4_S_u8u8u8u4_S_u4_S_r4_S_u8u8_S_u4_S_br4r4r4r4r4_S_b_S_u4u4_S_r4_S_u4u4_S_u8u4_S_u4_S_u8i4_S_u8u8r4r4r4_S_u8_S_u8u8u8u8_S_u4_S_u8u8u8u8_S_u8u8u8u8u8r4_S_i4_S_i4_S_i4_S_u1u1u1u1u1u1u1u1u8u8__
{
    struct S_u8u8u8u8u8u8_ p0;
    struct S_u4_ p1;
    struct S_r4r4u4u4_ p2;
    struct S_r4r4u4r4u4_ p3;
    struct S_u4u4bb_ p4;
    struct S_u8u4_ p5;
    struct S_u8u8u8u4_ p6;
    struct S_u4_ p7;
    struct S_r4_ p8;
    struct S_u8u8_ p9;
    struct S_u4_ p10;
    struct S_br4r4r4r4r4_ p11;
    struct S_b_ p12;
    struct S_u4u4_ p13;
    struct S_r4_ p14;
    struct S_u4u4_ p15;
    struct S_u8u4_ p16;
    struct S_u4_ p17;
    struct S_u8i4_ p18;
    struct S_u8u8r4r4r4_ p19;
    struct S_u8_ p20;
    struct S_u8u8u8u8_ p21;
    struct S_u4_ p22;
    struct S_u8u8u8u8_ p23;
    struct S_u8u8u8u8u8r4_ p24;
    struct S_i4_ p25;
    struct S_i4_ p26;
    struct S_i4_ p27;
    struct S_u1u1u1u1u1u1u1u1u8u8_ p28;
};
    
// VREvent_t
struct S_u4u4r4S_S_u8u8u8u8u8u8_S_u4_S_r4r4u4u4_S_r4r4u4r4u4_S_u4u4bb_S_u8u4_S_u8u8u8u4_S_u4_S_r4_S_u8u8_S_u4_S_br4r4r4r4r4_S_b_S_u4u4_S_r4_S_u4u4_S_u8u4_S_u4_S_u8i4_S_u8u8r4r4r4_S_u8_S_u8u8u8u8_S_u4_S_u8u8u8u8_S_u8u8u8u8u8r4_S_i4_S_i4_S_i4_S_u1u1u1u1u1u1u1u1u8u8___
{
    uint32_t p0;
    uint32_t p1;
    float p2;
    struct S_S_u8u8u8u8u8u8_S_u4_S_r4r4u4u4_S_r4r4u4r4u4_S_u4u4bb_S_u8u4_S_u8u8u8u4_S_u4_S_r4_S_u8u8_S_u4_S_br4r4r4r4r4_S_b_S_u4u4_S_r4_S_u4u4_S_u8u4_S_u4_S_u8i4_S_u8u8r4r4r4_S_u8_S_u8u8u8u8_S_u4_S_u8u8u8u8_S_u8u8u8u8u8r4_S_i4_S_i4_S_i4_S_u1u1u1u1u1u1u1u1u8u8__ p3;
};
    
// Compositor_FrameTiming
struct S_u4u4u4u4u4u4r8r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4S_S_r4r4r4r4r4r4r4r4r4r4r4r4_S_r4r4r4_S_r4r4r4_i4bb_u4u4_
{
    uint32_t p0;
    uint32_t p1;
    uint32_t p2;
    uint32_t p3;
    uint32_t p4;
    uint32_t p5;
    double p6;
    float p7;
    float p8;
    float p9;
    float p10;
    float p11;
    float p12;
    float p13;
    float p14;
    float p15;
    float p16;
    float p17;
    float p18;
    float p19;
    float p20;
    float p21;
    float p22;
    struct S_S_r4r4r4r4r4r4r4r4r4r4r4r4_S_r4r4r4_S_r4r4r4_i4bb_ p23;
    uint32_t p24;
    uint32_t p25;
};
    
// Compositor_CumulativeStats
struct S_u4u4u4u4u4u4u4u4u4u4u4u4u4u4u4u4r8r8r8r8r8u4_
{
    uint32_t p0;
    uint32_t p1;
    uint32_t p2;
    uint32_t p3;
    uint32_t p4;
    uint32_t p5;
    uint32_t p6;
    uint32_t p7;
    uint32_t p8;
    uint32_t p9;
    uint32_t p10;
    uint32_t p11;
    uint32_t p12;
    uint32_t p13;
    uint32_t p14;
    uint32_t p15;
    double p16;
    double p17;
    double p18;
    double p19;
    double p20;
    uint32_t p21;
};
    
// Compositor_StageRenderSettings
struct S_S_r4r4r4r4_S_r4r4r4r4_r4r4r4bbb_
{
    struct S_r4r4r4r4_ p0;
    struct S_r4r4r4r4_ p1;
    float p2;
    float p3;
    float p4;
    bool p5;
    bool p6;
    bool p7;
};
    
// VROverlayIntersectionParams_t
struct S_S_r4r4r4_S_r4r4r4_i4_
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    int32_t p2;
};
    
// VROverlayIntersectionResults_t
struct S_S_r4r4r4_S_r4r4r4_S_r4r4_r4_
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4_ p2;
    float p3;
};
    
// VROverlayIntersectionMaskPrimitive_Data_t
struct S_S_r4r4r4r4_S_r4r4r4__
{
    struct S_r4r4r4r4_ p0;
    struct S_r4r4r4_ p1;
};
    
// VROverlayIntersectionMaskPrimitive_t
struct S_i4S_S_r4r4r4r4_S_r4r4r4___
{
    int32_t p0;
    struct S_S_r4r4r4r4_S_r4r4r4__ p1;
};
    
// UnitPortPreservation
struct S_os_
{
    Il2CppObject* p0;
    Il2CppString* p1;
};
    
// TargetDevice
struct S_ssbb_
{
    Il2CppString* p0;
    Il2CppString* p1;
    bool p2;
    bool p3;
};
    
// FrameTime
struct S_r4i4i4_
{
    float p0;
    int32_t p1;
    int32_t p2;
};
    
// ThreadFrameTime
struct S_i4r4r4_
{
    int32_t p0;
    float p1;
    float p2;
};
    
// MarkerSummaryEntry
struct S_sr4r4r4r4i4i4_
{
    Il2CppString* p0;
    float p1;
    float p2;
    float p3;
    float p4;
    int32_t p5;
    int32_t p6;
};
    
// CameraVideoStreamFrameHeader_t
struct S_i4u4u4u4u4S_S_r4r4r4r4r4r4r4r4r4r4r4r4_S_r4r4r4_S_r4r4r4_i4bb_u8_
{
    int32_t p0;
    uint32_t p1;
    uint32_t p2;
    uint32_t p3;
    uint32_t p4;
    struct S_S_r4r4r4r4r4r4r4r4r4r4r4r4_S_r4r4r4_S_r4r4r4_i4bb_ p5;
    uint64_t p6;
};
    
// HmdQuad_t
struct S_S_r4r4r4_S_r4r4r4_S_r4r4r4_S_r4r4r4__
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4r4_ p2;
    struct S_r4r4r4_ p3;
};
    
// VROverlayView_t
struct S_u8S_pi4i4_S_r4r4r4r4__
{
    uint64_t p0;
    struct S_pi4i4_ p1;
    struct S_r4r4r4r4_ p2;
};
    
// NotificationBitmap_t
struct S_pi4i4i4_
{
    void* p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
};
    
// InputDigitalActionData_t
struct S_bu8bbr4_
{
    bool p0;
    uint64_t p1;
    bool p2;
    bool p3;
    float p4;
};
    
// InputAnalogActionData_t
struct S_bu8r4r4r4r4r4r4r4_
{
    bool p0;
    uint64_t p1;
    float p2;
    float p3;
    float p4;
    float p5;
    float p6;
    float p7;
    float p8;
};
    
// InputPoseActionData_t
struct S_bu8S_S_r4r4r4r4r4r4r4r4r4r4r4r4_S_r4r4r4_S_r4r4r4_i4bb__
{
    bool p0;
    uint64_t p1;
    struct S_S_r4r4r4r4r4r4r4r4r4r4r4r4_S_r4r4r4_S_r4r4r4_i4bb_ p2;
};
    
// InputSkeletalActionData_t
struct S_bu8_
{
    bool p0;
    uint64_t p1;
};
    
// VRSkeletalSummaryData_t
struct S_r4r4r4r4r4r4r4r4r4_
{
    float p0;
    float p1;
    float p2;
    float p3;
    float p4;
    float p5;
    float p6;
    float p7;
    float p8;
};
    
// InputOriginInfo_t
struct S_u8u4u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1_
{
    uint64_t p0;
    uint32_t p1;
    uint8_t p2;
    uint8_t p3;
    uint8_t p4;
    uint8_t p5;
    uint8_t p6;
    uint8_t p7;
    uint8_t p8;
    uint8_t p9;
    uint8_t p10;
    uint8_t p11;
    uint8_t p12;
    uint8_t p13;
    uint8_t p14;
    uint8_t p15;
    uint8_t p16;
    uint8_t p17;
    uint8_t p18;
    uint8_t p19;
    uint8_t p20;
    uint8_t p21;
    uint8_t p22;
    uint8_t p23;
    uint8_t p24;
    uint8_t p25;
    uint8_t p26;
    uint8_t p27;
    uint8_t p28;
    uint8_t p29;
    uint8_t p30;
    uint8_t p31;
    uint8_t p32;
    uint8_t p33;
    uint8_t p34;
    uint8_t p35;
    uint8_t p36;
    uint8_t p37;
    uint8_t p38;
    uint8_t p39;
    uint8_t p40;
    uint8_t p41;
    uint8_t p42;
    uint8_t p43;
    uint8_t p44;
    uint8_t p45;
    uint8_t p46;
    uint8_t p47;
    uint8_t p48;
    uint8_t p49;
    uint8_t p50;
    uint8_t p51;
    uint8_t p52;
    uint8_t p53;
    uint8_t p54;
    uint8_t p55;
    uint8_t p56;
    uint8_t p57;
    uint8_t p58;
    uint8_t p59;
    uint8_t p60;
    uint8_t p61;
    uint8_t p62;
    uint8_t p63;
    uint8_t p64;
    uint8_t p65;
    uint8_t p66;
    uint8_t p67;
    uint8_t p68;
    uint8_t p69;
    uint8_t p70;
    uint8_t p71;
    uint8_t p72;
    uint8_t p73;
    uint8_t p74;
    uint8_t p75;
    uint8_t p76;
    uint8_t p77;
    uint8_t p78;
    uint8_t p79;
    uint8_t p80;
    uint8_t p81;
    uint8_t p82;
    uint8_t p83;
    uint8_t p84;
    uint8_t p85;
    uint8_t p86;
    uint8_t p87;
    uint8_t p88;
    uint8_t p89;
    uint8_t p90;
    uint8_t p91;
    uint8_t p92;
    uint8_t p93;
    uint8_t p94;
    uint8_t p95;
    uint8_t p96;
    uint8_t p97;
    uint8_t p98;
    uint8_t p99;
    uint8_t p100;
    uint8_t p101;
    uint8_t p102;
    uint8_t p103;
    uint8_t p104;
    uint8_t p105;
    uint8_t p106;
    uint8_t p107;
    uint8_t p108;
    uint8_t p109;
    uint8_t p110;
    uint8_t p111;
    uint8_t p112;
    uint8_t p113;
    uint8_t p114;
    uint8_t p115;
    uint8_t p116;
    uint8_t p117;
    uint8_t p118;
    uint8_t p119;
    uint8_t p120;
    uint8_t p121;
    uint8_t p122;
    uint8_t p123;
    uint8_t p124;
    uint8_t p125;
    uint8_t p126;
    uint8_t p127;
    uint8_t p128;
    uint8_t p129;
};
    
// InputBindingInfo_t
struct S_u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1u1_
{
    uint8_t p0;
    uint8_t p1;
    uint8_t p2;
    uint8_t p3;
    uint8_t p4;
    uint8_t p5;
    uint8_t p6;
    uint8_t p7;
    uint8_t p8;
    uint8_t p9;
    uint8_t p10;
    uint8_t p11;
    uint8_t p12;
    uint8_t p13;
    uint8_t p14;
    uint8_t p15;
    uint8_t p16;
    uint8_t p17;
    uint8_t p18;
    uint8_t p19;
    uint8_t p20;
    uint8_t p21;
    uint8_t p22;
    uint8_t p23;
    uint8_t p24;
    uint8_t p25;
    uint8_t p26;
    uint8_t p27;
    uint8_t p28;
    uint8_t p29;
    uint8_t p30;
    uint8_t p31;
    uint8_t p32;
    uint8_t p33;
    uint8_t p34;
    uint8_t p35;
    uint8_t p36;
    uint8_t p37;
    uint8_t p38;
    uint8_t p39;
    uint8_t p40;
    uint8_t p41;
    uint8_t p42;
    uint8_t p43;
    uint8_t p44;
    uint8_t p45;
    uint8_t p46;
    uint8_t p47;
    uint8_t p48;
    uint8_t p49;
    uint8_t p50;
    uint8_t p51;
    uint8_t p52;
    uint8_t p53;
    uint8_t p54;
    uint8_t p55;
    uint8_t p56;
    uint8_t p57;
    uint8_t p58;
    uint8_t p59;
    uint8_t p60;
    uint8_t p61;
    uint8_t p62;
    uint8_t p63;
    uint8_t p64;
    uint8_t p65;
    uint8_t p66;
    uint8_t p67;
    uint8_t p68;
    uint8_t p69;
    uint8_t p70;
    uint8_t p71;
    uint8_t p72;
    uint8_t p73;
    uint8_t p74;
    uint8_t p75;
    uint8_t p76;
    uint8_t p77;
    uint8_t p78;
    uint8_t p79;
    uint8_t p80;
    uint8_t p81;
    uint8_t p82;
    uint8_t p83;
    uint8_t p84;
    uint8_t p85;
    uint8_t p86;
    uint8_t p87;
    uint8_t p88;
    uint8_t p89;
    uint8_t p90;
    uint8_t p91;
    uint8_t p92;
    uint8_t p93;
    uint8_t p94;
    uint8_t p95;
    uint8_t p96;
    uint8_t p97;
    uint8_t p98;
    uint8_t p99;
    uint8_t p100;
    uint8_t p101;
    uint8_t p102;
    uint8_t p103;
    uint8_t p104;
    uint8_t p105;
    uint8_t p106;
    uint8_t p107;
    uint8_t p108;
    uint8_t p109;
    uint8_t p110;
    uint8_t p111;
    uint8_t p112;
    uint8_t p113;
    uint8_t p114;
    uint8_t p115;
    uint8_t p116;
    uint8_t p117;
    uint8_t p118;
    uint8_t p119;
    uint8_t p120;
    uint8_t p121;
    uint8_t p122;
    uint8_t p123;
    uint8_t p124;
    uint8_t p125;
    uint8_t p126;
    uint8_t p127;
    uint8_t p128;
    uint8_t p129;
    uint8_t p130;
    uint8_t p131;
    uint8_t p132;
    uint8_t p133;
    uint8_t p134;
    uint8_t p135;
    uint8_t p136;
    uint8_t p137;
    uint8_t p138;
    uint8_t p139;
    uint8_t p140;
    uint8_t p141;
    uint8_t p142;
    uint8_t p143;
    uint8_t p144;
    uint8_t p145;
    uint8_t p146;
    uint8_t p147;
    uint8_t p148;
    uint8_t p149;
    uint8_t p150;
    uint8_t p151;
    uint8_t p152;
    uint8_t p153;
    uint8_t p154;
    uint8_t p155;
    uint8_t p156;
    uint8_t p157;
    uint8_t p158;
    uint8_t p159;
    uint8_t p160;
    uint8_t p161;
    uint8_t p162;
    uint8_t p163;
    uint8_t p164;
    uint8_t p165;
    uint8_t p166;
    uint8_t p167;
    uint8_t p168;
    uint8_t p169;
    uint8_t p170;
    uint8_t p171;
    uint8_t p172;
    uint8_t p173;
    uint8_t p174;
    uint8_t p175;
    uint8_t p176;
    uint8_t p177;
    uint8_t p178;
    uint8_t p179;
    uint8_t p180;
    uint8_t p181;
    uint8_t p182;
    uint8_t p183;
    uint8_t p184;
    uint8_t p185;
    uint8_t p186;
    uint8_t p187;
    uint8_t p188;
    uint8_t p189;
    uint8_t p190;
    uint8_t p191;
    uint8_t p192;
    uint8_t p193;
    uint8_t p194;
    uint8_t p195;
    uint8_t p196;
    uint8_t p197;
    uint8_t p198;
    uint8_t p199;
    uint8_t p200;
    uint8_t p201;
    uint8_t p202;
    uint8_t p203;
    uint8_t p204;
    uint8_t p205;
    uint8_t p206;
    uint8_t p207;
    uint8_t p208;
    uint8_t p209;
    uint8_t p210;
    uint8_t p211;
    uint8_t p212;
    uint8_t p213;
    uint8_t p214;
    uint8_t p215;
    uint8_t p216;
    uint8_t p217;
    uint8_t p218;
    uint8_t p219;
    uint8_t p220;
    uint8_t p221;
    uint8_t p222;
    uint8_t p223;
    uint8_t p224;
    uint8_t p225;
    uint8_t p226;
    uint8_t p227;
    uint8_t p228;
    uint8_t p229;
    uint8_t p230;
    uint8_t p231;
    uint8_t p232;
    uint8_t p233;
    uint8_t p234;
    uint8_t p235;
    uint8_t p236;
    uint8_t p237;
    uint8_t p238;
    uint8_t p239;
    uint8_t p240;
    uint8_t p241;
    uint8_t p242;
    uint8_t p243;
    uint8_t p244;
    uint8_t p245;
    uint8_t p246;
    uint8_t p247;
    uint8_t p248;
    uint8_t p249;
    uint8_t p250;
    uint8_t p251;
    uint8_t p252;
    uint8_t p253;
    uint8_t p254;
    uint8_t p255;
    uint8_t p256;
    uint8_t p257;
    uint8_t p258;
    uint8_t p259;
    uint8_t p260;
    uint8_t p261;
    uint8_t p262;
    uint8_t p263;
    uint8_t p264;
    uint8_t p265;
    uint8_t p266;
    uint8_t p267;
    uint8_t p268;
    uint8_t p269;
    uint8_t p270;
    uint8_t p271;
    uint8_t p272;
    uint8_t p273;
    uint8_t p274;
    uint8_t p275;
    uint8_t p276;
    uint8_t p277;
    uint8_t p278;
    uint8_t p279;
    uint8_t p280;
    uint8_t p281;
    uint8_t p282;
    uint8_t p283;
    uint8_t p284;
    uint8_t p285;
    uint8_t p286;
    uint8_t p287;
    uint8_t p288;
    uint8_t p289;
    uint8_t p290;
    uint8_t p291;
    uint8_t p292;
    uint8_t p293;
    uint8_t p294;
    uint8_t p295;
    uint8_t p296;
    uint8_t p297;
    uint8_t p298;
    uint8_t p299;
    uint8_t p300;
    uint8_t p301;
    uint8_t p302;
    uint8_t p303;
    uint8_t p304;
    uint8_t p305;
    uint8_t p306;
    uint8_t p307;
    uint8_t p308;
    uint8_t p309;
    uint8_t p310;
    uint8_t p311;
    uint8_t p312;
    uint8_t p313;
    uint8_t p314;
    uint8_t p315;
    uint8_t p316;
    uint8_t p317;
    uint8_t p318;
    uint8_t p319;
    uint8_t p320;
    uint8_t p321;
    uint8_t p322;
    uint8_t p323;
    uint8_t p324;
    uint8_t p325;
    uint8_t p326;
    uint8_t p327;
    uint8_t p328;
    uint8_t p329;
    uint8_t p330;
    uint8_t p331;
    uint8_t p332;
    uint8_t p333;
    uint8_t p334;
    uint8_t p335;
    uint8_t p336;
    uint8_t p337;
    uint8_t p338;
    uint8_t p339;
    uint8_t p340;
    uint8_t p341;
    uint8_t p342;
    uint8_t p343;
    uint8_t p344;
    uint8_t p345;
    uint8_t p346;
    uint8_t p347;
    uint8_t p348;
    uint8_t p349;
    uint8_t p350;
    uint8_t p351;
    uint8_t p352;
    uint8_t p353;
    uint8_t p354;
    uint8_t p355;
    uint8_t p356;
    uint8_t p357;
    uint8_t p358;
    uint8_t p359;
    uint8_t p360;
    uint8_t p361;
    uint8_t p362;
    uint8_t p363;
    uint8_t p364;
    uint8_t p365;
    uint8_t p366;
    uint8_t p367;
    uint8_t p368;
    uint8_t p369;
    uint8_t p370;
    uint8_t p371;
    uint8_t p372;
    uint8_t p373;
    uint8_t p374;
    uint8_t p375;
    uint8_t p376;
    uint8_t p377;
    uint8_t p378;
    uint8_t p379;
    uint8_t p380;
    uint8_t p381;
    uint8_t p382;
    uint8_t p383;
    uint8_t p384;
    uint8_t p385;
    uint8_t p386;
    uint8_t p387;
    uint8_t p388;
    uint8_t p389;
    uint8_t p390;
    uint8_t p391;
    uint8_t p392;
    uint8_t p393;
    uint8_t p394;
    uint8_t p395;
    uint8_t p396;
    uint8_t p397;
    uint8_t p398;
    uint8_t p399;
    uint8_t p400;
    uint8_t p401;
    uint8_t p402;
    uint8_t p403;
    uint8_t p404;
    uint8_t p405;
    uint8_t p406;
    uint8_t p407;
    uint8_t p408;
    uint8_t p409;
    uint8_t p410;
    uint8_t p411;
    uint8_t p412;
    uint8_t p413;
    uint8_t p414;
    uint8_t p415;
    uint8_t p416;
    uint8_t p417;
    uint8_t p418;
    uint8_t p419;
    uint8_t p420;
    uint8_t p421;
    uint8_t p422;
    uint8_t p423;
    uint8_t p424;
    uint8_t p425;
    uint8_t p426;
    uint8_t p427;
    uint8_t p428;
    uint8_t p429;
    uint8_t p430;
    uint8_t p431;
    uint8_t p432;
    uint8_t p433;
    uint8_t p434;
    uint8_t p435;
    uint8_t p436;
    uint8_t p437;
    uint8_t p438;
    uint8_t p439;
    uint8_t p440;
    uint8_t p441;
    uint8_t p442;
    uint8_t p443;
    uint8_t p444;
    uint8_t p445;
    uint8_t p446;
    uint8_t p447;
    uint8_t p448;
    uint8_t p449;
    uint8_t p450;
    uint8_t p451;
    uint8_t p452;
    uint8_t p453;
    uint8_t p454;
    uint8_t p455;
    uint8_t p456;
    uint8_t p457;
    uint8_t p458;
    uint8_t p459;
    uint8_t p460;
    uint8_t p461;
    uint8_t p462;
    uint8_t p463;
    uint8_t p464;
    uint8_t p465;
    uint8_t p466;
    uint8_t p467;
    uint8_t p468;
    uint8_t p469;
    uint8_t p470;
    uint8_t p471;
    uint8_t p472;
    uint8_t p473;
    uint8_t p474;
    uint8_t p475;
    uint8_t p476;
    uint8_t p477;
    uint8_t p478;
    uint8_t p479;
    uint8_t p480;
    uint8_t p481;
    uint8_t p482;
    uint8_t p483;
    uint8_t p484;
    uint8_t p485;
    uint8_t p486;
    uint8_t p487;
    uint8_t p488;
    uint8_t p489;
    uint8_t p490;
    uint8_t p491;
    uint8_t p492;
    uint8_t p493;
    uint8_t p494;
    uint8_t p495;
    uint8_t p496;
    uint8_t p497;
    uint8_t p498;
    uint8_t p499;
    uint8_t p500;
    uint8_t p501;
    uint8_t p502;
    uint8_t p503;
    uint8_t p504;
    uint8_t p505;
    uint8_t p506;
    uint8_t p507;
    uint8_t p508;
    uint8_t p509;
    uint8_t p510;
    uint8_t p511;
    uint8_t p512;
    uint8_t p513;
    uint8_t p514;
    uint8_t p515;
    uint8_t p516;
    uint8_t p517;
    uint8_t p518;
    uint8_t p519;
    uint8_t p520;
    uint8_t p521;
    uint8_t p522;
    uint8_t p523;
    uint8_t p524;
    uint8_t p525;
    uint8_t p526;
    uint8_t p527;
    uint8_t p528;
    uint8_t p529;
    uint8_t p530;
    uint8_t p531;
    uint8_t p532;
    uint8_t p533;
    uint8_t p534;
    uint8_t p535;
    uint8_t p536;
    uint8_t p537;
    uint8_t p538;
    uint8_t p539;
    uint8_t p540;
    uint8_t p541;
    uint8_t p542;
    uint8_t p543;
};
    
// SpatialAnchorPose_t
struct S_S_r4r4r4r4r4r4r4r4r4r4r4r4__
{
    struct S_r4r4r4r4r4r4r4r4r4r4r4r4_ p0;
};
    
// PropertyRead_t
struct S_i4pu4u4u4i4_
{
    int32_t p0;
    void* p1;
    uint32_t p2;
    uint32_t p3;
    uint32_t p4;
    int32_t p5;
};
    
// PropertyWrite_t
struct S_i4i4i4pu4u4i4_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    void* p3;
    uint32_t p4;
    uint32_t p5;
    int32_t p6;
};
    
// PathRead_t
struct S_u8pu4u4u4i4p_
{
    uint64_t p0;
    void* p1;
    uint32_t p2;
    uint32_t p3;
    uint32_t p4;
    int32_t p5;
    void* p6;
};
    
// PathWrite_t
struct S_u8i4i4pu4u4i4p_
{
    uint64_t p0;
    int32_t p1;
    int32_t p2;
    void* p3;
    uint32_t p4;
    uint32_t p5;
    int32_t p6;
    void* p7;
};
    
// VertexGradient
struct S_S_r4r4r4r4_S_r4r4r4r4_S_r4r4r4r4_S_r4r4r4r4__
{
    struct S_r4r4r4r4_ p0;
    struct S_r4r4r4r4_ p1;
    struct S_r4r4r4r4_ p2;
    struct S_r4r4r4r4_ p3;
};
    
// TMP_TextProcessingStack`1
struct S_oi4r4i4i4i4_
{
    Il2CppObject* p0;
    int32_t p1;
    float p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
};
    
// TMP_TextProcessingStack`1
struct S_oi4i4i4i4i4_
{
    Il2CppObject* p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
};
    
// TMP_FontStyleStack
struct S_u1u1u1u1u1u1u1u1u1u1_
{
    uint8_t p0;
    uint8_t p1;
    uint8_t p2;
    uint8_t p3;
    uint8_t p4;
    uint8_t p5;
    uint8_t p6;
    uint8_t p7;
    uint8_t p8;
    uint8_t p9;
};
    
// TMP_TextProcessingStack`1
struct S_oi4S_i4u1u1u1u1_i4i4i4_
{
    Il2CppObject* p0;
    int32_t p1;
    struct S_i4u1u1u1u1_ p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
};
    
// HighlightState
struct S_S_i4u1u1u1u1_S_r4r4r4r4__
{
    struct S_i4u1u1u1u1_ p0;
    struct S_r4r4r4r4_ p1;
};
    
// TMP_TextProcessingStack`1
struct S_oi4S_S_i4u1u1u1u1_S_r4r4r4r4__i4i4i4_
{
    Il2CppObject* p0;
    int32_t p1;
    struct S_S_i4u1u1u1u1_S_r4r4r4r4__ p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
};
    
// TMP_TextProcessingStack`1
struct S_oi4oi4i4i4_
{
    Il2CppObject* p0;
    int32_t p1;
    Il2CppObject* p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
};
    
// SpecialCharacter
struct S_oooi4_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    int32_t p3;
};
    
// PokeStateData
struct S_bS_r4r4r4_S_r4r4r4_r4S_r4r4r4_o_
{
    bool p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4r4_ p2;
    float p3;
    struct S_r4r4r4_ p4;
    Il2CppObject* p5;
};
    
// TeleportRequest
struct S_S_r4r4r4_S_r4r4r4r4_r4i4_
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4r4_ p1;
    float p2;
    int32_t p3;
};
    
// RaycastHitData
struct S_oS_r4r4r4_S_r4r4_r4i4_
{
    Il2CppObject* p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4_ p2;
    float p3;
    int32_t p4;
};
    
// RaycastHit
struct S_S_r4r4r4_S_r4r4r4_u4r4S_r4r4_i4_
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    uint32_t p2;
    float p3;
    struct S_r4r4_ p4;
    int32_t p5;
};
    
// HandExpressionName
struct S_S_ss__
{
    struct S_ss_ p0;
};
    
// AffordanceStateData
struct S_u1u1_
{
    uint8_t p0;
    uint8_t p1;
};
    
// JobHandle
struct S_u8i4i4p_
{
    uint64_t p0;
    int32_t p1;
    int32_t p2;
    void* p3;
};
    
// ChatMessage
struct S_sbssbss_
{
    Il2CppString* p0;
    bool p1;
    Il2CppString* p2;
    Il2CppString* p3;
    bool p4;
    Il2CppString* p5;
    Il2CppString* p6;
};
    
// XRNodeState
struct S_i4i4S_r4r4r4_S_r4r4r4r4_S_r4r4r4_S_r4r4r4_S_r4r4r4_S_r4r4r4_i4u8_
{
    int32_t p0;
    int32_t p1;
    struct S_r4r4r4_ p2;
    struct S_r4r4r4r4_ p3;
    struct S_r4r4r4_ p4;
    struct S_r4r4r4_ p5;
    struct S_r4r4r4_ p6;
    struct S_r4r4r4_ p7;
    int32_t p8;
    uint64_t p9;
};
    
// LooseAssemblyName
struct S_s_
{
    Il2CppString* p0;
};
    
// SemanticVersion
struct S_i4i4i4si4_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    Il2CppString* p3;
    int32_t p4;
};
    
// CursorRect
struct S_S_r4r4r4r4_i4_
{
    struct S_r4r4r4r4_ p0;
    int32_t p1;
};
    
// Playable
struct S_S_pu4__
{
    struct S_pu4_ p0;
};
    
// NotificationFlags
struct i2
{
    int16_t p0;
};
    
// NotificationEntry
struct S_r8obi2_
{
    double p0;
    Il2CppObject* p1;
    bool p2;
    int16_t p3;
};
    
// <buffer>e__FixedBuffer
struct S_c_
{
    Il2CppChar p0;
};
    
// IMECompositionString
struct S_i4S_c__
{
    int32_t p0;
    struct S_c_ p1;
};
    
// ControlItem
struct S_S_ss_S_ss_S_ss_sssS_oi4i4_S_oi4i4_S_oi4i4_S_oi4i4_u4u4u4S_i4_i4i4S_i4bcu1i1i2u2i4u4i8u8r4r8_S_i4bcu1i1i2u2i4u4i8u8r4r8_S_i4bcu1i1i2u2i4u4i8u8r4r8__
{
    struct S_ss_ p0;
    struct S_ss_ p1;
    struct S_ss_ p2;
    Il2CppString* p3;
    Il2CppString* p4;
    Il2CppString* p5;
    struct S_oi4i4_ p6;
    struct S_oi4i4_ p7;
    struct S_oi4i4_ p8;
    struct S_oi4i4_ p9;
    uint32_t p10;
    uint32_t p11;
    uint32_t p12;
    struct S_i4_ p13;
    int32_t p14;
    int32_t p15;
    struct S_i4bcu1i1i2u2i4u4i8u8r4r8_ p16;
    struct S_i4bcu1i1i2u2i4u4i8u8r4r8_ p17;
    struct S_i4bcu1i1i2u2i4u4i8u8r4r8_ p18;
};
    
// LayoutMatcher
struct S_S_ss_S_o__
{
    struct S_ss_ p0;
    struct S_o_ p1;
};
    
// InputEventPtr
struct S_Pv_
{
    void* p0;
};
    
// InputDeviceCommand
struct S_S_i4_i4_
{
    struct S_i4_ p0;
    int32_t p1;
};
    
// InputEventBuffer
struct S_S_Pvi4i4i4S_pi4i4_i4_i8i4b_
{
    struct S_Pvi4i4i4S_pi4i4_i4_ p0;
    int64_t p1;
    int32_t p2;
    bool p3;
};
    
// StateChangeMonitorTimeout
struct S_or8oi8i4_
{
    Il2CppObject* p0;
    double p1;
    Il2CppObject* p2;
    int64_t p3;
    int32_t p4;
};
    
// TouchState
struct S_i4S_r4r4_S_r4r4_r4S_r4r4_u1u1u1u1u4r8S_r4r4__
{
    int32_t p0;
    struct S_r4r4_ p1;
    struct S_r4r4_ p2;
    float p3;
    struct S_r4r4_ p4;
    uint8_t p5;
    uint8_t p6;
    uint8_t p7;
    uint8_t p8;
    uint32_t p9;
    double p10;
    struct S_r4r4_ p11;
};
    
// ButtonState
struct S_bi4r4S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooooS_r4r4_r4i4bbb_
{
    bool p0;
    int32_t p1;
    float p2;
    struct S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ p3;
    Il2CppObject* p4;
    Il2CppObject* p5;
    Il2CppObject* p6;
    Il2CppObject* p7;
    struct S_r4r4_ p8;
    float p9;
    int32_t p10;
    bool p11;
    bool p12;
    bool p13;
};
    
// PointerModel
struct S_bS_bi4r4S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooooS_r4r4_r4i4bbb_S_bi4r4S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooooS_r4r4_r4i4bbb_S_bi4r4S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooooS_r4r4_r4i4bbb_oS_r4r4_S_r4r4_S_r4r4r4_S_r4r4r4r4_r4r4r4r4S_r4r4__
{
    bool p0;
    struct S_bi4r4S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooooS_r4r4_r4i4bbb_ p1;
    struct S_bi4r4S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooooS_r4r4_r4i4bbb_ p2;
    struct S_bi4r4S_oor4r4i4i4i4i4i4S_r4r4r4_S_r4r4r4_S_r4r4_i4_ooooS_r4r4_r4i4bbb_ p3;
    Il2CppObject* p4;
    struct S_r4r4_ p5;
    struct S_r4r4_ p6;
    struct S_r4r4r4_ p7;
    struct S_r4r4r4r4_ p8;
    float p9;
    float p10;
    float p11;
    float p12;
    struct S_r4r4_ p13;
};
    
// XRFeatureDescriptor
struct S_soi4u4_
{
    Il2CppString* p0;
    Il2CppObject* p1;
    int32_t p2;
    uint32_t p3;
};
    
// RaycastHitData
struct S_oS_r4r4r4_S_r4r4_r4_
{
    Il2CppObject* p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4_ p2;
    float p3;
};
    
// Record
struct S_oi4u4_
{
    Il2CppObject* p0;
    int32_t p1;
    uint32_t p2;
};
    
// InputControlList`1
struct S_i4S_Pvi4i4i4S_pi4i4_i4_i4_
{
    int32_t p0;
    struct S_Pvi4i4i4S_pi4i4_i4_ p1;
    int32_t p2;
};
    
// DeviceInfo
struct S_i4sS_i4_i4s_
{
    int32_t p0;
    Il2CppString* p1;
    struct S_i4_ p2;
    int32_t p3;
    Il2CppString* p4;
};
    
// UnmanagedMemory
struct S_Pvi4i4i4i4i4i4PvPvPvPvPvPvPvPvPvPvbPv_
{
    void* p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
    void* p7;
    void* p8;
    void* p9;
    void* p10;
    void* p11;
    void* p12;
    void* p13;
    void* p14;
    void* p15;
    void* p16;
    bool p17;
    void* p18;
};
    
// PairedUser
struct S_i4u8ss_
{
    int32_t p0;
    uint64_t p1;
    Il2CppString* p2;
    Il2CppString* p3;
};
    
// Nullable`1
struct N_br8_
{
    bool hasValue;
    double p1;
};
    
// Decimal
struct S_i4i4i4i4u8_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    uint64_t p4;
};
    
// Nullable`1
struct N_bS_i4i4i4i4u8__
{
    bool hasValue;
    struct S_i4i4i4i4u8_ p1;
};
    
// PurgeFileTypeRequest
struct S_sS_u8__
{
    Il2CppString* p0;
    struct S_u8_ p1;
};
    
// Nullable`1
struct N_bi8_
{
    bool hasValue;
    int64_t p1;
};
    
// DateTimeOffset
struct S_S_u8_i2_
{
    struct S_u8_ p0;
    int16_t p1;
};
    
// Nullable`1
struct N_bS_S_u8_i2__
{
    bool hasValue;
    struct S_S_u8_i2_ p1;
};
    
// BranchRequest
struct S_i8s_
{
    int64_t p0;
    Il2CppString* p1;
};
    
// DirectoryConflictResolutionData
struct S_oooi4s_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    int32_t p3;
    Il2CppString* p4;
};
    
// DownloadedSegment
struct S_S_u4u4_i8u8b_
{
    struct S_u4u4_ p0;
    int64_t p1;
    uint64_t p2;
    bool p3;
};
    
// Color
struct S_si8i2i2_
{
    Il2CppString* p0;
    int64_t p1;
    int16_t p2;
    int16_t p3;
};
    
// Padding
struct S_i4i4i4i4b_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    bool p4;
};
    
// JsonPosition
struct S_i4i4sb_
{
    int32_t p0;
    int32_t p1;
    Il2CppString* p2;
    bool p3;
};
    
// BsonType
struct i1
{
    int8_t p0;
};
    
// RiderInfo
struct S_bssos_
{
    bool p0;
    Il2CppString* p1;
    Il2CppString* p2;
    Il2CppObject* p3;
    Il2CppString* p4;
};
    
// ObjectReferenceStack
struct S_i4ooo_
{
    int32_t p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    Il2CppObject* p3;
};
    
// Span`1
struct S_S_p_i4_
{
    struct S_p_ p0;
    int32_t p1;
};
    
// NavMeshBuildMarkup
struct S_i4i4i4i4i4i4i4i4_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
    int32_t p7;
};
    
// NavMeshBuildSource
struct S_S_r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4_S_r4r4r4_i4i4i4i4i4_
{
    struct S_r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
};
    
// DebugScreenCapture
struct S_S_Pvi4i4i4S_pi4i4_i4_i4i4i4_
{
    struct S_Pvi4i4i4S_pi4i4_i4_ p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
};
    
// AsyncReadManagerRequestMetric
struct S_ssu8u8u8u8u4bi4i4i4i4r8r8r8_
{
    Il2CppString* p0;
    Il2CppString* p1;
    uint64_t p2;
    uint64_t p3;
    uint64_t p4;
    uint64_t p5;
    uint32_t p6;
    bool p7;
    int32_t p8;
    int32_t p9;
    int32_t p10;
    int32_t p11;
    double p12;
    double p13;
    double p14;
};
    
// DisplayInfo
struct S_u8i4i4S_u4u4_S_i4i4i4i4_s_
{
    uint64_t p0;
    int32_t p1;
    int32_t p2;
    struct S_u4u4_ p3;
    struct S_i4i4i4i4_ p4;
    Il2CppString* p5;
};
    
// ScriptableRenderContext
struct S_pS_pi4i4__
{
    void* p0;
    struct S_pi4i4_ p1;
};
    
// ParticleCollisionEvent
struct S_S_r4r4r4_S_r4r4r4_S_r4r4r4_i4_
{
    struct S_r4r4r4_ p0;
    struct S_r4r4r4_ p1;
    struct S_r4r4r4_ p2;
    int32_t p3;
};
    
// ReadOnly
struct S_Pvi4S_pi4i4__
{
    void* p0;
    int32_t p1;
    struct S_pi4i4_ p2;
};
    
// VFXBatchedEffectInfo
struct S_ou4u4u4u4u4u4u8u8_
{
    Il2CppObject* p0;
    uint32_t p1;
    uint32_t p2;
    uint32_t p3;
    uint32_t p4;
    uint32_t p5;
    uint32_t p6;
    uint64_t p7;
    uint64_t p8;
};
    
// BuildPlayerOptions
struct S_ossi4i4i4i4o_
{
    Il2CppObject* p0;
    Il2CppString* p1;
    Il2CppString* p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
    int32_t p6;
    Il2CppObject* p7;
};
    
// ObjectChangeEventStream
struct S_S_Pvi4i4i4S_pi4i4_i4_S_Pvi4i4i4S_pi4i4_i4__
{
    struct S_Pvi4i4i4S_pi4i4_i4_ p0;
    struct S_Pvi4i4i4S_pi4i4_i4_ p1;
};
    
// VirtualMachineInformation
struct S_i4i4i4i4i4i4_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    int32_t p4;
    int32_t p5;
};
    
// SubSceneInfo
struct S_oS_i4_osS_i4u1u1u1u1__
{
    Il2CppObject* p0;
    struct S_i4_ p1;
    Il2CppObject* p2;
    Il2CppString* p3;
    struct S_i4u1u1u1u1_ p4;
};
    
// IVRSystem
struct S_oooooooooooooooooooooooooooooooooooooooooooooo_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    Il2CppObject* p3;
    Il2CppObject* p4;
    Il2CppObject* p5;
    Il2CppObject* p6;
    Il2CppObject* p7;
    Il2CppObject* p8;
    Il2CppObject* p9;
    Il2CppObject* p10;
    Il2CppObject* p11;
    Il2CppObject* p12;
    Il2CppObject* p13;
    Il2CppObject* p14;
    Il2CppObject* p15;
    Il2CppObject* p16;
    Il2CppObject* p17;
    Il2CppObject* p18;
    Il2CppObject* p19;
    Il2CppObject* p20;
    Il2CppObject* p21;
    Il2CppObject* p22;
    Il2CppObject* p23;
    Il2CppObject* p24;
    Il2CppObject* p25;
    Il2CppObject* p26;
    Il2CppObject* p27;
    Il2CppObject* p28;
    Il2CppObject* p29;
    Il2CppObject* p30;
    Il2CppObject* p31;
    Il2CppObject* p32;
    Il2CppObject* p33;
    Il2CppObject* p34;
    Il2CppObject* p35;
    Il2CppObject* p36;
    Il2CppObject* p37;
    Il2CppObject* p38;
    Il2CppObject* p39;
    Il2CppObject* p40;
    Il2CppObject* p41;
    Il2CppObject* p42;
    Il2CppObject* p43;
    Il2CppObject* p44;
    Il2CppObject* p45;
};
    
// EventHook
struct S_sOO_
{
    Il2CppString* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
};
    
// fsVersionedType
struct S_oso_
{
    Il2CppObject* p0;
    Il2CppString* p1;
    Il2CppObject* p2;
};
    
// RawRepository
struct S_S_u4u4_o_
{
    struct S_u4u4_ p0;
    Il2CppObject* p1;
};
    
// StyleSelectorPart
struct S_si4O_
{
    Il2CppString* p0;
    int32_t p1;
    Il2CppObject* p2;
};
    
// Touch
struct S_oS_oi4u4__
{
    Il2CppObject* p0;
    struct S_oi4u4_ p1;
};
    
// PxrLayerParam
struct S_i4i4i4i4u8u4u4u4u4u4u4u4u4pp_
{
    int32_t p0;
    int32_t p1;
    int32_t p2;
    int32_t p3;
    uint64_t p4;
    uint32_t p5;
    uint32_t p6;
    uint32_t p7;
    uint32_t p8;
    uint32_t p9;
    uint32_t p10;
    uint32_t p11;
    uint32_t p12;
    void* p13;
    void* p14;
};
    
// MatchResultInfo
struct S_bi4i4_
{
    bool p0;
    int32_t p1;
    int32_t p2;
};
    
// KeyValuePair`2
struct S_S_r4r4r4r4_o_
{
    struct S_r4r4r4r4_ p0;
    Il2CppObject* p1;
};
    
// State
struct S_oS_i4_i4r4_
{
    Il2CppObject* p0;
    struct S_i4_ p1;
    int32_t p2;
    float p3;
};
    
// Alloc
struct S_u4u4Ob_
{
    uint32_t p0;
    uint32_t p1;
    Il2CppObject* p2;
    bool p3;
};
    
// AllocToFree
struct S_S_u4u4Ob_ob_
{
    struct S_u4u4Ob_ p0;
    Il2CppObject* p1;
    bool p2;
};
    
// AllocToUpdate
struct S_u4u4oS_u4u4Ob_S_u4u4Ob_ob_
{
    uint32_t p0;
    uint32_t p1;
    Il2CppObject* p2;
    struct S_u4u4Ob_ p3;
    struct S_u4u4Ob_ p4;
    Il2CppObject* p5;
    bool p6;
};
    
// RenderNodeData
struct S_ooooooor4S_Pvi4i4i4i4S_pi4i4__S_Pvi4i4i4i4S_pi4i4___
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppObject* p2;
    Il2CppObject* p3;
    Il2CppObject* p4;
    Il2CppObject* p5;
    Il2CppObject* p6;
    float p7;
    struct S_Pvi4i4i4i4S_pi4i4__ p8;
    struct S_Pvi4i4i4i4S_pi4i4__ p9;
};
    
// Page
struct S_u2u2i4_
{
    uint16_t p0;
    uint16_t p1;
    int32_t p2;
};
    
// Entry
struct S_S_Pvi4i4i4i4S_pi4i4__S_Pvi4i4i4i4S_pi4i4__or4S_i4_oS_i4u2u1u1_i4bbbi4i4_
{
    struct S_Pvi4i4i4i4S_pi4i4__ p0;
    struct S_Pvi4i4i4i4S_pi4i4__ p1;
    Il2CppObject* p2;
    float p3;
    struct S_i4_ p4;
    Il2CppObject* p5;
    struct S_i4u2u1u1_ p6;
    int32_t p7;
    bool p8;
    bool p9;
    bool p10;
    int32_t p11;
    int32_t p12;
};
    
// RepeatRectUV
struct S_S_r4r4r4r4_S_r4r4r4r4__
{
    struct S_r4r4r4r4_ p0;
    struct S_r4r4r4r4_ p1;
};
    
// AllocMeshData
struct S_ooS_i4_oi4S_i4u2u1u1__
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    struct S_i4_ p2;
    Il2CppObject* p3;
    int32_t p4;
    struct S_i4u2u1u1_ p5;
};
    
// EvalHandlerArgs
struct S_sObi4ooO_
{
    Il2CppString* p0;
    Il2CppObject* p1;
    bool p2;
    int32_t p3;
    Il2CppObject* p4;
    Il2CppObject* p5;
    Il2CppObject* p6;
};
    
// QueryData
struct S_oosoooS_r4r4_i4_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    Il2CppString* p2;
    Il2CppObject* p3;
    Il2CppObject* p4;
    Il2CppObject* p5;
    struct S_r4r4_ p6;
    int32_t p7;
};
    
// unitytls_errorstate
struct S_u4u4u8_
{
    uint32_t p0;
    uint32_t p1;
    uint64_t p2;
};
    
// Void
struct v
{
    union
    {
        struct
        {
        };
        uint8_t __padding[1];
    };
};
    
// unitytls_tlsctx_callbacks
struct S_ooPv_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    void* p2;
};
    
// Entry
struct S_Oi8i8S_r4r4r4r4__
{
    Il2CppObject* p0;
    int64_t p1;
    int64_t p2;
    struct S_r4r4r4r4_ p3;
};
    
// Entry
struct S_i8i8S_Oi8i8S_r4r4r4r4___
{
    int64_t p0;
    int64_t p1;
    struct S_Oi8i8S_r4r4r4r4__ p2;
};
    
// MarkerOverlay
struct S_oS_r4r4r4r4_bbo_
{
    Il2CppObject* p0;
    struct S_r4r4r4r4_ p1;
    bool p2;
    bool p3;
    Il2CppObject* p4;
};
    
// LayerZOrder
struct S_u1i4_
{
    uint8_t p0;
    int32_t p1;
};
    
// ClipBlends
struct S_i4S_r4r4r4r4_i4S_r4r4r4r4__
{
    int32_t p0;
    struct S_r4r4r4r4_ p1;
    int32_t p2;
    struct S_r4r4r4r4_ p3;
};
    
// ClipDrawOptions
struct S_obsbsS_r4r4r4r4__
{
    Il2CppObject* p0;
    bool p1;
    Il2CppString* p2;
    bool p3;
    Il2CppString* p4;
    struct S_r4r4r4r4_ p5;
};
    
// ClipDrawData
struct S_oS_r4r4r4r4_S_r4r4r4r4_S_r4r4r4r4_S_r4r4r4r4_sbbr8r8ooobbi4oS_i4S_r4r4r4r4_i4S_r4r4r4r4__S_obsbsS_r4r4r4r4__o_
{
    Il2CppObject* p0;
    struct S_r4r4r4r4_ p1;
    struct S_r4r4r4r4_ p2;
    struct S_r4r4r4r4_ p3;
    struct S_r4r4r4r4_ p4;
    Il2CppString* p5;
    bool p6;
    bool p7;
    double p8;
    double p9;
    Il2CppObject* p10;
    Il2CppObject* p11;
    Il2CppObject* p12;
    bool p13;
    bool p14;
    int32_t p15;
    Il2CppObject* p16;
    struct S_i4S_r4r4r4r4_i4S_r4r4r4r4__ p17;
    struct S_obsbsS_r4r4r4r4__ p18;
    Il2CppObject* p19;
};
    
// OverlayDrawer
struct S_i4S_r4r4r4r4_soS_r4r4r4r4_oo_
{
    int32_t p0;
    struct S_r4r4r4r4_ p1;
    Il2CppString* p2;
    Il2CppObject* p3;
    struct S_r4r4r4r4_ p4;
    Il2CppObject* p5;
    Il2CppObject* p6;
};
    
// BindingOverrideJson
struct S_sssss_
{
    Il2CppString* p0;
    Il2CppString* p1;
    Il2CppString* p2;
    Il2CppString* p3;
    Il2CppString* p4;
};
    
// Frame
struct S_sS_r4r4r4r4_bbS_r4r4r4r4_S_r4r4_S_r4r4__
{
    Il2CppString* p0;
    struct S_r4r4r4r4_ p1;
    bool p2;
    bool p3;
    struct S_r4r4r4r4_ p4;
    struct S_r4r4_ p5;
    struct S_r4r4_ p6;
};
    
// Nullable`1
struct N_bS_i4i4__
{
    bool hasValue;
    struct S_i4i4_ p1;
};
    
// ActiveBuildStatus
struct S_sN_bS_i4i4___
{
    Il2CppString* p0;
    struct N_bS_i4i4__ p1;
};
    
// ParseResult`1
struct S_bo_
{
    bool p0;
    Il2CppObject* p1;
};
    
// SocketReceiveMessageFromResult
struct S_i4i4oS_oi4__
{
    int32_t p0;
    int32_t p1;
    Il2CppObject* p2;
    struct S_oi4_ p3;
};
    
// ValueTuple`3
struct S_ooi4_
{
    Il2CppObject* p0;
    Il2CppObject* p1;
    int32_t p2;
};
    
// ValueTuple`5
struct S_obboo_
{
    Il2CppObject* p0;
    bool p1;
    bool p2;
    Il2CppObject* p3;
    Il2CppObject* p4;
};
    
// DistanceInfo
struct S_S_r4r4r4_r4o_
{
    struct S_r4r4r4_ p0;
    float p1;
    Il2CppObject* p2;
};
    
// InlinedArray`1
struct S_i4S_ss_o_
{
    int32_t p0;
    struct S_ss_ p1;
    Il2CppObject* p2;
};
    
// Nullable`1
struct N_bS_Oi4__
{
    bool hasValue;
    struct S_Oi4_ p1;
};
    
}
