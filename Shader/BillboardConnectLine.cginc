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
float _LineWidth;

v2f vert (appdata v)
{
    float3 scale = float3(
        length(unity_ObjectToWorld._m00_m10_m20),
        length(unity_ObjectToWorld._m01_m11_m21),
        length(unity_ObjectToWorld._m02_m12_m22)
    );
    float3 rootPos = unity_ObjectToWorld._m03_m13_m23;
    float3 rootToCamera = _WorldSpaceCameraPos - rootPos;
    float3 up = float3(0, 1, 0);
    // right: normal to rootToCamera and up
    float3 right = normalize(cross(rootToCamera, up));

    float3 offsetPos = right * _OffsetX + up * _OffsetY;

    float3 rotRight = normalize(offsetPos);
    float3 rotUp = normalize(cross(rotRight, rootToCamera));
    float3 rotForward = -normalize(rootToCamera);
    float4x4 rotation = {
        1, 0, 0, 0,
        0, 1, 0, 0,
        0, 0, 1, 0,
        0, 0, 0, 1,
    };
    rotation._m00_m10_m20 = rotRight;
    rotation._m01_m11_m21 = rotUp;
    rotation._m02_m12_m22 = rotForward;

    float3 vertexPos = float3(v.vertex.x * length(offsetPos), v.vertex.y * _LineWidth, 0);
    vertexPos = mul(rotation, vertexPos);
    vertexPos += offsetPos * 0.5;
    float4 worldPos = float4(vertexPos + rootPos, 1);

    v2f o;
    o.vertex = mul(UNITY_MATRIX_VP, worldPos);
    UNITY_TRANSFER_FOG(o,o.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
    return o;
}
