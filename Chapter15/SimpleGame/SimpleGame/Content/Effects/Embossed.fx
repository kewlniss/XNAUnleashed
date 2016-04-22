sampler TextureSampler;

struct PixelInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float2 TexCoord : TEXCOORD0;
};


float4 pixelShader(PixelInput input) : SV_TARGET0
{
	float sharpAmount = 15.0f;
	float4 color;
	color.rgb = 0.5f;
	color.a = 1.0f;
	color += tex2D( TextureSampler, input.TexCoord - 0.0001) * sharpAmount;
	color -= tex2D( TextureSampler, input.TexCoord + 0.0001) * sharpAmount;
	color = (color.r+color.g+color.b) / 3.0f;
	
	return( color );
}

technique Default
{
	pass P0
	{
		PixelShader = compile ps_4_0 pixelShader();
	}
}

