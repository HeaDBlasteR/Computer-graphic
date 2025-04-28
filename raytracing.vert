#version 330 core
in vec3 vPosition; //¬ходные переменные vPosition - позици€ вершины
out vec3 glPosition;
void main (void)
{
	gl_Position = vec4(vPosition, 1.0);
	glPosition = vPosition;
}