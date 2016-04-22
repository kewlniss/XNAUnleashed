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
	
	//if (input.TexCoord.x > 0.5f) //uncomment to split screen
		color = dot(color.rgb, float3(0.3, 0.59, 0.11));

/*
//uncomment to saturate the middle of the screen
//with gray on either edge	
	float4 color = tex2D( TextureSampler, input.TexCoord);
	float4 gs = dot(color.rgb, float3(0.3, 0.59, 0.11));	
	if (input.TexCoord.x > 0.5f) //uncomment to split screen
		color = lerp(gs, color, (1 - input.TexCoord.x) * 2);
	else
		color = lerp(gs, color, input.TexCoord.x * 2);
*/		
	return( color );
}

technique Default
{
	pass P0
	{
		PixelShader = compile ps_4_0 pixelShader();
	}
}
