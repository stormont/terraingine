/********************************************************
* Voyage Terraingine v1.0
*
* Effect file for rendering vertices for easy selection.
********************************************************/

// Transformations
float4x4 World      : WORLD;
float4x4 View       : VIEW;
float4x4 Projection : PROJECTION;

// Vertex shader input struct
struct VS_INPUT
{
    float3 position : POSITION;
    float4 color : COLOR0;
};

// Vertex shader output struct
struct VS_OUTPUT
{
    float4 position : POSITION;
    float4 color : COLOR0;
};

// Vertex shader
VS_OUTPUT VS( VS_INPUT input )
{
    VS_OUTPUT Out = (VS_OUTPUT) 0;					// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position  = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}

// Effect technique for rendering vertices
technique TVertices
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS();
    }  
}