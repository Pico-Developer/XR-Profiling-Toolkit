// Made with Amplify Shader Editor v1.9.1
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TBY_AlphaAll"
{
	Properties
	{
		[Enum(UnityEngine.Rendering.BlendMode)]_Src("Src", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)]_Dst("Dst", Float) = 10
		[Enum(Off,0,Defeat,1)]_ZWriteMode("ZWriteMode", Float) = 0
		[Enum(UnityEngine.Rendering.CompareFunction)]_ZTestMode("ZTestMode", Float) = 4
		[Enum(UnityEngine.Rendering.CullMode)]_CullMode("CullMode", Float) = 0
		[Toggle]_Rotate90("Rotate90", Float) = 0
		_MainTex("MainTex", 2D) = "white" {}
		[HDR]_MainColor("MainColor", Color) = (1,1,1,1)
		_U_Speed("U_Speed", Float) = 0
		_V_Speed("V_Speed", Float) = 0
		_Mask("Mask", 2D) = "white" {}
		_MaskU_Speed("MaskU_Speed", Float) = 0
		_MaskV_Speed("MaskV_Speed", Float) = 0
		_Dissovle("Dissovle", 2D) = "white" {}
		[Toggle(_DISSCUSTOM_ON)] _DissCustom("DissCustom", Float) = 0
		[Toggle(_SOFTDISSOLVE_ON)] _SoftDissolve("SoftDissolve", Float) = 0
		_SoftDissPower("SoftDissPower", Range( 0 , 1)) = 0.6
		_DissolveRange("DissolveRange", Range( 0 , 1)) = 1
		_DissU_Speed("DissU_Speed", Float) = 0
		_DissV_Speed("DissV_Speed", Float) = 0
		_Niuqu("Niuqu", 2D) = "white" {}
		_NiuquPower("NiuquPower", Float) = 0
		_NiuquU_Speed("NiuquU_Speed", Float) = 0
		_NiuquV_Speed("NiuquV_Speed", Float) = 0
		_Bump("Bump", 2D) = "white" {}
		[Toggle(_BUMPSWITCH_ON)] _BumpSwitch("BumpSwitch", Float) = 0
		[Toggle(_BUMPCUSTOM_ON)] _BumpCustom("BumpCustom", Float) = 0
		[Toggle(_VERTEXNORMAL_ON)] _VertexNormal("VertexNormal", Float) = 0
		_BumpPower("BumpPower", Range( 0 , 1)) = 0
		_BumpVector("BumpVector ", Vector) = (0,0,0,0)
		_BumpV_speed("BumpV_speed", Float) = 0
		_BumpU_speed("BumpU_speed", Float) = 0

	}
	
	SubShader
	{
		
		
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
	LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend [_Src] [_Dst]
		AlphaToMask Off
		Cull [_CullMode]
		ColorMask RGBA
		ZWrite [_ZWriteMode]
		ZTest [_ZTestMode]
		Offset 0 , 0
		
		
		
		Pass
		{
			Name "Unlit"

			CGPROGRAM

			

			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			//only defining to not throw compilation error over Unity 5.5
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_COLOR
			#pragma shader_feature_local _BUMPSWITCH_ON
			#pragma shader_feature_local _BUMPCUSTOM_ON
			#pragma shader_feature_local _VERTEXNORMAL_ON
			#pragma shader_feature_local _SOFTDISSOLVE_ON
			#pragma shader_feature_local _DISSCUSTOM_ON


			struct appdata
			{
				float4 vertex : POSITION;
				float4 color : COLOR;
				float4 ase_texcoord : TEXCOORD0;
				float4 ase_texcoord2 : TEXCOORD2;
				float3 ase_normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 worldPos : TEXCOORD0;
				#endif
				float4 ase_color : COLOR;
				float4 ase_texcoord1 : TEXCOORD1;
				float4 ase_texcoord2 : TEXCOORD2;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};

			uniform float _ZWriteMode;
			uniform float _ZTestMode;
			uniform float _CullMode;
			uniform float _Src;
			uniform float _Dst;
			uniform sampler2D _Bump;
			uniform float _BumpU_speed;
			uniform float _BumpV_speed;
			uniform float4 _Bump_ST;
			uniform float _BumpPower;
			uniform float3 _BumpVector;
			uniform float4 _MainColor;
			uniform sampler2D _MainTex;
			uniform float _Rotate90;
			uniform float _U_Speed;
			uniform float _V_Speed;
			uniform float4 _MainTex_ST;
			uniform sampler2D _Niuqu;
			uniform float _NiuquU_Speed;
			uniform float _NiuquV_Speed;
			uniform float4 _Niuqu_ST;
			uniform float _NiuquPower;
			uniform sampler2D _Mask;
			uniform float _MaskU_Speed;
			uniform float _MaskV_Speed;
			uniform float4 _Mask_ST;
			uniform float _DissolveRange;
			uniform sampler2D _Dissovle;
			uniform float _DissU_Speed;
			uniform float _DissV_Speed;
			uniform float4 _Dissovle_ST;
			uniform float _SoftDissPower;

			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				float2 appendResult100 = (float2(_BumpU_speed , _BumpV_speed));
				float2 uv_Bump = v.ase_texcoord.xy * _Bump_ST.xy + _Bump_ST.zw;
				float2 panner98 = ( 1.0 * _Time.y * appendResult100 + uv_Bump);
				float4 tex2DNode96 = tex2Dlod( _Bump, float4( panner98, 0, 0.0) );
				float4 texCoord161 = v.ase_texcoord2;
				texCoord161.xy = v.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _BUMPCUSTOM_ON
				float staticSwitch162 = texCoord161.y;
				#else
				float staticSwitch162 = _BumpPower;
				#endif
				#ifdef _VERTEXNORMAL_ON
				float3 staticSwitch160 = v.ase_normal;
				#else
				float3 staticSwitch160 = float3( 0,0,0 );
				#endif
				#ifdef _BUMPSWITCH_ON
				float4 staticSwitch157 = ( ( tex2DNode96 * tex2DNode96.a ) * staticSwitch162 * float4( ( _BumpVector + staticSwitch160 ) , 0.0 ) );
				#else
				float4 staticSwitch157 = float4( 0,0,0,0 );
				#endif
				
				o.ase_color = v.color;
				o.ase_texcoord1.xy = v.ase_texcoord.xy;
				o.ase_texcoord2 = v.ase_texcoord2;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord1.zw = 0;
				float3 vertexValue = float3(0, 0, 0);
				#if ASE_ABSOLUTE_VERTEX_POS
				vertexValue = v.vertex.xyz;
				#endif
				vertexValue = staticSwitch157.rgb;
				#if ASE_ABSOLUTE_VERTEX_POS
				v.vertex.xyz = vertexValue;
				#else
				v.vertex.xyz += vertexValue;
				#endif
				o.vertex = UnityObjectToClipPos(v.vertex);

				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				fixed4 finalColor;
				#ifdef ASE_NEEDS_FRAG_WORLD_POSITION
				float3 WorldPosition = i.worldPos;
				#endif
				float2 appendResult48 = (float2(_U_Speed , _V_Speed));
				float2 uv_MainTex = i.ase_texcoord1.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 texCoord64 = i.ase_texcoord2;
				texCoord64.xy = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult65 = (float2(texCoord64.z , texCoord64.w));
				float2 panner5 = ( 1.0 * _Time.y * appendResult48 + ( uv_MainTex + appendResult65 ));
				float2 appendResult44 = (float2(_NiuquU_Speed , _NiuquV_Speed));
				float2 uv_Niuqu = i.ase_texcoord1.xy * _Niuqu_ST.xy + _Niuqu_ST.zw;
				float2 panner43 = ( 1.0 * _Time.y * appendResult44 + uv_Niuqu);
				float2 temp_cast_0 = (tex2D( _Niuqu, panner43 ).r).xx;
				float2 lerpResult39 = lerp( panner5 , temp_cast_0 , _NiuquPower);
				float2 appendResult166 = (float2((lerpResult39).y , (lerpResult39).x));
				float2 appendResult53 = (float2(_MaskU_Speed , _MaskV_Speed));
				float2 uv_Mask = i.ase_texcoord1.xy * _Mask_ST.xy + _Mask_ST.zw;
				float2 panner6 = ( 1.0 * _Time.y * appendResult53 + uv_Mask);
				float4 tex2DNode2 = tex2D( _Mask, panner6 );
				float4 texCoord79 = i.ase_texcoord2;
				texCoord79.xy = i.ase_texcoord2.xy * float2( 1,1 ) + float2( 0,0 );
				#ifdef _DISSCUSTOM_ON
				float staticSwitch77 = texCoord79.x;
				#else
				float staticSwitch77 = _DissolveRange;
				#endif
				float temp_output_178_0 = ( 1.0 - staticSwitch77 );
				float2 appendResult47 = (float2(_DissU_Speed , _DissV_Speed));
				float2 uv_Dissovle = i.ase_texcoord1.xy * _Dissovle_ST.xy + _Dissovle_ST.zw;
				float2 panner27 = ( 1.0 * _Time.y * appendResult47 + uv_Dissovle);
				float4 tex2DNode20 = tex2D( _Dissovle, panner27 );
				float smoothstepResult176 = smoothstep( 0.0 , (0.5 + (_SoftDissPower - 0.0) * (1.0 - 0.5) / (1.0 - 0.0)) , ( tex2DNode20.r - (-1.0 + (temp_output_178_0 - 0.0) * (1.0 - -1.0) / (1.0 - 0.0)) ));
				#ifdef _SOFTDISSOLVE_ON
				float staticSwitch197 = saturate( smoothstepResult176 );
				#else
				float staticSwitch197 = step( temp_output_178_0 , tex2DNode20.r );
				#endif
				float4 temp_output_15_0 = ( i.ase_color * _MainColor * tex2D( _MainTex, (( _Rotate90 )?( lerpResult39 ):( appendResult166 )) ) * tex2DNode2.r * i.ase_color.a * _MainColor.a * staticSwitch197 );
				float4 appendResult18 = (float4((temp_output_15_0).rgb , ( (temp_output_15_0).a * staticSwitch197 * tex2DNode2.r )));
				
				
				finalColor = appendResult18;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19100
Node;AmplifyShaderEditor.SamplerNode;40;-2424.507,-411.8559;Inherit;True;Property;_Niuqu;Niuqu;20;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;49;-2849.168,-505.2435;Inherit;False;Property;_U_Speed;U_Speed;8;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;50;-2845.094,-418.3433;Inherit;False;Property;_V_Speed;V_Speed;9;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;18;496.7244,-91.92675;Inherit;True;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;48;-2653.642,-502.5281;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;65;-3044.104,-566.5496;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;-2746.954,-646.4382;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1150.146,-85.33932;Float;False;True;-1;2;ASEMaterialInspector;100;5;TBY_AlphaAll;0770190933193b94aaa3065e307002fa;True;Unlit;0;0;Unlit;2;True;True;2;5;True;_Src;10;True;_Dst;0;1;False;;0;False;;True;0;False;;0;False;;False;False;False;False;False;False;False;False;False;True;0;False;;True;True;2;True;_CullMode;False;True;True;True;True;True;0;False;;False;False;False;False;False;False;False;True;False;0;False;;255;False;;255;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;0;False;;True;True;2;True;_ZWriteMode;True;3;True;_ZTestMode;True;True;0;False;;0;False;;True;2;RenderType=Transparent=RenderType;Queue=Transparent=Queue=0;True;2;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;1;Vertex Position,InvertActionOnDeselection;1;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2307.863,-173.7207;Inherit;False;Property;_NiuquPower;NiuquPower;21;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;44;-2892.802,-59.59166;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;43;-2714.623,-152.7834;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;5;-2409.271,-589.4454;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;3;-3070.897,-752.8933;Inherit;False;0;1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;-3104.329,-309.6811;Inherit;False;0;40;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;100;-662.3143,1188.589;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-875.3143,1274.589;Inherit;False;Property;_BumpV_speed;BumpV_speed;30;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;101;-869.3143,1170.589;Inherit;False;Property;_BumpU_speed;BumpU_speed;31;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;98;-514.2806,1050.417;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;96;-272.0986,1020.875;Inherit;True;Property;_Bump;Bump;24;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;97;-914.2338,942.5258;Inherit;False;0;96;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;64;-3367.807,-635.0272;Inherit;False;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;27;-2446.693,223.6398;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;47;-2639.934,307.2;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;-2771.458,124.794;Inherit;False;0;20;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;-2824.597,303.1275;Inherit;False;Property;_DissU_Speed;DissU_Speed;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2825.956,413.1109;Inherit;False;Property;_DissV_Speed;DissV_Speed;19;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;45;-3146.384,-128.7072;Inherit;False;Property;_NiuquU_Speed;NiuquU_Speed;22;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-3137.98,-4.042786;Inherit;False;Property;_NiuquV_Speed;NiuquV_Speed;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;32.12724,1034.346;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;109;359.8607,1037.075;Inherit;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT3;0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector3Node;152;-11.28645,1411.63;Inherit;False;Property;_BumpVector;BumpVector ;29;0;Create;True;0;0;0;False;0;False;0,0,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleAddOpNode;159;218.5593,1413.044;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StaticSwitch;160;-13.35491,1602.12;Inherit;False;Property;_VertexNormal;VertexNormal;27;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT3;0,0,0;False;0;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT3;0,0,0;False;5;FLOAT3;0,0,0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;161;-625.5787,1455.621;Inherit;False;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;106;-564.5065,1312.191;Inherit;False;Property;_BumpPower;BumpPower;28;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;162;-205.7621,1310.326;Inherit;False;Property;_BumpCustom;BumpCustom;26;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalVertexDataNode;112;-230.2386,1630.892;Inherit;False;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;77;-2088.821,384.9002;Inherit;False;Property;_DissCustom;DissCustom;14;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;23;-2415.435,387.8361;Inherit;False;Property;_DissolveRange;DissolveRange;17;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;79;-2402.806,522.6047;Inherit;False;2;-1;4;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;10;-1167.654,-646.0317;Inherit;False;Property;_MainColor;MainColor;7;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;39;-2088.451,-462.0266;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SwizzleNode;165;-1923.75,-521.2139;Inherit;False;FLOAT;1;1;2;3;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SwizzleNode;164;-1930.75,-616.2139;Inherit;False;FLOAT;0;1;2;3;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;166;-1771.75,-607.2139;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;168;-1603.541,-425.4917;Inherit;False;Property;_Rotate90;Rotate90;5;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;170;1299.691,-538.8696;Inherit;False;Property;_ZWriteMode;ZWriteMode;2;1;[Enum];Create;True;0;2;Off;0;Defeat;1;0;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;171;1302.928,-466.8861;Inherit;False;Property;_ZTestMode;ZTestMode;3;1;[Enum];Create;True;0;1;Option1;0;1;UnityEngine.Rendering.CompareFunction;True;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;169;1316.691,-397.8696;Inherit;False;Property;_CullMode;CullMode;4;1;[Enum];Create;True;0;1;Option1;0;1;UnityEngine.Rendering.CullMode;True;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;172;1323.828,-323.2815;Inherit;False;Property;_Src;Src;0;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;5;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;173;1320.828,-244.2815;Inherit;False;Property;_Dst;Dst;1;1;[Enum];Create;True;0;0;1;UnityEngine.Rendering.BlendMode;True;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-340.2345,-641.4277;Inherit;False;7;7;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ComponentMaskNode;16;178.8895,-422.3644;Inherit;True;True;True;True;False;1;0;COLOR;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-34.27578,-88.07224;Inherit;True;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;63;-194.8464,-317.3053;Inherit;False;False;False;False;True;1;0;COLOR;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;6;-930.024,553.9461;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;53;-1182.821,706.8458;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;54;-1401.11,681.7462;Inherit;False;Property;_MaskU_Speed;MaskU_Speed;11;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-1403.11,814.7462;Inherit;False;Property;_MaskV_Speed;MaskV_Speed;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1236.315,522.7373;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;1;-1360.63,-451.3147;Inherit;True;Property;_MainTex;MainTex;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;60;-791.9366,-785.9564;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;20;-2043.596,51.41193;Inherit;True;Property;_Dissovle;Dissovle;13;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;157;784.0085,891.0295;Inherit;False;Property;_BumpSwitch;BumpSwitch;25;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;181;-1370.915,-216.4073;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;192;-1358.609,58.81262;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;178;-1824.138,251.9643;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;176;-1035.59,60.1936;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-1580.431,455.0666;Inherit;False;Property;_SoftDissPower;SoftDissPower;16;0;Create;True;0;0;0;False;0;False;0.6;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;197;-633.7886,-90.34454;Inherit;False;Property;_SoftDissolve;SoftDissolve;15;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;200;-798.6586,-1.012887;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;195;-1601.175,235.5008;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;-1;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-682.3939,526.5471;Inherit;True;Property;_Mask;Mask;10;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;198;-1261.507,289.0196;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;1;False;1;FLOAT;0
WireConnection;40;1;43;0
WireConnection;18;0;16;0
WireConnection;18;3;37;0
WireConnection;48;0;49;0
WireConnection;48;1;50;0
WireConnection;65;0;64;3
WireConnection;65;1;64;4
WireConnection;66;0;3;0
WireConnection;66;1;65;0
WireConnection;0;0;18;0
WireConnection;0;1;157;0
WireConnection;44;0;45;0
WireConnection;44;1;46;0
WireConnection;43;0;42;0
WireConnection;43;2;44;0
WireConnection;5;0;66;0
WireConnection;5;2;48;0
WireConnection;100;0;101;0
WireConnection;100;1;102;0
WireConnection;98;0;97;0
WireConnection;98;2;100;0
WireConnection;96;1;98;0
WireConnection;27;0;21;0
WireConnection;27;2;47;0
WireConnection;47;0;51;0
WireConnection;47;1;52;0
WireConnection;105;0;96;0
WireConnection;105;1;96;4
WireConnection;109;0;105;0
WireConnection;109;1;162;0
WireConnection;109;2;159;0
WireConnection;159;0;152;0
WireConnection;159;1;160;0
WireConnection;160;0;112;0
WireConnection;162;1;106;0
WireConnection;162;0;161;2
WireConnection;77;1;23;0
WireConnection;77;0;79;1
WireConnection;39;0;5;0
WireConnection;39;1;40;1
WireConnection;39;2;41;0
WireConnection;165;0;39;0
WireConnection;164;0;39;0
WireConnection;166;0;165;0
WireConnection;166;1;164;0
WireConnection;168;0;166;0
WireConnection;168;1;39;0
WireConnection;15;0;60;0
WireConnection;15;1;10;0
WireConnection;15;2;1;0
WireConnection;15;3;2;1
WireConnection;15;4;60;4
WireConnection;15;5;10;4
WireConnection;15;6;197;0
WireConnection;16;0;15;0
WireConnection;37;0;63;0
WireConnection;37;1;197;0
WireConnection;37;2;2;1
WireConnection;63;0;15;0
WireConnection;6;0;4;0
WireConnection;6;2;53;0
WireConnection;53;0;54;0
WireConnection;53;1;55;0
WireConnection;1;1;168;0
WireConnection;20;1;27;0
WireConnection;157;0;109;0
WireConnection;181;0;178;0
WireConnection;181;1;20;1
WireConnection;192;0;20;1
WireConnection;192;1;195;0
WireConnection;178;0;77;0
WireConnection;176;0;192;0
WireConnection;176;2;198;0
WireConnection;197;1;181;0
WireConnection;197;0;200;0
WireConnection;200;0;176;0
WireConnection;195;0;178;0
WireConnection;2;1;6;0
WireConnection;198;0;177;0
ASEEND*/
//CHKSM=72EED216E4AC4BC559E6D0673ACD4630AB54BC08