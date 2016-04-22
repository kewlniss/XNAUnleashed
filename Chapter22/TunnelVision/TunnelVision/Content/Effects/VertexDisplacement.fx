float4x4 World	: WORLD;
float4x4 View;
float4x4 Projection;

float4 AmbientColor : COLOR0;
float Timer : TIME;
float Offset = 1.0f;

texture ColorMap;
sampler ColorMapSampler = sampler_state
{
    texture = <ColorMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexInput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

VertexOutput vertexShader(VertexInput input)
{
    VertexOutput output = (VertexOutput)0;
    float4x4 WorldViewProjection = mul(mul(World, View), Projection);
    output.TexCoord = input.TexCoord  + Timer * .005;

    float4 Pos = input.Position;
    float y = Pos.y * Offset + Timer;
    float x = sin(y) * Offset;
    Pos.x += x;

    output.Position = mul(Pos, WorldViewProjection);

    return( output );
}

struct PixelInput
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

float4 pixelShader(PixelInput input) : SV_TARGET0
{
    float4 color;    
    color = tex2D(ColorMapSampler, input.TexCoord);
    return(color);
}

technique Default
{
    pass P0
    {
        VertexShader = compile vs_4_0 vertexShader();
        PixelShader = compile ps_4_0 pixelShader();
    }
}
