#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_3
	#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinates : TEXCOORD0;
};

float pxRange;
float2 textureSize;
float4 fgColor;

float median(float r, float g, float b) {
	return max(min(r, g), min(max(r, g), b));
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 coord = input.TextureCoordinates;
    float3 msd = tex2D(SpriteTextureSampler, coord).rgb;
    float sd = median(msd.r, msd.g, msd.b);
    float2 unitRange = float2(pxRange, pxRange) / textureSize;
    float2 screenTexSize = float2(1.0, 1.0) / fwidth(coord);
    float screenPxDistance = max(0.5 * dot(unitRange, screenTexSize), 1.0) * (sd - 0.5);
    float opacity = clamp(screenPxDistance + 0.5, 0.0, 1.0);
    return lerp(float4(0, 0, 0, 0), fgColor, opacity);
}

technique SpriteDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};
