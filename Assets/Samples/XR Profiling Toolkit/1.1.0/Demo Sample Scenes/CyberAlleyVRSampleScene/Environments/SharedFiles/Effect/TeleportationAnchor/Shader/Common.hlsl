//
//      __  __       _______ _____  _______   __
//      |  \/  |   /\|__   __|  __ \|_   _\ \ / /
//      | \  / |  /  \  | |  | |__) | | |  \ V /
//      | |\/| | / /\ \ | |  |  _  /  | |   > <
//      | |  | |/ ____ \| |  | | \ \ _| |_ / . \
//      |_|  |_/_/    \_\_|  |_|  \_\_____/_/ \_\
//									   (ByteDance)
//
//      Created by Matrix team.
//      Procedural LOGO:https://www.shadertoy.com/view/ftKBRW
//
//      The team was set up on September 4, 2019.
//

#ifndef MATRIX_COMMON_SHADER_COMMON_INCLUDED
#define MATRIX_COMMON_SHADER_COMMON_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

uniform float4 _CustomDirectionLightData0;  // x : intensity yzw : direction
uniform float4 _CustomDirectionLightData1;  // xyz : color

half3 SampleNormalRG(float2 uv, TEXTURE2D_PARAM(bumpMap, sampler_bumpMap), half scale = half(1.0))
{
#ifdef _NORMALMAP
    half4 n = SAMPLE_TEXTURE2D(bumpMap, sampler_bumpMap, uv);
    return UnpackNormalRG(n, scale);
#else
    return half3(0.0h, 0.0h, 1.0h);
#endif
}

half4 SampleAlbedoAlpha(float2 uv, TEXTURE2D_PARAM(albedoAlphaMap, sampler_albedoAlphaMap), float mipmapBias)
{
    return SAMPLE_TEXTURE2D_BIAS(albedoAlphaMap, sampler_albedoAlphaMap, uv, mipmapBias);
}

half4 SampleAlbedoAlphaClip(float2 uv, TEXTURE2D_PARAM(baseMap, sampler_baseMap), half4 baseColor, half cutoff)
{
    half4 albedoAlpha = SAMPLE_TEXTURE2D(baseMap, sampler_baseMap, uv) * baseColor;

#if _ALPHATEST_ON
    clip(albedoAlpha.a - cutoff);
#endif

	return albedoAlpha;
}

float2 RotateUV(float2 uv, half angle, half2 offsets = half2(0.0, 0.0))
{
    half angleCos = cos(angle);
    half angleSin = sin(angle);
    half2x2 rotateMatrix = half2x2(angleCos, -angleSin, angleSin, angleCos);
    half2 UvOffsets = 0.5 - offsets;
    return mul(uv - UvOffsets, rotateMatrix) + UvOffsets;
}

float Rand(float co)
{
    return frac(sin(co * 43758.5453));
}

float2 Rand2D(float2 uv)
{
    return frac(sin(uv * float2(12.9898, 78.233)) * 43758.5453);
}

float3 Rand3D(float3 uvw)
{
    return frac(sin(uvw * float3(12.9898, 78.233, 43.2316)) * 43758.5453);
}

half HeightBlend(half input1, half height1, half input2, half height2, half blendFactor)
{
	half heightStart = max(height1, height2) - blendFactor;
	half b1 = max(height1 - heightStart, 0);
	half b2 = max(height2 - heightStart, 0);
	return ((input1 * b1) + (input2 * b2)) / (b1 + b2);
}

float2 FlipBook(float2 uv, float width, float height, int index)
{
    float2 size = float2(1.0f / width, 1.0f / height);
	uint totalFrames = width * height;

	// wrap x and y indexes
	uint indexX = index % width;
	uint indexY = floor((index % totalFrames) / width);

	// get offsets to our sprite index
	float2 offset = float2(size.x * indexX, -size.y * indexY);

	// get single sprite UV
	float2 newUV = uv * size;

	// flip Y (to start 0 from top)
	newUV.y = newUV.y + size.y * (height - 1);

	return newUV + offset;
}

// In Unity, view space look into -Z direction, so
// positive viewSpaceZOffsetAmount means bring vertex depth closer to camera
// negative viewSpaceZOffsetAmount means push vertex depth away from camera
// *if you just want to use this function, you don't have to understand the math inside in order to use it
inline float4 GetNewClipPosWithZOffsetVSPerspectiveCamera(float4 originalPositionCS, float viewSpaceZOffsetAmount)
{
    // push imaginary vertex in view space
    // use max(near+eps,x) to prevent push over camera's near plane
    float float_Eps = 5.960464478e-8;  // 2^-24, machine epsilon: 1 + EPS = 1 (half of the ULP for 1.0f)
    // _ProjectionParams.y is the camera’s near plane
    float modifiedPositionVS_Z = -max(_ProjectionParams.y + float_Eps, abs(originalPositionCS.w) - viewSpaceZOffsetAmount); 
  
    // we only care mul(UNITY_MATRIX_P, modifiedPositionVS).z, and
    // UNITY_MATRIX_P's Z row's xy is always 0, and
    // positionVS's w is always 1
    // so this is the only math that remains after removing all the useless math that won't affect calculating mul(UNITY_MATRIX_P, modifiedPositionVS).z
    float modifiedPositionCS_Z = modifiedPositionVS_Z * UNITY_MATRIX_P[2].z + UNITY_MATRIX_P[2].w;

    // when this function received an originalPositionCS.xyzw and we want to apply viewspace ZOffset,
    // we can't edit it's xy because it will affect vertex position on screen
    // we can't edit it's w because positionCS.w will be used by w division later in hardware, which also affect ndc's xy vertex position
    // so we can only edit originalPositionCS.z

    // But in order to do a correct view space ZOffset, we need to edit both originalPositionCS's zw
    // So we first "cancel" the hardware w division by * original CLIPw to our new modified CLIPz first
    // then we do the correct w division manually in vertex shader to simulate hardware's w division
    // original NDCz = original CLIPz / original CLIPw

    // [here are the steps to find out the correct positionCS.z to output]
    // our desired NDCz = modified CLIPz / modified CLIPw
    // our desired NDCz = modified CLIPz / modified CLIPw * original CLIPw / original CLIPw
    // our desired NDCz = modified CLIPz * original CLIPw / modified CLIPw / original CLIPw
    // our desired NDCz = (modified CLIPz * original CLIPw / modified CLIPw) / (original CLIPw)
    // our desired NDCz = (modified CLIPz * original CLIPw / -modified VIEWz) / (original CLIPw)
    // so (modified CLIPz * original CLIPw / -modified VIEWz) is our output positionCS.z
    originalPositionCS.z = modifiedPositionCS_Z * originalPositionCS.w / (-modifiedPositionVS_Z); // overwrite positionCS.z

    return originalPositionCS;    
}

inline float4 GetNewClipPosWithZOffsetVSOrthographicCamera(float4 originalPositionCS, float viewSpaceZOffsetAmount)
{
    // since depth buffer is linear when using Orthographic camera
    // just push imaginary vertex linearly and overwrite originalPositionCS.z
    float zoffsetCS = viewSpaceZOffsetAmount / (_ProjectionParams.z-_ProjectionParams.y); // if near plane is really small, use * _ProjectionParams.w ?
    zoffsetCS *= (UNITY_NEAR_CLIP_VALUE > 0 ? 1 : -2); // DirectX ndcZ is [1,0], OpenGL ndcZ is [-1,1]
    originalPositionCS.z = originalPositionCS.z + zoffsetCS;
    return originalPositionCS;
}

// support both Orthographic and Perspective camera projection, 
// always slower than above functions but easier for user if they need to support both cameras
inline float4 GetNewClipPosWithZOffsetVS(float4 originalPositionCS, float viewSpaceZOffsetAmount)
{
    // since instruction count is not high and it is pure ALU, maybe not worth a static uniform branching here, 
    // so we use a?b:c (movc: conditional move) here
    return unity_OrthoParams.w ? GetNewClipPosWithZOffsetVSOrthographicCamera(originalPositionCS,viewSpaceZOffsetAmount) :
        GetNewClipPosWithZOffsetVSPerspectiveCamera(originalPositionCS,viewSpaceZOffsetAmount);
}

inline half4 GetBoxProjectedCubemapReflection(half3 reflectVector, float3 positionWS, float4 cubeMapPosition, float4 cubeMapBoxMin, float4 cubeMapBoxMax, TEXTURECUBE_PARAM(cubeMapTex, cubeMapSampler), half mip)
{
    reflectVector = BoxProjectedCubemapDirection(reflectVector, positionWS, cubeMapPosition, cubeMapBoxMin, cubeMapBoxMax);
    half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(cubeMapTex, cubeMapSampler, reflectVector, mip);

    return encodedIrradiance;
}

half3 GetAdjustLightColor(Light light, half lightStrength, half shadowStrength, half3 shadowColor, half shadowColorStrength)
{
    half shadowAttenuation = lerp(1, light.shadowAttenuation, shadowStrength);//自定义阴影强度
	half3 attenuatedLightColor = light.color * light.distanceAttenuation;
	half3 defaultShadowColor = attenuatedLightColor * shadowAttenuation;//默认阴影颜色
	half3 adjustShadowColor = lerp(attenuatedLightColor, shadowColor, 1 - shadowAttenuation);//自定义阴影颜色：shadowAttenuation为1，表示没有阴影，因此要反过来
	return lerp(1, lerp(defaultShadowColor, adjustShadowColor, shadowColorStrength), lightStrength);
}

inline float GetFragmentEyeDepth(float4 projection)
{
    float depth = projection.z / projection.w;
    #if !UNITY_REVERSED_Z
        depth = depth * 0.5 + 0.5;
    #endif
    return LinearEyeDepth(depth, _ZBufferParams);
}

inline float GetScreenEyeDepth(float2 uv)
{
    float depth = SampleSceneDepth(uv);
    return LinearEyeDepth(depth, _ZBufferParams);
}

#endif