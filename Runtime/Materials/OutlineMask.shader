Shader "Custom/OutlineMask" {
    SubShader {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "Queue" = "Transparent+100" }
        Pass {
            Name "Mask"
            ZTest Always // Makes it work through walls
            ZWrite Off
            ColorMask 0 // Don't draw any color
            Stencil {
                Ref 1
                Pass Replace // Put a '1' in the stencil buffer
            }
        }
    }
}