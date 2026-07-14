//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "EivaaGames/Diffuse Cubemap with Rimlight" {
Properties {

_MainTex ("Texture", 2D) = "white" { }

_Cube ("Cubemap", Cube) = "" { }

_ReflColor ("Relection Color", Color) = (0.5,0.5,0.5,1)

_RimColor ("Rim Color", Color) = (1,1,1,1)

_RimPower ("Rim Power", Range(0.5, 8)) = 3.0

_RimDirection ("Rim Direction", Vector) = (0,1,0,1)

}
SubShader {
 Tags { "RenderType" = "Opaque" }
 Pass {
 Name "FORWARD"
  Tags { "LIGHTMODE" = "FORWARDBASE" "RenderType" = "Opaque" "SHADOWSUPPORT" = "true" }
  GpuProgramID 47412
}
 Pass {
 Name "FORWARD"
  Tags { "LIGHTMODE" = "FORWARDADD" "RenderType" = "Opaque" }
 ZWrite Off
  GpuProgramID 110104
}
 Pass {
 Name "DEFERRED"
  Tags { "LIGHTMODE" = "DEFERRED" "RenderType" = "Opaque" }
  GpuProgramID 141029
}
}
Fall back "Diffuse"
}