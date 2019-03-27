// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/PuzzleOutLine"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Outline ("Outline", Color) = (1,1,1,1)
		_OutlineWidth("Outline Width", Range(0, 15)) = 2
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFragO
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
			
			fixed4 _Outline;
			float4 _MainTex_TexelSize;
			float _OutlineWidth;
			
			fixed4 SpriteFragO(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				c.rgb *= c.a;
								
				half4 outlineC = _Outline;
				// outlineC.a *= ceil(c.a);
				outlineC.rgb *= c.a;
				outlineC.a *= c.a;
				
				fixed upAlpha = tex2D(_MainTex, IN.texcoord + fixed2(0, _MainTex_TexelSize.y * _OutlineWidth) ).a;
				fixed downAlpha = tex2D(_MainTex, IN.texcoord - fixed2(0, _MainTex_TexelSize.y * _OutlineWidth) ).a;
				fixed rightAlpha = tex2D(_MainTex, IN.texcoord + fixed2(_MainTex_TexelSize.x * _OutlineWidth, 0) ).a;
				fixed leftAlpha = tex2D(_MainTex, IN.texcoord - fixed2(_MainTex_TexelSize.x * _OutlineWidth, 0) ).a;
				
				return lerp(outlineC, c, ceil(upAlpha * downAlpha * rightAlpha * leftAlpha) );
			}
			
        ENDCG
        }
    }
}
