Shader "UnlitTransparent" {
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    Category {
       Lighting Off
       ZWrite Off
       Cull Back
       Blend SrcAlpha OneMinusSrcAlpha
       
       SubShader {
            Tags {"Queue"="Transparent+1000"}
            Pass {
               SetTexture [_MainTex] {
                      Combine Texture, Texture
                }
            }
        } 
    }
}