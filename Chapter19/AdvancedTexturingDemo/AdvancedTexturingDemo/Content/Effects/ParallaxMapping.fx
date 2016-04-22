float4x4 World : WORLD;
float4x4 View;
float4x4 Projection;

float4 AmbientColor : COLOR0;
float3 LightPosition;
float4 LightDiffuseColor : COLOR1;

float2 ScaleAmount;
float3 CameraPosition;

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

texture2D NormalMap;

sampler2D NormalMapSampler = sampler_state
{
    texture = <NormalMap>;
    minFilter = linear;
    magFilter = linear;
    mipFilter = linear;
};

texture2D DepthMap;

sampler2D DepthMapSampler = sampler_state
{
    texture = <DepthMap>;
    minFilter = linear;
    magFilter = linear;
    mipFilter = linear;
};

struct VertexInput
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float3 Tangent : TANGENT;
    float2 TexCoord : TEXCOORD0;	
};

struct VertexOutput
{
    float4 Position : POSITION;
    float2 TexCoord : TEXCOORD0;	
    float3 LightDirection : TEXCOORD1;
    float2 Normal : TEXCOORD2;
    float3 ViewDirection : TEXCOORD3;
};

VertexOutput vertexShader(VertexInput input)
{
    VertexOutput output;

    output.LightDirection = LightPosition - input.Position;

    float3 worldPosition = mul(float4(input.Position, 1.0f), World).xyz;
    output.ViewDirection = worldPosition  - CameraPosition;

    float4x4 worldViewProjection = mul(mul(World, View), Projection);
    output.Position = mul(float4(input.Position, 1.0f), worldViewProjection);
    output.TexCoord = input.TexCoord;

    output.Normal = input.TexCoord;

    // Transform the light vector from object space into tangent space
    float3x3 tbnMatrix;
    tbnMatrix[0] = mul(input.Tangent, (float3x3)worldViewProjection);
    tbnMatrix[1] = mul(cross(input.Tangent, input.Normal),
		(float3x3)worldViewProjection);
    tbnMatrix[2] = mul(input.Normal, (float3x3)World);
    output.LightDirection = mul(tbnMatrix, output.LightDirection);
    output.ViewDirection = mul(tbnMatrix, output.ViewDirection);
    return(output);
}

float4 pixelShader(VertexOutput input) : COLOR
{
    float3 viewDirection = normalize(input.ViewDirection);
    float depth = ScaleAmount.x *
        (float)tex2D(DepthMapSampler, input.TexCoord) + ScaleAmount.y;
    input.TexCoord = depth * (float)viewDirection + input.TexCoord;

    input.LightDirection = normalize(input.LightDirection);

    float3 Normal = 2.0f * (tex2D(NormalMapSampler, input.Normal).rgb - 0.5f);

    return( (LightDiffuseColor * saturate(dot(input.LightDirection, Normal)) +
        AmbientColor) * tex2D(ColorMapSampler, input.TexCoord));
}

technique Default
{
    pass P0
    {
        VertexShader = compile vs_4_0 vertexShader();
        PixelShader = compile ps_4_0 pixelShader();
    }
}
