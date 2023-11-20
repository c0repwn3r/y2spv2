Shader "Depth Mask" 
{
	Properties 
	{
	}
	SubShader 
	{
		Tags { "Queue" = "Geometry-40" "RenderType"="Opaque" }
		LOD 200

		Lighting Off
        ZTest LEqual
        ZWrite On
        ColorMask 0
        Pass {}
	}
}
