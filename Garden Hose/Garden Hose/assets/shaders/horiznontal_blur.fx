#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
float2 SingleTexelSize;
float Size;


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

float4 Round(VertexShaderOutput input) : COLOR
{
    float TwoPi = 6.28318530718;
	
	//  Properties.
    float Size = 15;
    float Directions = 16;
    float LayerCount = 16;
    float Radius = Size / LayerCount;
	
	// Setup
    float2 TexelCoordinate = input.TextureCoordinates - SingleTexelSize;
    float4 Color = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
	
	
	// Sample surrounding pixels.
    for (float Period = 0; Period <= TwoPi; Period += (TwoPi / Directions))
    {
        for (float CurRadius = 1; CurRadius <= (Radius * Size); CurRadius += Radius)
        {
            Color += tex2D(SpriteTextureSampler, input.TextureCoordinates 
			+ float2(CurRadius * SingleTexelSize.x * cos(Period), CurRadius * SingleTexelSize.y * sin(Period)))
			* input.Color;
        }

    }
	
    return Color / ((LayerCount * Directions) + 1);
}

float4 Rect(VertexShaderOutput input) : COLOR
{
	// Properties.
    float HalfSize = Size / 2;
    float2 Step = 1 / Size;
	
	// Setup.
    float4 Color = float4(0, 0, 0, 0);
	
	// Loop.
    for (float X = -HalfSize; X < HalfSize; X += 1)
    {
        for (float Y = -HalfSize; Y < HalfSize; Y += 1)
        {
            Color += tex2D(SpriteTextureSampler, input.TextureCoordinates +
			float2(X * SingleTexelSize.x, Y * SingleTexelSize.y)) * input.Color;
        }
    }
	
	// Return.
    return Color / (Size * Size);
	
}

technique SpriteDrawing
{
	pass P0
	{
        PixelShader = compile PS_SHADERMODEL Rect();
    }
};