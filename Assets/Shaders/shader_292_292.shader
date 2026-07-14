//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "EivaaGames/Diffuse Cubemap" {
Properties {

_MainTex ("Texture", 2D) = "white" { }

_Cube ("Cubemap", Cube) = "" { }

_ReflColor ("Relection Color", Color) = (0.5,0.5,0.5,1)

}
SubShader {
 Tags { "RenderType" = "Opaque" }
 Pass {
 Name "FORWARD"
  Tags { "LIGHTMODE" = "FORWARDBASE" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
  GpuProgramID 45153
}
 Pass {
 Name "FORWARD"
  Tags { "LIGHTMODE" = "FORWARDADD" "RenderType" = "Opaque" }
 ZWrite Off
  GpuProgramID 130402
}
 Pass {
 Name "DEFERRED"
  Tags { "LIGHTMODE" = "DEFERRED" "RenderType" = "Opaque" }
  GpuProgramID 184276
}
}
Fall back "Diffuse"
}