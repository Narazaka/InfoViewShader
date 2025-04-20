// make fog work
#pragma multi_compile_fog
            
#pragma vertex vert
#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    UNITY_FOG_COORDS(1)
    float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
float4 _MainTex_ST;
float _OffsetX;
float _OffsetY;
float _ScaleX;
float _ScaleY;

v2f vert (appdata v)
{
    float3 scale = float3(
        length(unity_ObjectToWorld._m00_m10_m20),
        length(unity_ObjectToWorld._m01_m11_m21),
        length(unity_ObjectToWorld._m02_m12_m22)
    );
    float3 rootPos = unity_ObjectToWorld._m03_m13_m23;
    // float2 screenCenter = _Center.xy;
    float3 rootToCamera = _WorldSpaceCameraPos - rootPos;
    float3 up = float3(0, 1, 0);
    // right: normal to rootToCamera and up
    float3 right = normalize(cross(rootToCamera, up));
                
    float3 vertexPos = - right * v.vertex.x + up * v.vertex.y;
    vertexPos = float3(vertexPos.x / scale.x * _ScaleX, vertexPos.y / scale.y * _ScaleY, vertexPos.z / scale.z);
    float3 offsetPos = right * _OffsetX + up * _OffsetY;
    offsetPos = float3(offsetPos.x / scale.x, offsetPos.y / scale.y, offsetPos.z / scale.z);

    v2f o;
    o.vertex = UnityObjectToClipPos(vertexPos + offsetPos);
    UNITY_TRANSFER_FOG(o,o.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
}
