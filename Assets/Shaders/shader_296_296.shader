//////////////////////////////////////////
//
// NOTE: This is *not* a valid shader file
//
///////////////////////////////////////////
Shader "Hidden/FastBlur" {
Properties {

_MainTex ("Base (RGB)", 2D) = "white" { }

_Bloom ("Bloom (RGB)", 2D) = "black" { }

}
SubShader {
 Pass {
 ZTest Off
 ZWrite Off
 Cull Off
  GpuProgramID 27755
}
 Pass {
 ZTest Always
 ZWrite Off
 Cull Off
  GpuProgramID 95791
}
 Pass {
 ZTest Always
 ZWrite Off
 Cull Off
  GpuProgramID 195994
}
 Pass {
 ZTest Always
 ZWrite Off
 Cull Off
  GpuProgramID 198615
}
 Pass {
 ZTest Always
 ZWrite Off
 Cull Off
  GpuProgramID 276782
}
}
}