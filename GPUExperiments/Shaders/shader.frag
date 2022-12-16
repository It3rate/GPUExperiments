#version 330 core

in vec3 vColor;
in vec3 pos;
in vec3 norm;
out vec4 color;


float roundedRectangle (vec2 uv, vec2 pos, vec2 size, float radius, float thickness)
{
  float d = length(max(abs(uv - pos),size) - size) - radius;
  return smoothstep(0.66, 0.33, d / thickness * 5.0);
}

void main()
{
  vec3 kColor = vec3(0.4, 0.0, 0.0);
  //float len = abs(pos.x);//+pos.y+pos.z);
  float len = length(pos)+(pos.y*.7);
  float intensity = smoothstep(0.9, 1.0, (len - .2)*10.0);
  //float intensity = len < .5 ? 0.0 : 0.7;
  color = vec4(mix(vColor, 1.0-vColor + norm, intensity), 1);
}
//void mainX()
//{
//    color = vec4(vColor, 1);
//}
