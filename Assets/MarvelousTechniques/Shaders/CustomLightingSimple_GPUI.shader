//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2015 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "GPUInstancer/Kirnu/Marvelous/CustomLightingSimple" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_LayoutTexture ("Layout Texture", 2D) = "white" {}
		_LayoutTexturePower ("Layout Texture Power", Float) = 1
 		_MainColor ("Main Color", Color) = (1,0.73,0.117,0)
 		_TopLight ("Top Light", Float) = 1
 		_RightLight ("Right Light", Float) = 1
 		_FrontLight ("Front Light", Float) = 1
		_RimColor ("Rim Color", Color) = (0,0,0,0)
		_RimPower ("Rim Power", Float) = 0.0
		
 		_LightTint ("Light Tint", Color) = (1,1,1,0)
 		_AmbientColor ("Ambient Color", Color) = (0.5,0.1,0.2,0.0)
 		_AmbientPower ("Ambient Power", Float) = 0.0
		_LightmapColor ("Lightmap Tint", Color) = (0,0,0,0)
		_LightmapPower ("Lightmap Power", Float) = 1
		_ShadowPower ("Shadow Light", Float) = 0
		[Toggle(USE_LIGHTMAP)]_UseLightMap ("Lightmap Enabled", Float) = 0

		[HideInInspector]_LightDirF ("Front Light Direction", Vector) = (0,0,-1)
		[HideInInspector]_LightDirT ("Top Light Direction", Vector) = (0,1,0)
		[HideInInspector]_LightDirR ("Right Light Direction", Vector) = (1,0,0)
		[Toggle(USE_DIR_LIGHT)] _UseDirLight ("Directional Light", Float) = 0
	}
	SubShader {
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
		LOD 200
		
		Pass {

		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "RenderType"="Opaque" }
			CGPROGRAM
#include "UnityCG.cginc"
#include "./../../GPUInstancer/Shaders/Include/GPUInstancerInclude.cginc"
#pragma instancing_options procedural:setupGPUI
#pragma multi_compile_instancing
				#pragma shader_feature USE_LIGHTMAP
				#pragma shader_feature USE_DIR_LIGHT
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#define USE_LAYOUT_TEXTURE;
				#define USE_MAIN_TEX;
				
				#pragma vertex vert
				#pragma fragment frag

				uniform half3 _MainColor;
				uniform half _TopLight;
				uniform half _RightLight;
				uniform half _FrontLight;
				uniform half3 _RimColor;
				uniform half _RimPower;
				
				uniform half3 _AmbientColor;
				uniform half _AmbientPower;
				uniform half _UseLightMap;
				
				uniform half _LightmapPower;
				uniform half3 _LightTint;
				uniform half3 _LightmapColor;
				uniform half _ShadowPower;
				
				uniform float _LayoutTexturePower;
				
				#include "Marvelous.cginc"
				
				CL_OUT_WPOS vert(CL_IN v) {
					return customLightingWPosVertSimple(v, _MainColor, _RimColor, _RimPower, _RightLight, _FrontLight, _TopLight, _AmbientColor, _AmbientPower);
				}
				
				fixed4 frag(CL_OUT_WPOS v) : COLOR {
					return customLightingFrag(v, _LightTint, _UseLightMap, _LightmapPower, _LightmapColor, _ShadowPower);
				}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
