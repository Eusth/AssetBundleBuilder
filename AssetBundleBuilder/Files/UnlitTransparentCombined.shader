Shader "UnlitTransparentCombined" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _SubTex ("Base (RGB)", 2D) = "white" {}
    }
    Category {
       Lighting Off
       ZWrite Off
       Cull Back
       Blend SrcAlpha OneMinusSrcAlpha
       
       SubShader {
            Tags {"Queue"="Transparent+1000"  "IgnoreProjector"="True"}
            Pass {
               SetTexture [_MainTex] {
                      Combine Texture, Texture
                }
                SetTexture [_SubTex] { combine Texture lerp(Texture) Previous }
            }
        } 
    }
}