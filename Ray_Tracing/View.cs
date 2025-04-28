using System;
using System.IO;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

public class View
{
    private int BasicProgramID;
    private int BasicVertexShader;
    private int BasicFragmentShader;
    private int vbo_position;

    public void InitShaders()
    {
        BasicProgramID = GL.CreateProgram();
        loadShader("C:\\Users\\nozdr\\source\\repos\\Ray_Tracing\\raytracing.vert", ShaderType.VertexShader,
            BasicProgramID, out BasicVertexShader);
        loadShader("C:\\Users\\nozdr\\source\\repos\\Ray_Tracing\\raytracing.frag", ShaderType.FragmentShader,
            BasicProgramID, out BasicFragmentShader);
        GL.LinkProgram(BasicProgramID);

        Vector3[] vertdata = new Vector3[] {
            new Vector3(-1f, -1f, 0f),
            new Vector3( 1f, -1f, 0f),
            new Vector3( 1f,  1f, 0f),
            new Vector3(-1f,  1f, 0f) };

        GL.GenBuffers(1, out vbo_position);
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
        GL.BufferData<Vector3>(BufferTarget.ArrayBuffer, (IntPtr)(vertdata.Length * Vector3.SizeInBytes),
                             vertdata, BufferUsageHint.StaticDraw);

        int attribute_vpos = GL.GetAttribLocation(BasicProgramID, "vPosition");
        int uniform_pos = GL.GetUniformLocation(BasicProgramID, "campos");
        int uniform_aspect = GL.GetUniformLocation(BasicProgramID, "aspect");

        GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.Uniform3(uniform_pos, new Vector3(0, 0, 0));
        GL.Uniform1(uniform_aspect, 1.0f);
        GL.UseProgram(BasicProgramID);
        GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

        int status = 0;
        GL.GetProgram(BasicProgramID, GetProgramParameterName.LinkStatus, out status);
        Console.WriteLine(GL.GetProgramInfoLog(BasicProgramID));
    }

    void loadShader(String filename, ShaderType type, int program, out int address)
    {
        address = GL.CreateShader(type);
        using (System.IO.StreamReader sr = new StreamReader(filename))
        {
            GL.ShaderSource(address, sr.ReadToEnd());
        }
        GL.CompileShader(address);
        GL.AttachShader(program, address);
        Console.WriteLine(GL.GetShaderInfoLog(address));
    }
}