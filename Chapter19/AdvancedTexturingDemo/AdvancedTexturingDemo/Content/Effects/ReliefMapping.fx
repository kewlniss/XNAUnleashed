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

texture2D ReliefMap;
sampler2D ReliefMapSampler = sampler_state
{
    texture = <ReliefMap>;
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
    float3 ViewDirection : TEXCOORD3;
};

VertexOutput vertexShader(VertexInput input)
{
    VertexOutput output;
    output.LightDirection = LightPosition - input.Position;

    float3 worldPosition = mul(float4(input.Position, 1.0f), World).xyz;
    output.ViewDirection = worldPosition - CameraPosition;

    float4x4 worldViewProjection = mul(mul(World, View), Projection);
    output.Position = mul(float4(input.Position, 1.0f), worldViewProjection);
    output.TexCoord = input.TexCoord;

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
    const int numStepsLinear = 15;  //linear search number of steps
    const int numStepsBinary = 6;  //binary search number of steps

    float3 position = float3(input.TexCoord,0);
    float3 viewDirection = normalize(input.ViewDirection);

    float depthBias = 1.0 - viewDirection.z;
    depthBias *= depthBias; 
    depthBias *= depthBias; 
    depthBias = 1.0 - depthBias * depthBias;
    viewDirection.xy *= depthBias;
    viewDirection.xy *= ScaleAmount;

// ray intersect depth map using linear and binary searches
// depth value stored in alpha channel (black at is object surface)
    viewDirection /= viewDirection.z * numStepsLinear;
    int i;
    for( i=0; i<numStepsLinear; i++ )
    {
        float4 tex = tex2D(ReliefMapSampler, position.xy);
        if (position.z < tex.w)
            position += viewDirection;
    }
    for( i=0; i<numStepsBinary; i++ )
    {
        viewDirection *= 0.5;
        float4 tex = tex2D(ReliefMapSampler, position.xy);
        if (position.z < tex.w)
            position += viewDirection;
        else
            position -= viewDirection;
    }

    input.TexCoord = position.xy;
    //transform to tangent space
    viewDirection = normalize(input.ViewDirection);
    input.LightDirection = normalize(input.LightDirection);

    float3 Normal = (float3)(2.0f * (tex2D(ReliefMapSampler, input.TexCoord) - 0.5f));
    Normal.y = -Normal.y;
    Normal.z = sqrt(1.0 - Normal.x*Normal.x - Normal.y*Normal.y);

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
