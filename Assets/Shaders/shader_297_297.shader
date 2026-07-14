//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Mobile/Transparent/Vertex Color" {
Properties {

_Color ("Main Color", Color) = (1,1,1,1)

_SpecColor ("Spec Color", Color) = (1,1,1,0)

_Emission ("Emmisive Color", Color) = (0,0,0,0)

_Shininess ("Shininess", Range(0.1, 1)) = 0.699999988079071

_MainTex ("Base (RGB) Trans (A)", 2D) = "white" { }

}
SubShader {
 Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
 Pass {
  Tags { "IGNOREPROJECTOR" = "true" "QUEUE" = "Transparent" "RenderType" = "Transparent" }
 ZWrite Off
  GpuProgramID 23406
}
}
}