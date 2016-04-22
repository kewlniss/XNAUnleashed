sampler TextureSampler;

struct PixelInput
{
	float4 Pos : SV_POSITION;
	float4 Color : COLOR0;
	float2 TexCoord : TEXCOORD0;
};


float4 pixelShader(PixelInput input) : SV_TARGET0
{
	float4 color = tex2D( TextureSampler, input.TexCoord);
	color += tex2D( TextureSampler, input.TexCoord+0.01);
	color += tex2D( TextureSampler, input.TexCoord-0.02);
	color += tex2D( TextureSampler, input.TexCoord+0.01);
	color = color / 4;
	
	return( color );
}

technique Default
{
	pass P0
	{
		PixelShader = compile ps_4_0 pixelShader();
	}
}

