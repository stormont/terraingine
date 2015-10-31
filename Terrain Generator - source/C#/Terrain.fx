/********************************************************
* Voyage Terraingine v1.0
*
* Effect file for rendering terrain.
*
* This file is split into divisions according to the
* number of textures capable of being rendered by the 
* end-user's video card (maximum of 8 displayed).
********************************************************/

//
// Transformations
//
float4x4 WorldView	: WORLDVIEW;
float4x4 Projection : PROJECTION;


//
// Primary light
//
float3 PrimaryLight < string UIDirectional = "Light Direction"; >;


//
// Textures
//
texture Texture1;
texture Texture2;
texture Texture3;
texture Texture4;
texture Texture5;
texture Texture6;
texture Texture7;
texture Texture8;


//
// Texture samplers
//
sampler Texture1Sampler =
sampler_state
{
    Texture = <Texture1>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
};

sampler Texture2Sampler =
sampler_state
{
    Texture = <Texture2>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
};

sampler Texture3Sampler =
sampler_state
{
    Texture = <Texture3>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
};

sampler Texture4Sampler =
sampler_state
{
    Texture = <Texture4>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
};

sampler Texture5Sampler =
sampler_state
{
    Texture = <Texture5>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
};

sampler Texture6Sampler =
sampler_state
{
    Texture = <Texture6>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
};

sampler Texture7Sampler =
sampler_state
{
    Texture = <Texture7>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
};

sampler Texture8Sampler =
sampler_state
{
    Texture = <Texture8>;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    MipFilter = LINEAR;
};


/*
***	Rendering 0 textures
*/


//
// Vertex shader input struct
//
struct VS_INPUT0
{
    float3 position : POSITION;
    float3 normal : NORMAL;
};


//
// Vertex shader output struct
//
struct VS_OUTPUT0
{
    float4 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader
//
VS_OUTPUT0 VS0( VS_INPUT0 input )
{
    VS_OUTPUT0 Out = (VS_OUTPUT0) 0;				// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}


//
// Effect technique for rendering vertices
//
technique TTerrain0
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS0();
    }  
}


/*
***	Rendering 1 texture
*/


//
// Vertex shader input struct
//
struct VS_INPUT1
{
    float3 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader output struct
//
struct VS_OUTPUT1
{
    float4 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader
//
VS_OUTPUT1 VS1( VS_INPUT1 input )
{
    VS_OUTPUT1 Out = (VS_OUTPUT1) 0;				// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}


//
// Effect technique for rendering vertices
//
technique TTerrain1
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS1();
    }  
}


/*
***	Rendering 2 textures
*/


//
// Vertex shader input struct
//
struct VS_INPUT2
{
    float3 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader output struct
//
struct VS_OUTPUT2
{
    float4 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader
//
VS_OUTPUT2 VS2( VS_INPUT2 input )
{
    VS_OUTPUT2 Out = (VS_OUTPUT2) 0;				// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}


//
// Effect technique for rendering vertices
//
technique TTerrain2
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS2();
    }  
}


/*
***	Rendering 3 textures
*/


//
// Vertex shader input struct
//
struct VS_INPUT3
{
    float3 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader output struct
//
struct VS_OUTPUT3
{
    float4 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader
//
VS_OUTPUT0 VS3( VS_INPUT3 input )
{
    VS_OUTPUT3 Out = (VS_OUTPUT3) 0;				// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}


//
// Effect technique for rendering vertices
//
technique TTerrain3
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS3();
    }  
}


/*
***	Rendering 4 textures
*/


//
// Vertex shader input struct
//
struct VS_INPUT4
{
    float3 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader output struct
//
struct VS_OUTPUT4
{
    float4 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader
//
VS_OUTPUT4 VS4( VS_INPUT4 input )
{
    VS_OUTPUT4 Out = (VS_OUTPUT4) 0;				// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}


//
// Effect technique for rendering vertices
//
technique TTerrain4
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS4();
    }  
}


/*
***	Rendering 5 textures
*/


//
// Vertex shader input struct
//
struct VS_INPUT5
{
    float3 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader output struct
//
struct VS_OUTPUT5
{
    float4 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader
//
VS_OUTPUT5 VS5( VS_INPUT5 input )
{
    VS_OUTPUT5 Out = (VS_OUTPUT5) 0;				// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}


//
// Effect technique for rendering vertices
//
technique TTerrain5
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS5();
    }  
}


/*
***	Rendering 6 textures
*/


//
// Vertex shader input struct
//
struct VS_INPUT6
{
    float3 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader output struct
//
struct VS_OUTPUT6
{
    float4 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader
//
VS_OUTPUT6 VS6( VS_INPUT6 input )
{
    VS_OUTPUT6 Out = (VS_OUTPUT6) 0;				// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}


//
// Effect technique for rendering vertices
//
technique TTerrain6
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS6();
    }  
}


/*
***	Rendering 7 textures
*/


//
// Vertex shader input struct
//
struct VS_INPUT7
{
    float3 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader output struct
//
struct VS_OUTPUT7
{
    float4 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader
//
VS_OUTPUT7 VS7( VS_INPUT7 input )
{
    VS_OUTPUT7 Out = (VS_OUTPUT7) 0;				// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}


//
// Effect technique for rendering vertices
//
technique TTerrain7
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS7();
    }  
}


/*
***	Rendering 8 textures
*/


//
// Vertex shader input struct
//
struct VS_INPUT8
{
    float3 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader output struct
//
struct VS_OUTPUT8
{
    float4 position : POSITION;
    float4 color : COLOR0;
};


//
// Vertex shader
//
VS_OUTPUT8 VS8( VS_INPUT8 input )
{
    VS_OUTPUT8 Out = (VS_OUTPUT8) 0;				// Initialize output struct
    float4x4 WorldView = mul( World, View );		// Set WorldView matrix
    float3 pos = mul( float4( input.position, 1 ), (float4x3) WorldView );	// Set position in view space

    Out.position = mul( float4( pos, 1 ), Projection );	// position (projected)
    Out.color = input.color;								// output color

    return Out;
}


//
// Effect technique for rendering vertices
//
technique TTerrain8
{
    pass P0
    {
		// Rendering states
		CullMode	= None;
		Lighting	= False;
        
        // Shaders
        VertexShader = compile vs_1_1 VS8();
    }  
}