#pragma multi_compile _ _HIDE_BY_DISTANCE
#pragma multi_compile_fog
#pragma multi_compile_instancing

#pragma vertex vert
#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f
{
    float2 uv : TEXCOORD0;
    UNITY_FOG_COORDS(1)
    float4 vertex : SV_POSITION;
#ifdef _HIDE_BY_DISTANCE
    float cameraDistance : COLOR1;
#endif
    UNITY_VERTEX_OUTPUT_STEREO
};

sampler2D _MainTex;
float4 _MainTex_ST;
float _OffsetX;
float _OffsetY;
float _LineWidth;

v2f vert (appdata v)
{
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_OUTPUT(v2f, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    float3 scale = float3(
        length(unity_ObjectToWorld._m00_m10_m20),
        length(unity_ObjectToWorld._m01_m11_m21),
        length(unity_ObjectToWorld._m02_m12_m22)
    );
    float3 rootPos = unity_ObjectToWorld._m03_m13_m23;
#if defined(USING_STEREO_MATRICES)
    float3 cameraPos = (unity_StereoWorldSpaceCameraPos[0] + unity_StereoWorldSpaceCameraPos[1]) * 0.5;
#else
    float3 cameraPos = _WorldSpaceCameraPos;
#endif
    float3 rootToCamera = cameraPos - rootPos;
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

    o.vertex = mul(UNITY_MATRIX_VP, worldPos);
    UNITY_TRANSFER_FOG(o,o.vertex);
    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
#ifdef _HIDE_BY_DISTANCE
    o.cameraDistance = length(cameraPos - (rootPos + offsetPos));
#endif
    return o;
}
