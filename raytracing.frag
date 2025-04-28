#version 330 core
out vec4 FragColor;
in vec3 glPosition;
void main ( void )
{
	FragColor = vec4 ( abs(glPosition.xy), 0, 1.0);
}