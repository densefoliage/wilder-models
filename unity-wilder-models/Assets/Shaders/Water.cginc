static const half TIME_FACTOR = 0.125;
static const half TIME_FACTOR_VARIANCE = 0.05;
static const half STREAM_TIME_FACTOR = 0.25;

static const half NOISE_SCALE = 0.05;
static const half STREAM_NOISE_SCALE = 0.125;
static const half SHORE_SCALE = 0.8;

float Foam (float shore, float2 worldXZ, sampler2D noiseTex) {
	shore = sqrt(shore) * 0.8;

	float2 noiseUV = worldXZ + _Time.y * TIME_FACTOR;
	float4 noise = tex2D(noiseTex, noiseUV * (NOISE_SCALE * 0.5));

	float distortion1 = noise.x * (1 - shore);
	float foam1 = sin((shore + distortion1) * 10 - (_Time.y + TIME_FACTOR_VARIANCE));
	foam1 *= foam1;

	float distortion2 = noise.y * (1 - shore);
	float foam2 = sin((shore + distortion2) * 10 + (_Time.y - TIME_FACTOR_VARIANCE) + 2);
	foam2 *= foam2 * 0.7;

	return max(foam1, foam2) * shore;
}

float Waves (float2 worldXZ, sampler2D noiseTex) {
	float2 uv1 = worldXZ;
	uv1.y += (_Time.y * TIME_FACTOR);
	float4 noise1 = tex2D(noiseTex, uv1 * NOISE_SCALE);

	float2 uv2 = worldXZ;
	uv2.x += (_Time.y * TIME_FACTOR * (1-TIME_FACTOR_VARIANCE));
	float4 noise2 = tex2D(noiseTex, uv2 * NOISE_SCALE);

	float blendWave = sin(
		(worldXZ.x + worldXZ.y) * 0.1 +
		(noise1.y + noise2.z) + (_Time.y * TIME_FACTOR)
	);
	blendWave *= blendWave;

	float waves =
		lerp(noise1.z, noise1.w, blendWave) +
		lerp(noise2.x, noise2.y, blendWave);
	return smoothstep(0.75, 2, waves);
}

float Stream (float2 streamUV, sampler2D noiseTex) {
	float2 uv = streamUV;
	uv.x = uv.x * STREAM_NOISE_SCALE + (_Time.y * TIME_FACTOR * 0.05);
	uv.y -= _Time.y * STREAM_TIME_FACTOR;
	float4 noise = tex2D(noiseTex, uv);

	float2 uv2 = streamUV;
	uv2.x = uv2.x * STREAM_NOISE_SCALE - (_Time.y * TIME_FACTOR * 0.05);
	uv2.y -= _Time.y * STREAM_TIME_FACTOR * (1 - TIME_FACTOR_VARIANCE);
	float4 noise2 = tex2D(noiseTex, uv2);
	
	return noise.x * noise2.w;
}