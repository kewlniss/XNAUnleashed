float4x4 World	: WORLD;
float4x4 View;
float4x4 Projection;

float RotateAngle = 0;

texture ColorMap;
sampler2D ColorMapSampler = sampler_state
{
    Texture = <ColorMap>;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter = LINEAR;
    addressU = mirror;
    addressV = mirror;
};

struct VertexInput
{
    float4 Position	: POSITION0;
    //float Size		: PSIZE0;
    float4 Color 	: COLOR0;
};

struct VertexOutput
{
    float4 Position	: POSITION0;
    //float Size		: PSIZE0;
    float4 Color   	: COLOR0;
};

VertexOutput vertexShader (VertexInput input)
{
    VertexOutput output;
    float4x4 worldViewProjection = mul(mul(World, View), Projection);
    output.Position = mul(input.Position, worldViewProjection);    
    output.Color = input.Color;
    //output.Size = input.Size;

    return output;
}

struct PixelInput
{
    float4 Position : POSITION0;

    #ifdef XBOX360
        float4 TexCoords : SPRITETEXCOORD0;
    #else
        float2 TexCoords : TEXCOORD0;
    #endif
    float4 Color : COLOR0;
};

float4 pixelShader(PixelInput input) : SV_TARGET0
{ 
    float2 texCoords;

    #ifdef XBOX360
        texCoords = abs(input.TexCoords.zw);
    #else
        texCoords = input.TexCoords.xy;
    #endif    
    
    //only take the rotation penalty if we need to
	if (RotateAngle > 0)
	{
		texCoords -= .5f;

		float ca = cos(RotateAngle);
		float sa = sin(RotateAngle);
		float2 tempCoords;
		tempCoords.x = texCoords.x * ca - texCoords.y * sa;
		tempCoords.y = texCoords.x * sa + texCoords.y * ca;
		texCoords = tempCoords;
		texCoords *= 1.4142135623730951; //sqrt(2);

		texCoords += .5f;
	}

    return ( saturate(tex2D(ColorMapSampler, texCoords) * input.Color) );
}

technique Default
{
    pass P0
    {
        VertexShader = compile vs_4_0 vertexShader();
        PixelShader  = compile ps_4_0 pixelShader();
    }
}
