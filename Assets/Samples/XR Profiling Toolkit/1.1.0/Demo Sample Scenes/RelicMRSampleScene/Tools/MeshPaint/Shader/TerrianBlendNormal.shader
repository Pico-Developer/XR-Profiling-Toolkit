Shader "Terrian/Ground/TerrianBlendNormal"
{
    Properties 
    {
        _Splat0 ("Layer 1(RGBA)", 2D) = "white" {}
        _CombinedMap0 ("Layer 1 Combined Map(RGB Normal A HeightMap)", 2D) = "Bump" {}
    	_Smoothness0("Smoothness 1", Range(0, 1)) = 0.5
        _Metallic0("Metallic 1", Range(0, 1)) = 0.05
        _Splat1 ("Layer 2(RGBA)", 2D) = "white" {}
        _CombinedMap1 ("Layer 2 Combined Map(RGB Normal A HeightMap)", 2D) = "Bump" {}
    	_Smoothness1("Smoothness 2", Range(0, 1)) = 0.5
        _Metallic1("Metallic 2", Range(0, 1)) = 0.05
        _Splat2 ("Layer 3(RGBA)", 2D) = "white" {}
        _CombinedMap2 ("Layer 3 Combined Map(RGB Normal A HeightMap)", 2D) = "Bump" {}
    	_Smoothness2("Smoothness 3", Range(0, 1)) = 0.5
        _Metallic2("Metallic 3", Range(0, 1)) = 0.05
        _Control ("Control (RGBA)", 2D) = "white" {}
        _BlendWeight ("Blend Weight", Range(0.001,1)) = 0.2
        
        
        [HideInInspector] _MetallicGlossMap("Metallic", 2D) = "white" {}
		[HideInInspector] _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
		[HideInInspector] _SpecGlossMap("Specular", 2D) = "white" {}
		[HideInInspector] _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
		[HideInInspector] _OcclusionMap("Occlusion", 2D) = "white" {}
		[HideInInspector] _EmissionColor("Color", Color) = (0,0,0)
		[HideInInspector] _EmissionMap("Emission", 2D) = "white" {}
		[HideInInspector] _ReceiveShadows("Receive Shadows", Float) = 1.0
		[HideInInspector] _QueueOffset("Queue offset", Float) = 0.0
		[HideInInspector] _Surface("__surface", Float) = 0.0
		[HideInInspector] _Blend("__blend", Float) = 0.0
		[HideInInspector] _AlphaClip("__clip", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
		[HideInInspector] _Cull("__cull", Float) = 2.0
        
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "SplatCount" = "3"
        }
        
        Pass{
            Tags{"LightMode" = "UniversalForward"}
			Blend[_SrcBlend][_DstBlend]
			ZWrite[_ZWrite]
			Cull[_Cull]
			
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"  
			#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
			// -------------------------------------
			// Material Keywords
			#pragma shader_feature _NORMALMAP
			#pragma shader_feature _ALPHATEST_ON
			#pragma shader_feature _ALPHAPREMULTIPLY_ON
			#pragma shader_feature _EMISSION
			#pragma shader_feature _METALLICSPECGLOSSMAP
			#pragma shader_feature _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
			#pragma shader_feature _OCCLUSIONMAP

			#pragma shader_feature _SPECULARHIGHLIGHTS_OFF
			#pragma shader_feature _ENVIRONMENTREFLECTIONS_OFF
			#pragma shader_feature _SPECULAR_SETUP
			#pragma shader_feature _RECEIVE_SHADOWS_OFF

			// -------------------------------------
			// Universal Pipeline keywords
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile _ _SHADOWS_SOFT
			#pragma multi_compile _ _MIXED_LIGHTING_SUBTRACTIVE
			// -------------------------------------
			// Unity defined keywords
			#pragma multi_compile _ DIRLIGHTMAP_COMBINED
			#pragma multi_compile _ LIGHTMAP_ON
			#pragma multi_compile_fog

			//--------------------------------------
			// GPU Instancing
			#pragma multi_compile_instancing
            
            #pragma vertex vert
            #pragma fragment frag
            
            //额外光照
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            CBUFFER_START(UnityPerMaterial) //变量引入开始
            CBUFFER_END //变量引入结束
            TEXTURE2D(_Control);SAMPLER(sampler_Control);
            TEXTURE2D(_Splat0);SAMPLER(sampler_Splat0);
            TEXTURE2D(_Splat1);SAMPLER(sampler_Splat1);
            TEXTURE2D(_Splat2);SAMPLER(sampler_Splat2);
            TEXTURE2D(_CombinedMap0);SAMPLER(sampler_CombinedMap0);
            TEXTURE2D(_CombinedMap1);SAMPLER(sampler_CombinedMap1);
            TEXTURE2D(_CombinedMap2);SAMPLER(sampler_CombinedMap2);
            TEXTURE2D(_LightmapShadow);SAMPLER(sampler_LightmapShadow);
            //获取截屏图像
			SAMPLER(_CameraOpaqueTexture);
            float4 _Splat0_ST;
            float4 _Splat1_ST;
            float4 _Splat2_ST;
            float4 _CombinedMap0_ST;
            float4 _CombinedMap1_ST;
            float4 _CombinedMap2_ST;
            float4 _Control_ST;
            float _BlendWeight;
            float _Smoothness0;
            float _Metallic0;
            float _Smoothness1;
            float _Metallic1;
            float _Smoothness2;
            float _Metallic2;

            
            struct a2v
            {
                float3 positionOS : POSITION;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
                float2 uv_Control : TEXCOORD0;
                float2 uv_Splat0 : TEXCOORD1;
                float2 uv_Splat1 : TEXCOORD2;
                float2 uv_Splat2 : TEXCOORD3;
                float2 uv_CombinedMap0 : TEXCOORD4;
                float2 uv_CombinedMap1 : TEXCOORD5;
                float2 uv_CombinedMap2 : TEXCOORD6;
                float2 lightmapUV : TEXCOORD7;
            	UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv_Splat0 : TEXCOORD0;
                float2 uv_Splat1 : TEXCOORD1;
                float2 uv_Splat2 : TEXCOORD2;
                float2 uv_CombinedMap0 : TEXCOORD3;
                float2 uv_CombinedMap1 : TEXCOORD4;
                float2 uv_CombinedMap2 : TEXCOORD5;
                float2 uv_Control : TEXCOORD6;
                float2 uvLM : TEXCOORD7;
				DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 8);
				float3 positionWS               : TEXCOORD9;
				float3 normalWS                 : TEXCOORD10;
				float4 tangentWS                : TEXCOORD11;    // xyz: tangent, w: sign
				float3 viewDirWS                : TEXCOORD12;
				half4 fogFactorAndVertexLight   : TEXCOORD13; // x: fogFactor, yzw: vertex light
				float4 positionCS               : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            half4 Blend(half depth1, half depth2, half depth3, half4 control)
            {
                half4 blend;

                blend.r = depth1 * control.r;
                blend.g = depth2 * control.g;
                blend.b = depth3 * control.b;

                half ma = max(blend.r, max(blend.g, max(blend.b, 0)));
                blend = max(blend - ma + _BlendWeight, 0) * control;
                return blend / (blend.r + blend.g + blend.b);
            }
            
            v2f vert(a2v input)
            {
                v2f output;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);

				float3 positionWS = TransformObjectToWorld(input.positionOS);
				float3 positionVS = TransformWorldToView(positionWS);
				float4 positionCS = TransformWorldToHClip(positionWS);

				float4 ndc = positionCS * 0.5f;
				float4 positionNDC;
				positionNDC.xy = float2(ndc.x, ndc.y * _ProjectionParams.x) + ndc.w;
				positionNDC.zw = positionCS.zw;
				real sign = input.tangentOS.w * GetOddNegativeScale();
				float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
				float3 tangentWS = TransformObjectToWorldDir(input.tangentOS.xyz);
				float3 bitangentWS = cross(normalWS, tangentWS) * sign;
				float3 viewDirWS = GetCameraPositionWS() - positionWS;
				half3 vertexLight = VertexLighting(positionWS, normalWS);
				half fogFactor = ComputeFogFactor(positionCS.z);
				output.uvLM = TRANSFORM_TEX(input.lightmapUV, _BaseMap);
				output.normalWS = normalWS;
				output.viewDirWS = viewDirWS;
				output.tangentWS = half4(tangentWS.xyz, sign);
				OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
				OUTPUT_SH(output.normalWS.xyz, output.vertexSH);
				output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
				output.positionWS = positionWS;
				output.positionCS = positionCS;
				output.uv_Control = input.uv_Control;
                output.uv_Splat0 = TRANSFORM_TEX(input.uv_Splat0, _Splat0);
                output.uv_Splat1 = TRANSFORM_TEX(input.uv_Splat1, _Splat1);
                output.uv_Splat2 = TRANSFORM_TEX(input.uv_Splat2, _Splat2);
                output.uv_CombinedMap0 = TRANSFORM_TEX(input.uv_CombinedMap0, _CombinedMap0);
                output.uv_CombinedMap1 = TRANSFORM_TEX(input.uv_CombinedMap1, _CombinedMap1);
                output.uv_CombinedMap2 = TRANSFORM_TEX(input.uv_CombinedMap2, _CombinedMap2);
				return output;
            }

            half3 CDirectBDRF(BRDFData brdfData, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS)
			{
				float3 halfDir = SafeNormalize(float3(lightDirectionWS)+float3(viewDirectionWS));

				float NoH = saturate(dot(normalWS, halfDir));
				half LoH = saturate(dot(lightDirectionWS, halfDir));
				float d = NoH * NoH * brdfData.roughness2MinusOne + 1.00001f;

				half LoH2 = LoH * LoH;
				half specularTerm = brdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * brdfData.normalizationTerm);

				#if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
				specularTerm = specularTerm - HALF_MIN;
				specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
				#endif

				half3 color = specularTerm * brdfData.specular + brdfData.diffuse;
				return color;
			}

			half3 CLightingPhysicallyBased(BRDFData brdfData, half3 lightColor, half3 lightDirectionWS, half lightAttenuation, half3 normalWS, half3 viewDirectionWS)
			{
				half NdotL = saturate(dot(normalWS, lightDirectionWS));
				half3 radiance = lightColor * (lightAttenuation * NdotL);
				return CDirectBDRF(brdfData, normalWS, lightDirectionWS, viewDirectionWS) * radiance;
			}
            
            float4 frag(v2f input) : SV_Target
            {
				half4 splat_control = SAMPLE_TEXTURE2D(_Control, sampler_Control, input.uv_Control).rgba;
            	
                half4 lay1 = SAMPLE_TEXTURE2D(_Splat0, sampler_Splat0, input.uv_Splat0);
                half4 lay2 = SAMPLE_TEXTURE2D(_Splat1, sampler_Splat1, input.uv_Splat1);
                half4 lay3 = SAMPLE_TEXTURE2D(_Splat2, sampler_Splat2, input.uv_Splat2);
                float3 normalFromCombinedMap1 =  UnpackNormal(SAMPLE_TEXTURE2D(_CombinedMap0, sampler_CombinedMap0, input.uv_CombinedMap0));
                float3 normalFromCombinedMap2 =  UnpackNormal(SAMPLE_TEXTURE2D(_CombinedMap1, sampler_CombinedMap1, input.uv_CombinedMap1));
                float3 normalFromCombinedMap3 =  UnpackNormal(SAMPLE_TEXTURE2D(_CombinedMap2, sampler_CombinedMap2, input.uv_CombinedMap2));
                float heightFromCombinedMap1 = SAMPLE_TEXTURE2D(_CombinedMap0, sampler_CombinedMap0, input.uv_CombinedMap0).a; 
                float heightFromCombinedMap2 = SAMPLE_TEXTURE2D(_CombinedMap1, sampler_CombinedMap1, input.uv_CombinedMap1).a; 
                float heightFromCombinedMap3 = SAMPLE_TEXTURE2D(_CombinedMap2, sampler_CombinedMap2, input.uv_CombinedMap2).a; 
               
                half4 blend = Blend(lay1.a, lay2.a, lay3.a, splat_control);
                // 根据比例融合各张贴图的颜色信息
                float4 _BaseColor =half4(lay1.rgba* blend.r + lay2.rgba * blend.g + lay3.rgba * blend.b);
                // 融合粗糙度
                float Smoothness =  (_Smoothness0 * blend.r + _Smoothness1 * blend.g + _Smoothness2 * blend.b);
                // 融合金属度
                float Metallic =  (_Metallic0 * blend.r + _Metallic1 * blend.g + _Metallic2 * blend.b);
                // 融合高度信息
                float fusedHeight = heightFromCombinedMap1 * blend.r + heightFromCombinedMap2 * blend.g + heightFromCombinedMap3 * blend.b;
                // 融合法线向量
                half3 fusedNormal = normalize(normalFromCombinedMap1 * blend.r + normalFromCombinedMap2 * blend.g + normalFromCombinedMap3 * blend.b);
            	
				UNITY_SETUP_INSTANCE_ID(input);
				float2 uv = input.uvLM;
				half alpha = _BaseColor.a;
				clip(alpha - _Cutoff);
				half4 specGloss;

            	specGloss.rgb = Metallic.rrr;
            	specGloss.a = Smoothness;


				float3 albedo =  _BaseColor.rgb;
				float metallic;
            	float specular;
				metallic = specGloss.r;
				specular = half3(0.0h, 0.0h, 0.0h);

				float smoothness = specGloss.a;
            	half4 normalTS = half4( fusedNormal,0);
				float occlusion = SampleOcclusion(uv);
				half3 emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
            	
				half4 positionWS = half4( input.positionWS,1);
				half3 viewDirWS = SafeNormalize(input.viewDirWS);

				float sgn = input.tangentWS.w;      // should be either +1 or -1
				float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
				float4 normalWS = float4(TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz)),0);

				normalWS = float4(NormalizeNormalPerPixel(normalWS),0);
				float3 viewDirectionWS = viewDirWS;


				float4 shadowCoord = TransformWorldToShadowCoord(positionWS);

				half fogCoord = input.fogFactorAndVertexLight.x;
				half3 vertexLighting = input.fogFactorAndVertexLight.yzw;
				half3 bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, normalWS);

				BRDFData brdfData;
				InitializeBRDFData(albedo, metallic, specular, smoothness, alpha, brdfData);

				Light mainLight = GetMainLight(shadowCoord);
				MixRealtimeAndBakedGI(mainLight,normalWS, bakedGI, half4(0, 0, 0, 0));

				float3 color = GlobalIllumination(brdfData, bakedGI, occlusion, normalWS, viewDirectionWS);
				color += CLightingPhysicallyBased(brdfData, mainLight.color, mainLight.direction, mainLight.distanceAttenuation * mainLight.shadowAttenuation, normalWS, viewDirectionWS);
            	
				uint pixelLightCount = GetAdditionalLightsCount();
				for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
				{
					Light light = GetAdditionalLight(lightIndex, positionWS);
					color += LightingPhysicallyBased(brdfData, light,normalWS, viewDirectionWS);
				}
            	
				color += emission;

				color.rgb = MixFog(color.rgb, fogCoord);
				return float4(color, alpha);
            	
            }
            ENDHLSL
        }
    }
}
