#pragma fragment frag

float4 _Color;
#ifdef _CUTOFF_COLOR
float _Cutoff;
#endif

float _VRChatCameraMode;
float _VRChatMirrorMode;
float CVRRenderingCam;

inline bool isMirror() {
    return _VRChatMirrorMode > 0 || CVRRenderingCam == 2;
}

inline bool isCamera() {
    return _VRChatCameraMode == 1 || _VRChatCameraMode == 2 || CVRRenderingCam == 1;
}

#pragma multi_compile _ _HIDE_IN_LOCAL
#ifdef _HIDE_IN_LOCAL
float _ShowInLocalHandCamera;
float _IsLocal;

inline void clipIfLocal() {
    clip(isMirror() + (_ShowInLocalHandCamera > 0 ? isCamera() : 0) - _IsLocal);
}
#endif

#ifdef _MIRROR_FLIP
inline float2 mirrorFlip(float2 uv) {
    return lerp(uv, float2(1 - uv.x, uv.y), isMirror());
}
#endif

#ifdef _HIDE_BY_DISTANCE
float _HideDistance;
#ifdef _HIDE_DISTANCE_FADE_AREA
float _HideDistanceFadeArea;
#endif
#endif

fixed4 frag (v2f i) : SV_Target
{
#ifdef _MIRROR_FLIP
    fixed4 col = tex2D(_MainTex, mirrorFlip(i.uv)) * _Color;
#else
    fixed4 col = tex2D(_MainTex, i.uv) * _Color;
#endif
#ifdef _HIDE_BY_DISTANCE
#ifdef _HIDE_DISTANCE_FADE_AREA
    float fade = saturate(1 - (_HideDistance - i.cameraDistance) / _HideDistanceFadeArea);
    col.a *= 1 - fade;
#else
    clip(_HideDistance - i.cameraDistance);
#endif
#endif

#ifdef _HIDE_IN_LOCAL
    clipIfLocal();
#endif
#ifdef _CUTOFF_COLOR
    clip(col.a - _Cutoff);
#endif
    UNITY_APPLY_FOG(i.fogCoord, col);
    return col;
}
