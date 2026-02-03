Shader "Custom/OutlineFill" {
    Properties {
        [HideInInspector] _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _OutlineWidth("Outline Width", Range(0, 20)) = 2
    }
    SubShader {
        Tags { "RenderType" = "Transparent" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent+110" }
        Pass {
            Name "Fill"
            // This Tag is the secret sauce for per - object settings
            Tags { "LightMode" = "OutlineFillPass" }

            Cull Off
            ZTest Always
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            Stencil {
                Ref 1
                Comp NotEqual
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes { float4 positionOS : POSITION; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; float4 color : COLOR; };

            // These must match the names in the C# script
            CBUFFER_START(UnityPerMaterial)
            float4 _OutlineColor;
            float _OutlineWidth;
            CBUFFER_END

            Varyings vert(Attributes input) {
                Varyings output;
                float3 viewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS.xyz));
                float3 viewNormal = TransformWorldToViewDir(TransformObjectToWorldNormal(input.normalOS));

                viewPos += viewNormal * - viewPos.z * (_OutlineWidth / 1000.0);

                output.positionCS = TransformWViewToHClip(viewPos);
                output.color = _OutlineColor;
                return output;
            }

            half4 frag(Varyings input) : SV_Target { return input.color; }
            ENDHLSL
        }
    }
}