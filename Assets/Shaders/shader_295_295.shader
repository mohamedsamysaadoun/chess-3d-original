//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "EivaaGames/Diffuse Cubemap with Mask" {
Properties {

_MainTex ("Texture", 2D) = "white" { }

_Cube ("Cubemap", Cube) = "" { }

_ReflColor ("Relection Color", Color) = (0.5,0.5,0.5,1)

_ReflMask ("Relection Mask", 2D) = "white" { }

}
SubShader {
 Tags { "RenderType" = "Opaque" }
 Pass {
 Name "FORWARD"
  Tags { "LIGHTMODE" = "FORWARDBASE" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
  GpuProgramID 8402
}
 Pass {
 Name "FORWARD"
  Tags { "LIGHTMODE" = "FORWARDADD" "RenderType" = "Opaque" }
 ZWrite Off
  GpuProgramID 94068
}
 Pass {
 Name "DEFERRED"
  Tags { "LIGHTMODE" = "DEFERRED" "RenderType" = "Opaque" }
  GpuProgramID 194632
}
}
Fall back "Diffuse"
}