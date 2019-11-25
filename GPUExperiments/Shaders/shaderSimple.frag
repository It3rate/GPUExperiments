#version 430 core

uniform sampler2D destTex;
in vec4 vColor;
in vec2 texCoord;
out vec4 color;

void main()
{
	float edge = smoothstep(0.96, 0.95, vColor.a) - smoothstep(0.72, 0.7, vColor.a);
    color = vColor * edge;
	if(vColor.a < 0.7)
	{
		color = texture(destTex, texCoord) * smoothstep(0.7, 0.6, vColor.a);
	}
	
}
// #version 430\n
// uniform sampler2D srcTex; 
// in vec2 texCoord; 
// out vec4 color;

// void main() { 
// float c = texture(srcTex, texCoord).x;
// color = vec4(c, 1.0, 1.0, 1.0); 
// }