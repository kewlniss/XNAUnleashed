float4x4 World : WORLD;
float4x4 View;
float4x4 Projection;

float4 AmbientColor : COLOR0;
float3 LightPosition;

texture ColorMap;

sampler2D ColorMapSampler = sampler_state
{
    texture = <ColorMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    addressU = mirror;
    addressV = mirror;
};

struct VertexInput
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TexCoord : TEXCOORD0;
};

struct VertexOutput
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;
    float3 WorldSpacePosition : TEXCOORD1;
    float3 Normal : TEXCOORD2;
};

VertexOutput vertexShader(VertexInput input)
{
    VertexOutput output;
    
    output.WorldSpacePosition = mul(float4(input.Position, 1.0f), World).xyz;
    
    float4x4 worldViewProjection = mul(mul(World, View), Projection);
    output.Position = mul(float4(input.Position, 1.0f), worldViewProjection);
    output.TexCoord = input.TexCoord;
    output.Normal = normalize(mul(input.Normal, (float3x3)World)).xyz;
    
    return output;
}

float4 pixelShader(VertexOutput input) : COLOR
{
    float3 LightDirection = normalize(LightPosition - input.WorldSpacePosition);
    float DiffuseLight = dot(LightDirection, input.Normal);

    return( tex2D(ColorMapSampler, input.TexCoord) * DiffuseLight + AmbientColor);
}

technique Default
{
    pass P0
    {
        VertexShader = compile vs_4_0 vertexShader();
        PixelShader = compile ps_4_0 pixelShader();
    }
}
