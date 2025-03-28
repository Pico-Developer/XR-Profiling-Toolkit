Shader "Terrian/Water/RiverURP"
{
    Properties
    {
        [Header(Depth Gradient)]
        //浅水的颜色
        _DepthShallowColor("Depth Shallow Color", Color) = (0.325, 0.807, 0.971, 0.725)
        //深水的颜色
        _DepthDeepColor("Depth Deep Color", Color) = (0.086, 0.407, 1, 0.749)
        //从浅水到深水过渡的最大距离
        _DepthMaxDistance("Depth Maximum Distance", Float) = 1

        [Header(Foam)]
        //泡沫的颜色
        _FoamColor("Foam Color", Color) = (1,1,1,1)
        //泡沫出现的最大距离
        _FoamMaxDistance("Foam Maximum Distance", Float) = 0.2
        //水面噪声的二维纹理
        _SurfaceNoise("Surface Noise", 2D) = "white" {}
        //表面噪声的截止值
        _SurfaceNoiseCutoff("Surface Noise Cutoff", Range(0, 1)) = 0.777
        //表面噪声扭曲程度
        _SurfaceDistortionScale("Surface Distortion Scale", Range(0, 1)) = 0.27

        [Header(Distortion)]
        _SurfaceDistortion("Surface Distortion", 2D) = "white" {}

        [Header(Caustics)]
        _CausticsColor("Color", Color) = (1,1,1,1)
        _CausticsTex ("Texture", 2D) = "black"{}
        _CausticsScale ("Scale", float) = 1.0
        _CausticsDistortionScale ("Caustics Distortion Scale", Range(0.0, 1.0)) = 0.5

        [Wave]
        [Toggle]_UseWave("Use Wave", int) = 1
        _Xm ("Xm", Range(0, 1)) = 0.72
        _P ("P", Range(0, 1)) = 1
        _IntervalDistance ("IntervalDistance", float) = 5
        _DunesDistance ("DunesDistance", float) = 10
        _HeightScale ("HeightScale", float) = 0.5
        _WaveSpeed ("WaveSpeed", float) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            ZWrite On
            ColorMask 0
        }

        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            CBUFFER_START(UnityPerMaterial)
            
            CBUFFER_END

            struct appdata
            {
                float3 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float2 uvCaustics : TEXCOORD3;
                float2 uvDistortion : TEXCOORD5;
                float2 uvNoise : TEXCOORD6;
            };
            
            TEXTURE2D(_CameraDepthTexture);SAMPLER(sampler_CameraDepthTexture);
            
            //Depth Gradient
            float4 _DepthShallowColor;
            float4 _DepthDeepColor;
            float _DepthMaxDistance;
            

            //Foam
            float3 _FoamColor;
            float _FoamMaxDistance;
            TEXTURE2D(_SurfaceNoise);
            SAMPLER(sampler_SurfaceNoise);
            float4 _SurfaceNoise_ST;
            float _SurfaceNoiseCutoff;
            float _SurfaceDistortionScale;

            //Distortion
            TEXTURE2D(_SurfaceDistortion);
            SAMPLER(sampler_SurfaceDistortion);
            float4 _SurfaceDistortion_ST;
            float _NormalDistortionScale;

            //Caustics
            half3 _CausticsColor;
            TEXTURE2D(_CausticsTex);
            SAMPLER(sampler_CausticsTex);

            float4 _CausticsTex_ST;
            float _CausticsScale;
            float _CausticsDistortionScale;

            //Wave
            int _UseWave = 1;
            float _Xm = 0.72;
            float _P = 1;
            float _IntervalDistance = 5;
            float _DunesDistance = 10;
            float _HeightScale = 0.5;
            float _WaveSpeed = 5;

            float GetWaveHeight(float2 worldPos)
            {
                float pos = worldPos.x + _WaveSpeed * _Time.y;

                float realPos = pos % (_DunesDistance + _IntervalDistance);

                if (realPos > _DunesDistance && realPos < _DunesDistance + _IntervalDistance)
                {
                    return 0;
                }

                float S = 0;
                float x = realPos / _DunesDistance;
                float height;
                if (x < _Xm)
                {
                    height = _HeightScale * (1 - cos(PI * (x - S) / (_Xm - S)));
                }
                else
                {
                    S = 1;
                    height = _HeightScale * (_P * S + 1) * (1 - cos((PI / (_P * S + 1)) * ((x - S) / (_Xm - S))));
                }

                return height;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.uvCaustics = v.uv * _CausticsTex_ST.xy + _CausticsTex_ST.zw * _Time.y;
                o.uv = v.uv;
                o.uvDistortion = v.uv * _SurfaceDistortion_ST.xy + _SurfaceDistortion_ST.zw * _Time.y;
                o.uvNoise = v.uv * _SurfaceNoise_ST.xy + _SurfaceNoise_ST.zw * _Time.y;

                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                if (_UseWave)
                {
                    o.worldPos.y += GetWaveHeight(o.worldPos.xz);
                }

                o.vertex = TransformWorldToHClip(o.worldPos);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float2 distortNoise = SAMPLE_TEXTURE2D(_SurfaceDistortion, sampler_SurfaceDistortion, i.uvDistortion).xy * 2 - 1;
                float2 screenUV = i.vertex.xy / _ScreenParams.xy;
                half depthTex = SAMPLE_TEXTURE2D(_CameraDepthTexture,sampler_CameraDepthTexture,screenUV).x;
                
                //depth color gradient
                float depth = LinearEyeDepth(depthTex,_ZBufferParams);
                float depthOffset = depth - LinearEyeDepth(i.vertex.z,_ZBufferParams);//i.screenPos.w
                float depthOffset01 = saturate(depthOffset / _DepthMaxDistance);
                float4 waterColor = lerp(_DepthShallowColor, _DepthDeepColor, depthOffset01);
   
                //foam color
                float foamOffset01 = saturate(depthOffset / _FoamMaxDistance);
                float surfaceNoiseCutoff = foamOffset01 * _SurfaceNoiseCutoff;
                float2 noiseDistortSample = distortNoise * _SurfaceDistortionScale;
                float2 noiseUV = float2(i.uvNoise.x + noiseDistortSample.x, i.uvNoise.y + noiseDistortSample.y);
                float surfaceNoiseSample = SAMPLE_TEXTURE2D(_SurfaceNoise, sampler_SurfaceNoise, noiseUV).r;
                float surfaceNoise = smoothstep(surfaceNoiseCutoff - 0.1, surfaceNoiseCutoff + 0.1, surfaceNoiseSample);
                float3 foamColor = surfaceNoise * _FoamColor.rgb;

                //Caustics
                float2 causticsDistortSample = distortNoise * _CausticsDistortionScale;
                float2 causticsUV = float2(i.uvCaustics.x + causticsDistortSample.x, i.uvCaustics.y + causticsDistortSample.y);
                float3 causticsColor = SAMPLE_TEXTURE2D(_CausticsTex, sampler_CausticsTex, causticsUV) * _CausticsScale * _CausticsColor;

                return half4(waterColor.rgb + foamColor + causticsColor, saturate(waterColor.a + depthTex));
            }
            ENDHLSL
        }
    }
}