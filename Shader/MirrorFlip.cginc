float _VRChatMirrorMode;

float2 mirrorFlip(float2 uv) {
    return lerp(uv, float2(1 - uv.x, uv.y), _VRChatMirrorMode > 0);
}
