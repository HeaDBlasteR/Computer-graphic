using System.Drawing;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using System;
using OpenTK.Mathematics;
using System.IO;

namespace Ray_Tracing
{
    internal class View
    {
        private int BasicProgramID;
        private int BasicVertexShader;
        private int BasicFragmentShader;
        private Vector3[] vertdata;
        private int attribute_vpos = 0;
        private int uniform_pos;
        private int uniform_aspect;
        private Vector3 campos = new Vector3(0, 0, 0);
        private float aspect = 1.0f;
        private int vbo_position;

        public void InitShaders()
        {
            BasicProgramID = GL.CreateProgram();
            loadShader("raytracing.vert", ShaderType.VertexShader, BasicProgramID, out BasicVertexShader);
            loadShader("raytracing.frag", ShaderType.FragmentShader, BasicProgramID, out BasicFragmentShader);
            GL.LinkProgram(BasicProgramID);

            vertdata = new Vector3[] {
                new Vector3(-1f, -1f, 0f),
                new Vector3(1f, -1f, 0f),
                new Vector3(1f, 1f, 0f),
                new Vector3(-1f, 1f, 0f) };

            GL.GenBuffers(1, out vbo_position);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.BufferData(BufferTarget.ArrayBuffer, vertdata.Length * Vector3.SizeInBytes,
                vertdata, BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            uniform_pos = GL.GetUniformLocation(BasicProgramID, "pos");
            uniform_aspect = GL.GetUniformLocation(BasicProgramID, "aspect");

            GL.Uniform3(uniform_pos, campos);
            GL.Uniform1(uniform_aspect, aspect);
            GL.UseProgram(BasicProgramID);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        void loadShader(string filename, ShaderType type, int program, out int address)
        {
            string shaderCode = File.ReadAllText(filename);
            address = GL.CreateShader(type);
            GL.ShaderSource(address, shaderCode);
            GL.CompileShader(address);
            GL.AttachShader(program, address);
        }

        public void Draw()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(BasicProgramID);
            GL.EnableVertexAttribArray(attribute_vpos);
            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo_position);
            GL.VertexAttribPointer(attribute_vpos, 3, VertexAttribPointerType.Float, false, 0, 0);

            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);

            GL.DisableVertexAttribArray(attribute_vpos);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.UseProgram(0);
        }
    }
}