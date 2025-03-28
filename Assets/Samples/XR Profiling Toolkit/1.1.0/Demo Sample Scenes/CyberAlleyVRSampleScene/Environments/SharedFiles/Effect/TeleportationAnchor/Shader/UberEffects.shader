Shader "PicoVideo/Effects/UberEffects"
{
    Properties 
	{
        [Main(_SurfaceOptions, _, on, off)]_SurfaceOptions("Surface Options", Float) = 0
        [ExtendEnum(_SurfaceOptions, Matrix.ShaderGUI.SurfaceType,1,_Blend)]_Surface("Surface Type", Float) = 1
        [HideInInspector][ExtendEnum(_SurfaceOptions, Matrix.ShaderGUI.BlendMode)] _Blend("Blending Mode", Float) = 2
        [ExtendEnum(_SurfaceOptions, Matrix.ShaderGUI.RenderFace)]_Cull("Render Face", Float) = 2
        [ExtendToggle(_SurfaceOptions, _, _ZWrite)]_CustomZWrite("Custom ZWrite", Float) = 0
        [HideInInspector][SubToggle(_SurfaceOptions)]_ZWrite("ZWrite", Float) = 0
        [ExtendEnum(_SurfaceOptions, UnityEngine.Rendering.CompareFunction)]_ZTest("ZTest", Float) = 4
        [ExtendEnum(_SurfaceOptions, Matrix.ShaderGUI.ColorWriteMask)]_ColorMask ("ColorMask", Float) = 15
        [ExtendToggle(_SurfaceOptions, _ALPHATEST_ON, _Cutoff)] _AlphaClip("Alpha Clipping", Float) = 0.0
		[HideInInspector]_Cutoff("Threshold", Range(0.0, 1.0)) = 0.5
        [SubToggle(_SurfaceOptions, _BILLBOARD)]_EnableBillboard("Enable Billboard", float) = 0
        [ExtendToggle(_SurfaceOptions, _DISCOLORATION , _DecolorationIntensity)]_Decoloration("Decoloration", int) = 0
	    [HideInInspector]_DecolorationIntensity("Decoloration Intensity", Range(0.0 , 15.0)) = 1
        [Title(_SurfaceOptions, Stencil)]
		[Sub(_SurfaceOptions)]_StencilRef("StencilRef", Range(0,255)) = 127
		[ExtendEnum(_SurfaceOptions, UnityEngine.Rendering.CompareFunction)]_StencilComp("StencilComp", int) = 8
        [ExtendEnum(_SurfaceOptions, UnityEngine.Rendering.StencilOp)]_StencilPass("Stencil Pass", Float) = 0
        [ExtendEnum(_SurfaceOptions, UnityEngine.Rendering.StencilOp)]_StencilFail("Stencil Fail", Float) = 0

        [Main(_SurfaceInputs, _, on, off)]_BaseOptions("Surface Inputs", Float) = 0
		[Sub(_SurfaceInputs)][MainTexture] _BaseMap("Base Map", 2D) = "white" {}
		[Sub(_SurfaceInputs)][MainColor][HDR] _BaseColor("Base Color", Color) = (1,1,1,1)
        [ExtendToggle(_SurfaceInputs, _, _BackFaceBaseColor)] _EnableBackFaceBaseColor("Enable BackFace Base Color", Float) = 0
        [HideInInspector][HDR]_BackFaceBaseColor("BackFace Base Color", Color) = (1,1,1,1)
        [SubToggle(_SurfaceInputs)] _ApplyVertexColor("Apply Vertex Color", Float) = 1
        [KWEnum(_SurfaceInputs, UV, _, ScreenUV, _BASEMAP_SCREEN_UV, ReflectionUV, _BASEMAP_REFLECTION_UV)] _BaseMapUvType("BaseMap UvType", float) = 0
        [Sub(_SurfaceInputs)] _BaseMapRotation("BaseMap Rotation", Range(0.0, 1.0)) = 0.0
        [Sub(_SurfaceInputs)] _BaseMapUSpeed("BaseMap U Speed", Float) = 0
		[Sub(_SurfaceInputs)] _BaseMapVSpeed("BaseMap V Speed", Float) = 0
        [SubToggle(_SurfaceInputs)] _BaseMapCustomSpeed("BaseMap Custom Speed (CustomData1.xy, UV0.zw)", Float) = 0
        [Tex(_SurfaceInputs, _NormalScale)]_NormalMap("Normal Map (RG)", 2D) = "white" {}
        [HideInInspector]_NormalScale("Normal Scale (RG)", Float) = 1.0

        [Main(_ReceiveLightOptions, _ENABLE_LIGHT_AFFECT)]_ReceiveLightOptions("RealTime Light Options", Float) = 0
        [Sub(_ReceiveLightOptions)]_RealTimeLightStrength("Real Time Light Strength", range(0, 1)) = 1
        [Sub(_ReceiveLightOptions)]_RealTimeShadowStrength("Real Time Shadow Strength", range(0, 1)) = 1
        [Sub(_ReceiveLightOptions)][HDR]_RealTimeShadowColor("Real Time Shadow Color", Color) = (0,0,0,1)
        [Sub(_ReceiveLightOptions)]_RealTimeShadowColorStrength("Real Time Shadow Color Strength", range(0, 1)) = 0

        [Main(_MaskOptions, _MASK_MAP)]_MaskOptions("Mask Options", Float) = 0
		[Sub(_MaskOptions)] _MaskMap("Mask Map (R)", 2D) = "white" {}
        [Channel(_MaskOptions)]_MaskChannelMask("Mask Channel Mask (Default R)", Vector) = (1,0,0,0)
        [Sub(_MaskOptions)] _MaskRotation("Mask Rotation", Range(0.0, 1.0)) = 0.0
        [Sub(_MaskOptions)] _MaskUSpeed("Mask U Speed", Float) = 0
		[Sub(_MaskOptions)] _MaskVSpeed("Mask V Speed", Float) = 0
        [SubToggle(_MaskOptions)] _MaskCustomSpeed("Mask Custom Speed (CustomData1.zw, UV1.xy)", Float) = 0
        [Sub(_MaskOptions)] _MaskIntensity ("Mask Intensity", Float) = 1
        [Sub(_MaskOptions)] _MaskMinValue ("Mask Min", Range(0, 1)) = 0
        [Sub(_MaskOptions)] _MaskMaxValue ("Mask Max", Range(1, 100)) = 1
        
        [Main(_DistortionOptions, _DISTORTION_MAP)]_DistortionOptions("Distortion Options", Float) = 0
		[Sub(_DistortionOptions)] _DistortionMap("Distortion Map (RG)", 2D) = "white" {}
        [Sub(_DistortionOptions)] _DistortionRotation("Distortion Rotation", Range(0.0, 1.0)) = 0.0
        [Sub(_DistortionOptions)] _DistortionUSpeed("Distortion U Speed", Float) = 0
		[Sub(_DistortionOptions)] _DistortionVSpeed("Distortion V Speed", Float) = 0
        [Sub(_DistortionOptions)] _DistortionIntensity("Distortion Intensity", Float) = 1
        [ExtendSub(_DistortionOptions, _DISTORTION_MASK_MAP)] _DistortionMaskMap("Distortion Mask Map (R)", 2D) = "white" {}
        [Channel(_DistortionOptions)]_DistortionMaskChannelMask("Distortion Mask Channel Mask (Default R)", Vector) = (1,0,0,0)

        [Main(_DissolveOptions, _DISSOLVE_MAP)]_DissolveOptions("Dissolve Options", Float) = 0
        [Sub(_DissolveOptions)] _DissolveMap ("Dissolve Map (R)", 2D) = "white" {}
        [Channel(_DissolveOptions)] _DissolveChannelMask("Dissolve Channel Mask (Default R)", Vector) = (1,0,0,0)
        [Sub(_DissolveOptions)] _DissolveRotation("Dissolve Rotation", Range(0.0, 1.0)) = 0.0
	    [Sub(_DissolveOptions)] _DissolveUSpeed("Dissolve U Speed", Float) = 0
	    [Sub(_DissolveOptions)] _DissolveVSpeed("Dissolve V Speed", Float) = 0
        [Sub(_DissolveOptions)] _DissolveIntensity ("Dissolve Intensity", Range(0, 1)) = 0
        [Sub(_DissolveOptions)] _DissolveWidth ("Dissolve Width", Range(0, 1)) = 0.001
        [Sub(_DissolveOptions)][HDR]_DissolveEdgeColor("Dissolve Edge Color", Color) = (1, 1, 1, 1)
        [Sub(_DissolveOptions)] _DissolveEdgeIntensity("Dissolve Edge Intensity", Float) = 1
        [SubToggle(_DissolveOptions)] _DissolveHardEdge("Dissolve Hard Edge", Float) = 1
        [SubToggle(_DissolveOptions)] _DissolveCustomIntensity("Dissolve Custom Intensity (CustomData2.x, UV1.z)", Float) = 0
        [SubToggle(_DissolveOptions)] _DissolveCustomWidth("Dissolve Custom Width (CustomData2.y, UV1.w)", Float) = 0

        [Main(_MatCapGroup, _MATCAP)]_MatCapGroup("MatCap Options", Float) = 0
        [Tex(_MatCapGroup)]_Matcap("Matcap", 2D) = "white" {}
        [Sub(_MatCapGroup)]_MatcapUVScale("MatcapUVScale", range(0.01, 1)) = 1
        [Sub(_MatCapGroup)]_MatcapStrength("Matcap Strength", range(0.01, 3)) = 1

        [Main(_RimLightOptions, _RIM_LIGHT)] _RimLightOptions ("RimLight Options", Float) = 0
        [Sub(_RimLightOptions)] [HDR]_RimLightColor("RimLight Color", Color) = (1, 1, 1, 1)
        [Sub(_RimLightOptions)] _RimLightWidth("RimLight Width", Range(0.01, 30)) = 1
        [Sub(_RimLightOptions)] _RimLightSmoothness("RimLight Smoothness", Range(1, 100)) = 1
        [Sub(_RimLightOptions)] _RimLightIntensity("RimLight Intensity", Float) = 1
        [Sub(_RimLightOptions)] _RimLightMinValue ("RimLight Min", Range(0, 1)) = 0
        [Sub(_RimLightOptions)] _RimLightMaxValue ("RimLight Max", Range(1, 100)) = 1
        [SubToggle(_RimLightOptions)] _RimLightReverse("RimLight Reverse", Float) = 0
        [SubToggle(_RimLightOptions)] _EnableRimLightVertexColorMask("Enable RimLight Vertex Color Mask", Float) = 0
        [Channel(_RimLightOptions)] _RimLightVertexColorMask("RimLight Vertex Color Mask (Default R)", Vector) = (1,0,0,0)

        [Main(_EmissionOptions, _EMISSION)]_EmissionOptions("Emission Options", Float) = 0
		[Sub(_EmissionOptions)]_EmissionMap("Emission Map", 2D) = "white" {}
        [Sub(_EmissionOptions)][HDR]_EmissionColor("EmissionColor", Color) = (0,0,0)
        [SubToggle(_EmissionOptions)] _EmissionWorldPosUV("Use WorldPos As UV", Float) = 0
        [Sub(_EmissionOptions)] _EmissioUSpeed("Emission U Speed", Float) = 0
	    [Sub(_EmissionOptions)] _EmissioVSpeed("Emission V Speed", Float) = 0
        [Sub(_EmissionOptions)]_EmissionIntensity("Emission Intensity", Range(0.01, 10)) = 1
        [Title(_EmissionOptions, Cubemap)]
        [Tex(_EmissionOptions, _EmissionCubemapColor)]_EmssionCubemap("Emssion Cubemap", Cube) = "" {}
        [HideInInspector][HDR]_EmissionCubemapColor("EmissionCubemapColor", Color) = (1,1,1,1)
        [Sub(_EmissionOptions)]_EmissionCubemapIntensity("Emission Cubemap Intensity", Range(0.01, 10)) = 1
        [Sub(_EmissionOptions)]_EmissionCubemapLod("Emission Cubemap Lod", Range(0, 10)) = 0

        [Main(_DepthFadeOptions, _DEPTH_FADE)]_DepthFadeOptions("DepthFade Options (Transparent)", Float) = 0
        [Sub(_DepthFadeOptions)] _DepthFadeNear("Depth Fade Near", Float) = 1.0
        [Sub(_DepthFadeOptions)] _DepthFadeFar("Depth Fade Far", Float) = 10.0

        [Main(_AdvancedOptions, _, on, off)]_AdvancedOptions("Advanced Options", Float) = 0
        [SubToggle(_AdvancedOptions, _ENABLE_FOG)] _EnableFog("Enable Fog", float) = 1
	    [SubToggle(_AdvancedOptions, CUSTOM_FOG_FRAGMENT)] _CustomFogFragment("Custom Fog Use Fragment", float) = 0 
        [ExtendToggle(_AdvancedOptions, _DEPTH_BIAS, _DepthBias)] _EnableDepthBias("Enable Depth Bias", Float) = 0.0
        [HideInInspector][Sub(_AdvancedOptions)]_DepthBias("Depth Bias", Range(-1.0, 1.0)) = 0
        [ExtendToggle(_AdvancedOptions, _GLOBAL_CLOUD_SHADOW, _CloudShadowIntensity)] _GlobalCloudShadow("Global Cloud Shadow", float) = 0
        [HideInInspector][Sub(_AdvancedOptions)] _CloudShadowIntensity("Global Cloud Shadow Intensity", Range(0, 1)) = 1
        [Sub(_AdvancedOptions)] _AdjustColorIntensity("Global Adjust Color Intensity", Range(0, 1)) = 1
        [SubIntRange(_AdvancedOptions)] _QueueOffset("Priority", Range(-50, 50)) = 0

        // ObsoleteProperties
        [HideInInspector] _MainTex("BaseMap", 2D) = "white" {}
        [HideInInspector] _Color("Base Color", Color) = (0.5, 0.5, 0.5, 1)
        
        [HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
        //PicoVideo;OptimizedOverlayAlphaBlend;WuJunLin;Start
        [HideInInspector] _SrcBlendAlpha("__srcAlpha", Float) = 1.0
        [HideInInspector] _DstBlendAlpha("__dstAlpha", Float) = 0.0
        //PicoVideo;OptimizedOverlayAlphaBlend;WuJunLin;End
		//[HideInInspector] _ZWrite("__zw", Float) = 1.0
    }

    SubShader
	{
        Tags 
		{
            "RenderType" = "Opaque" 
            "RenderPipeline" = "UniversalPipeline" 
            "IgnoreProjector" = "True"
        }

        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
        #include "Common.hlsl"

        #if _BASEMAP_REFLECTION_UV || _MATCAP || _RIM_LIGHT || (_EMISSION && _EMISSION_CUBEMAP)
            #define REQUIRES_WORLDNORMAL
        #endif
    
        #if defined(REQUIRES_WORLDNORMAL) && _NORMALMAP
            #define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
        #endif

        #if _NORMALMAP
            TEXTURE2D(_NormalMap);   SAMPLER(sampler_NormalMap);
        #endif

        #if _MASK_MAP
            TEXTURE2D(_MaskMap);   SAMPLER(sampler_MaskMap);
        #endif

        #if _DISTORTION_MAP
            TEXTURE2D(_DistortionMap);   SAMPLER(sampler_DistortionMap);
            #if _DISTORTION_MASK_MAP
                TEXTURE2D(_DistortionMaskMap);   SAMPLER(sampler_DistortionMaskMap);
            #endif
        #endif

        #if _DISSOLVE_MAP
            TEXTURE2D(_DissolveMap);   SAMPLER(sampler_DissolveMap);
        #endif

        #if _MATCAP
            TEXTURE2D(_Matcap);      SAMPLER(sampler_Matcap);
        #endif

        #if _EMISSION_CUBEMAP
            TEXTURECUBE(_EmssionCubemap);	SAMPLER(sampler_EmssionCubemap);
        #endif
        
        CBUFFER_START(UnityPerMaterial)
            half _Surface;
            half _Cutoff;
            half _DepthBias;
            half _DecolorationIntensity;

            half4 _BaseMap_ST;
            half4 _BaseColor;
            half _EnableBackFaceBaseColor;
            half4 _BackFaceBaseColor;
            half _ApplyVertexColor;
            half _BaseMapRotation;
            half _BaseMapUSpeed;
            half _BaseMapVSpeed;
            half _BaseMapCustomSpeed;
            half _NormalScale;

            half _RealTimeLightStrength;
            half _RealTimeShadowStrength;
            half3 _RealTimeShadowColor;
            half _RealTimeShadowColorStrength;

            half4 _MaskMap_ST;
            half4 _MaskChannelMask;
            half _MaskRotation;
            half _MaskUSpeed;
            half _MaskVSpeed;
            half _MaskCustomSpeed;
            half _MaskIntensity;
            half _MaskMinValue;
            half _MaskMaxValue;

            half4 _DistortionMap_ST;
            half _DistortionRotation;
            half _DistortionUSpeed;
            half _DistortionVSpeed;
            half _DistortionIntensity;
            half4 _DistortionMaskMap_ST;
            half4 _DistortionMaskChannelMask;

            half4 _DissolveMap_ST;
            half4 _DissolveChannelMask;
            half _DissolveRotation;
	        half _DissolveUSpeed;
	        half _DissolveVSpeed;
            half _DissolveIntensity;
            half _DissolveWidth;
            half4 _DissolveEdgeColor;
            half _DissolveEdgeIntensity;
            half _DissolveHardEdge;
            half _DissolveCustomIntensity;
            half _DissolveCustomWidth;

            half _MatcapUVScale;
            half _MatcapStrength;
            half _Metallic;
            half _MatCapHighLightsStrength;

            half3 _RimLightColor;
            half _RimLightIntensity;
            half _RimLightWidth;
            half _RimLightSmoothness;
            half _RimLightMinValue;
            half _RimLightMaxValue;
            half _RimLightReverse;
            half _EnableRimLightVertexColorMask;
            half4 _RimLightVertexColorMask;

            half4 _EmissionMap_ST;
            half3 _EmissionColor;
            half _EmissionWorldPosUV;
            half _EmissioUSpeed;
            half _EmissioVSpeed;
            half _EmissionIntensity;
            half4 _EmissionCubemapColor;
            half _EmissionCubemapIntensity;
            half _EmissionCubemapLod;

            half _DepthFadeNear;
            half _DepthFadeFar;

            half _CloudShadowIntensity;
            half _AdjustColorIntensity;
        CBUFFER_END

        ENDHLSL

        Pass
		{
            Name "ForwardUberEffects"

            Blend[_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha] //PicoVideo;OptimizedOverlayAlphaBlend;WuJunLin
            ZWrite[_ZWrite]
            ZTest [_ZTest]
            Cull [_Cull]
            ColorMask [_ColorMask]

            Stencil
		    {
			    Ref [_StencilRef]
			    Comp [_StencilComp]
			    Pass [_StencilPass]
                Fail [_StencilFail]
		    }

            Lighting Off

			Tags
            {
                "LightMode" = "UniversalForward"
            }
			
            HLSLPROGRAM

            #pragma prefer_hlslcc gles
            #pragma target 2.0

            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_vertex _BILLBOARD
            #pragma shader_feature_local _ENABLE_LIGHT_AFFECT
            #pragma shader_feature_local _ _BASEMAP_SCREEN_UV _BASEMAP_REFLECTION_UV
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _MASK_MAP
            #pragma shader_feature_local _DISTORTION_MAP
            #pragma shader_feature_local _DISTORTION_MASK_MAP
            #pragma shader_feature_local _DISSOLVE_MAP
            #pragma shader_feature_local _MATCAP
            #pragma shader_feature_local _RIM_LIGHT
            #pragma shader_feature_local _EMISSION
            #pragma shader_feature_local _EMISSION_CUBEMAP
            #pragma shader_feature_local _DEPTH_FADE
            #pragma shader_feature_local_vertex _DEPTH_BIAS
            #pragma shader_feature_local _ENABLE_FOG
            #pragma shader_feature_local_fragment _DISCOLORATION

            // -------------------------------------
            // Universal Pipeline keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT

            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            // #pragma multi_compile _ CUSTOM_FOG
            #pragma shader_feature _ CUSTOM_FOG_FRAGMENT
            #pragma shader_feature_local_fragment _GLOBAL_CLOUD_SHADOW

            #ifdef CUSTOM_FOG
			    #include "Packages/org.byted.vrtv.engine.ecosystem-simulate/Runtime/FogSimulate/Resource/Shaders/Fog.hlsl"
            #endif

            //全局云投影
            #if _GLOBAL_CLOUD_SHADOW
	            #include "Packages/org.byted.vrtv.engine.ecosystem-simulate/Runtime/CloudShadowSimulate/Resource/Shaders/CloudShadow.hlsl"
            #endif

            #pragma vertex UberEffectsVert
            #pragma fragment UberEffectsFrag

            struct Attributes
            {
                float3 positionOS : POSITION;

                #if defined(REQUIRES_WORLDNORMAL)
                    float3 normalOS : NORMAL;
                    float4 tangentOS : TANGENT;
                #endif

                float4 uvAndCustom1XY: TEXCOORD0;//xy:uv, zw:customData1.xy
                float4 custom1ZWAndCustom2XY: TEXCOORD1;//xy:customData1.zw, zw:customData2.xy

                half4 color : COLOR;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            struct Varyings
            {
                float4 uv: TEXCOORD0;//xy:basemap, zw:mask

                float4 uv2: TEXCOORD1;//xy:distortion, zw:dissolve

                float4 uv3: TEXCOORD2;//x:dissolve intensity, y:dissolve width, zw:emission

                float2 uv4: TEXCOORD3;//xy:distortion mask

                #if defined(REQUIRES_WORLDNORMAL)
                    float3 normalWS     : TEXCOORD4;
                #endif

                #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
                    float4 tangentWS : TEXCOORD5;    // xyz: tangent, w: sign
                #endif

                float3 positionWS    : TEXCOORD6;

                float4 viewDirWSAndFogFactor    : TEXCOORD7;

                #if _BASEMAP_SCREEN_UV || _DEPTH_FADE
                    float4 screenPos : TEXCOORD8;
                #endif

                #if _MAIN_LIGHT_SHADOWS && _ENABLE_LIGHT_AFFECT
                    float4 shadowCoord              : TEXCOORD9;
                #endif

                half4 color : COLOR;

                float4 positionCS : SV_POSITION;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
             float4 CalculateContrast( float contrastValue, float4 colorTarget )
			{
				float t = 0.5 * ( 1.0 - contrastValue );
				return mul( float4x4( contrastValue,0,0,t,  0,contrastValue,0,t,  0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
			}

            Varyings UberEffectsVert(Attributes input)
            {
                Varyings output = (Varyings) 0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
               
                float2 uv = input.uvAndCustom1XY.xy;
                float4 custom1 = float4(input.uvAndCustom1XY.zw, input.custom1ZWAndCustom2XY.xy);
                float2 custom2 = input.custom1ZWAndCustom2XY.zw;

                float2 uvOffsetBase = _BaseMapCustomSpeed * custom1.xy + (1 - _BaseMapCustomSpeed) * float2(_BaseMapUSpeed, _BaseMapVSpeed) * _Time.y;
                output.uv.xy = TRANSFORM_TEX(RotateUV(uv, _BaseMapRotation * PI * 2), _BaseMap) + uvOffsetBase;
                
                #if _MASK_MAP
                    float2 uvOffsetMask = _MaskCustomSpeed * custom1.zw + (1 - _MaskCustomSpeed) * float2(_MaskUSpeed, _MaskVSpeed) * _Time.y;
                    output.uv.zw = TRANSFORM_TEX(RotateUV(uv, _MaskRotation * PI * 2), _MaskMap) + uvOffsetMask;
                #endif

                #if _DISTORTION_MAP
                    float2 uvOffsetFlow = float2(_DistortionUSpeed, _DistortionVSpeed) * _Time.y;
                    output.uv2.xy = TRANSFORM_TEX(RotateUV(uv, _DistortionRotation * PI * 2), _DistortionMap) + uvOffsetFlow;
                    #if _DISTORTION_MASK_MAP
                        output.uv4 = TRANSFORM_TEX(uv, _DistortionMaskMap);
                    #endif                
                #endif

                #if _DISSOLVE_MAP
                    float2 uvOffsetDisslove = float2(_DissolveUSpeed, _DissolveVSpeed) * _Time.y;
                    output.uv2.zw = TRANSFORM_TEX(RotateUV(uv, _DissolveRotation * PI * 2), _DissolveMap) + uvOffsetDisslove;
                    output.uv3.x = _DissolveCustomIntensity * custom2.x + _DissolveIntensity * (1 - _DissolveCustomIntensity);
                    output.uv3.y = _DissolveCustomWidth * custom2.y + _DissolveWidth * (1 - _DissolveCustomWidth);
                #endif

                #if _BILLBOARD
                    float3 quadPivotPosOS = float3(0,0,0);
                    float3 quadPivotPosWS = TransformObjectToWorld(quadPivotPosOS);
                    float3 quadPivotPosVS = TransformWorldToView(quadPivotPosWS);

	                float3 worldScale = float3(
    	                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
    	                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
    	                length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
                    );

                    float3 posVS = quadPivotPosVS + float3(input.positionOS * worldScale);//recontruct quad 4 points in view space

	                output.positionWS = quadPivotPosWS + float3(input.positionOS * worldScale);
                    output.positionCS = TransformWViewToHClip(posVS);
                #else
	                output.positionWS = TransformObjectToWorld(input.positionOS);
	                output.positionCS = TransformWorldToHClip(output.positionWS);
                #endif

                output.viewDirWSAndFogFactor.xyz = GetWorldSpaceViewDir(output.positionWS);
                #if defined(REQUIRES_WORLDNORMAL)
                    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                    output.normalWS = normalInput.normalWS;
                #endif

                #if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
                    real sign = input.tangentOS.w * GetOddNegativeScale();
                    output.tangentWS = half4(normalInput.tangentWS.xyz, sign);
                #endif

                #if _DEPTH_BIAS
                    output.positionCS = GetNewClipPosWithZOffsetVS(output.positionCS, _DepthBias);
                #endif

                #if _EMISSION
                    half2 uvOffsetEmission = half2(_EmissioUSpeed, _EmissioVSpeed) * _Time.y;
                    output.uv3.zw = TRANSFORM_TEX(_EmissionWorldPosUV ? output.positionWS.xy : uv, _EmissionMap) + uvOffsetEmission;
                #endif

                #if _BASEMAP_SCREEN_UV || _DEPTH_FADE
                     output.screenPos = ComputeScreenPos(output.positionCS);
                #endif

                #if _MAIN_LIGHT_SHADOWS && _ENABLE_LIGHT_AFFECT
                    output.shadowCoord = TransformWorldToShadowCoord(output.positionWS);
                #endif

                #if _ENABLE_FOG
                    #if CUSTOM_FOG
                        output.viewDirWSAndFogFactor.w = FogVert(output.positionWS);
                    #else
                        output.viewDirWSAndFogFactor.w = ComputeFogFactor(output.positionCS.z);
                    #endif
                #endif

                output.color = input.color;

                return output;
            }

            half4 UberEffectsFrag(Varyings input, half facing : VFACE) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                #if _DISTORTION_MAP
                    half2 distortion = SAMPLE_TEXTURE2D(_DistortionMap, sampler_DistortionMap, input.uv2.xy).xy;
                    distortion = distortion * 2 - 1;
                    distortion *= _DistortionIntensity;

                    #if _DISTORTION_MASK_MAP
                        half distortionMask = dot(SAMPLE_TEXTURE2D(_DistortionMaskMap, sampler_DistortionMaskMap, input.uv4), _DistortionMaskChannelMask);
                        distortion *= distortionMask;
                    #endif
                #else
                    half2 distortion = half2(0, 0);
                #endif

                #if _BASEMAP_REFLECTION_UV || _RIM_LIGHT || CUSTOM_FOG || (_EMISSION && _EMISSION_CUBEMAP)
                    half3 viewDirWS = SafeNormalize(input.viewDirWSAndFogFactor.xyz);
                #endif

                //计算世界空间法线
                #if defined(REQUIRES_WORLDNORMAL)
                    #if _NORMALMAP
                        float sgn = input.tangentWS.w;      // should be either +1 or -1
                        float3 bitangent = sgn * cross(input.normalWS, input.tangentWS.xyz);
                        half3 normalTS = SampleNormalRG(input.uv.xy, TEXTURE2D_ARGS(_NormalMap, sampler_NormalMap), _NormalScale);
                        half3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS));
                    #else
                        half3 normalWS = input.normalWS;
                    #endif

                    normalWS = NormalizeNormalPerPixel(normalWS * facing);
                #endif

                #if _BASEMAP_REFLECTION_UV || (_EMISSION && _EMISSION_CUBEMAP)
                    half3 reflectVector = reflect(-viewDirWS, normalWS);
                #endif

                //三种不同的BaseMapUV模式
                #if _BASEMAP_SCREEN_UV
                    float2 baseMapUV = TRANSFORM_TEX((input.screenPos.xy / input.screenPos.w), _BaseMap);
                #elif _BASEMAP_REFLECTION_UV
                    float2 baseMapUV = TRANSFORM_TEX(reflectVector.xz, _BaseMap) + half2(_BaseMapUSpeed, _BaseMapVSpeed) * _Time.y;
                #else
                    float2 baseMapUV = input.uv.xy;
                #endif

                half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, baseMapUV + distortion) * (_ApplyVertexColor * input.color + (1 - _ApplyVertexColor));
                baseColor *= lerp(_BaseColor, lerp(_BackFaceBaseColor, _BaseColor, (facing + 1) / 2), _EnableBackFaceBaseColor);//新增背面颜色设置

                #if _ALPHATEST_ON
                    clip(baseColor.a - _Cutoff);
                #endif

                #if _GLOBAL_CLOUD_SHADOW
                    baseColor.rgb = ApplyGlobalCloudShadow(baseColor.rgb, input.positionWS, _CloudShadowIntensity);
	            #endif

                half4 color = baseColor;

                #if _MATCAP
                    float3 viewNormal = mul((float3x3)GetWorldToViewMatrix(), normalWS);
                    float2 matCapUV = viewNormal.xy * 0.5 * _MatcapUVScale + 0.5;
                    half3 matcap = SAMPLE_TEXTURE2D(_Matcap, sampler_Matcap, matCapUV).rgb;
                    color.rgb *= matcap * _MatcapStrength;
                #endif

                #if _MAIN_LIGHT_SHADOWS && _ENABLE_LIGHT_AFFECT
		            Light mainLight = GetMainLight(input.shadowCoord);
		            color.rgb *= GetAdjustLightColor(mainLight, _RealTimeLightStrength, _RealTimeShadowStrength, _RealTimeShadowColor, _RealTimeShadowColorStrength);
	            #endif

                #if _DISSOLVE_MAP
                    half disslove = dot(SAMPLE_TEXTURE2D(_DissolveMap, sampler_DissolveMap, input.uv2.zw + distortion), _DissolveChannelMask);
                    half dissloveIntensity = input.uv3.x;
                    half dissolveWidth = input.uv3.y;
                    half dissolveWithParticle = (dissloveIntensity * (1 + dissolveWidth) - dissolveWidth);
                    half dissolveAlpha = saturate(smoothstep(dissolveWithParticle, (dissolveWithParticle + dissolveWidth), disslove));
                    color.a *= _DissolveHardEdge ? sign(dissolveAlpha) : dissolveAlpha;
                    color.rgb += _DissolveEdgeColor.rgb * _DissolveEdgeColor.a * _DissolveEdgeIntensity * (_DissolveHardEdge ? sign(1 - dissolveAlpha) : (1 - dissolveAlpha));
                #endif

                //美术要求Mask在溶解之后，不受扭曲影响
                #if _MASK_MAP
                    half mask = dot(SAMPLE_TEXTURE2D(_MaskMap, sampler_MaskMap, input.uv.zw), _MaskChannelMask) * _MaskIntensity;
                    mask = smoothstep(_MaskMinValue, _MaskMaxValue, mask);
                    color = half4(color.rgb, saturate(color.a * mask));
                #endif

                #if _RIM_LIGHT
                    half cosTheta = dot(normalWS, viewDirWS);
                    half rimLightStrength = pow(saturate(1 - 1 / _RimLightWidth * cosTheta), _RimLightSmoothness);
                    rimLightStrength = (_RimLightReverse ? 1 - rimLightStrength : rimLightStrength) * _RimLightIntensity;
                    rimLightStrength = smoothstep(_RimLightMinValue, _RimLightMaxValue, rimLightStrength);
                    rimLightStrength *= _EnableRimLightVertexColorMask * dot(input.color, _RimLightVertexColorMask) + (1 - _EnableRimLightVertexColorMask);
                    color.rgb += _RimLightColor * rimLightStrength;
                #endif

                #if _EMISSION
                    half3 emission = SAMPLE_TEXTURE2D(_EmissionMap, sampler_EmissionMap, input.uv3.zw + distortion).rgb * _EmissionColor * _EmissionIntensity;
                    color.rgb += emission;
                    #if _EMISSION_CUBEMAP
			            half3 cubemapEmission = SAMPLE_TEXTURECUBE_LOD(_EmssionCubemap, sampler_EmssionCubemap, reflectVector, _EmissionCubemapLod).rgb;
			            color.rgb += cubemapEmission * _EmissionCubemapColor.rgb * _EmissionCubemapIntensity;
		            #endif
                #endif
                
                 #if _DISCOLORATION // 去色 ： R*0.3+G*0.59+B*0.11   , 
                    half grayColor = Luminance(color.rgb);
                    half4 newGrayColor = half4(grayColor, grayColor, grayColor ,1 );
                    newGrayColor = saturate( CalculateContrast( _DecolorationIntensity , newGrayColor));
                    color = half4(newGrayColor.rgb,color.a);
                #endif
                
                #if _DEPTH_FADE
                     float depth = LinearEyeDepth(input.screenPos.z / input.screenPos.w, _ZBufferParams);
                     half fade = saturate((_DepthFadeFar - depth) / (_DepthFadeFar - _DepthFadeNear));
                     color.a = lerp(0, color.a, fade);
                #endif

                #if _ENABLE_FOG
                    #if CUSTOM_FOG
                        color.rgb = FogFrag(color.rgb, viewDirWS, input.positionWS, input.viewDirWSAndFogFactor.w);
                    #else
                        color.rgb = MixFog(color.rgb, input.viewDirWSAndFogFactor.w);
                    #endif
                #endif

                return color;
            }

            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0
            Cull[_Cull]

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma target 2.0

            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local_fragment _ALPHATEST_ON

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthOnlyPass.hlsl"
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma target 2.0

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            ENDHLSL
        }
    }

    FallBack "Hidden/Universal Render Pipeline/FallbackError"
    CustomEditor "Matrix.CommonShader.UberEffectsShaderGUI"
}