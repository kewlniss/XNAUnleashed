float4x4 World	: WORLD;
float4x4 View;
float4x4 Projection;

float4 AmbientColor : COLOR0;

float4x4 WorldViewProjection : WORLDVIEWPROJECTION;
texture Texture;
sampler TextureSampler = sampler_state
{
    texture = <Texture>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    AddressU = mirror;
    AddressV = mirror;
};

struct VertexInput
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
};

VertexOutput vertexShader(VertexInput input)
{
    VertexOutput output;
    WorldViewProjection = mul(mul(World, View), Projection);
    output.Position = mul(input.Position, WorldViewProjection);
    output.TexCoord = input.TexCoord;
    return( output );
}

struct PixelInput
{
    float2 TexCoord : TEXCOORD0;	
};

float4 pixelShader(PixelInput input) : COLOR
{
    return( tex2D(TextureSampler, input.TexCoord) * AmbientColor);
}

technique Default
{
    pass P0
    {
        VertexShader = compile vs_4_0 vertexShader();
        PixelShader = compile ps_4_0 pixelShader();
    }
}
