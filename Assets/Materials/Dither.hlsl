// sam hocevar's implementation
float3 rgb2hsv(float3 c)
{
    float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
    float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
    float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));

    float d = q.x - min(q.w, q.y);
    float e = 1.0e-10;
    return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

float3 hsv2rgb(float3 c)
{
    float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
    float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
    return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

void dither_float(float4 ColourIn, float2 UV, float3 Divisions, float Scale, out float4 ColourOut)
{
    float2 fuv = UV * _ScreenParams.xy;
    
    float2 dco = floor(fuv / Scale);
    //float2 uv = (dco * Scale) / _ScreenParams.xy;
    //float4 inc = tex2D(ColourIn, uv);
    
    float chk = 0.0;
    if (dco.x % 2.0 == dco.y % 2.0)
        chk = 1.0;
    
    float3 hsv = rgb2hsv(ColourIn.xyz) * Divisions;
    
    float3 flr = floor(hsv);
    float3 dif = hsv - flr;
    
    float3 col = flr;
    if (dif.x > 0.3 && dif.x < 0.6)
        col.x += chk;
    if (dif.y > 0.3 && dif.y < 0.6)
        col.y += chk;
    if (dif.z > 0.3 && dif.z < 0.6) 
        col.z += chk;

    if (dif.x > 0.6)
        col.x += 1.0;
    if (dif.y > 0.6)
        col.y += 1.0;
    if (dif.z > 0.6)
        col.z += 1.0;
    
    col /= Divisions;
    
    ColourOut = float4(hsv2rgb(col), 1.0);
}