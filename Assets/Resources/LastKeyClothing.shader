Shader "LAST KEY/Clothing Vertex Regions"
{
    Properties { _BaseMap("Skin texture",2D)="white"{} _BaseColor("Skin tint",Color)=(1,1,1,1) }
    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseMap_ST;half4 _BaseColor;
            CBUFFER_END
            struct Attributes { float4 positionOS:POSITION;float3 normalOS:NORMAL;float2 uv:TEXCOORD0;half4 color:COLOR; };
            struct Varyings {float4 positionCS:SV_POSITION;float3 positionWS:TEXCOORD0;float3 normalWS:TEXCOORD1;float2 uv:TEXCOORD2;half4 color:COLOR;};
            Varyings vert(Attributes i){Varyings o;VertexPositionInputs p=GetVertexPositionInputs(i.positionOS.xyz);o.positionCS=p.positionCS;o.positionWS=p.positionWS;o.normalWS=TransformObjectToWorldNormal(i.normalOS);o.uv=TRANSFORM_TEX(i.uv,_BaseMap);o.color=i.color;return o;}
            half4 frag(Varyings i):SV_Target
            {
                half3 skin=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,i.uv).rgb*_BaseColor.rgb;
                half3 albedo=lerp(i.color.rgb,skin,i.color.a);half3 normal=normalize(i.normalWS);
                Light light=GetMainLight(TransformWorldToShadowCoord(i.positionWS));half3 illumination=SampleSH(normal)+light.color*saturate(dot(normal,light.direction))*light.shadowAttenuation;
                #ifdef _ADDITIONAL_LIGHTS
                for(uint n=0;n<GetAdditionalLightsCount();n++){Light l=GetAdditionalLight(n,i.positionWS);illumination+=l.color*saturate(dot(normal,l.direction))*l.distanceAttenuation*l.shadowAttenuation;}
                #endif
                return half4(albedo*max(illumination,half3(.16,.18,.20)),1);
            }
            ENDHLSL
        }
        UsePass "Universal Render Pipeline/Lit/ShadowCaster"
        UsePass "Universal Render Pipeline/Lit/DepthOnly"
    }
}
