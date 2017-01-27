using System;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace engenious.Graphics
{
    public sealed class EffectPass :IDisposable
    {
        internal int program;

        internal EffectPass(string name)//TODO: content loading
        {
            this.Name = name;
            using (Execute.OnUiContext)
            {
                program = GL.CreateProgram();
            }

        }

        internal void BindAttribute(VertexElementUsage usage, string name)
        {
            using (Execute.OnUiContext)
            {
                GL.BindAttribLocation(program, (int) usage, name);
            }
        }

        internal void CacheParameters()
        {
            int total = -1;
            using (Execute.OnUiContext)
            {
                GL.GetProgram(program, GetProgramParameterName.ActiveUniforms, out total);
                for (int i = 0; i < total; ++i)
                {
                    int size;
                    ActiveUniformType type;
                    string name = GL.GetActiveUniform(program, i, out size, out type);
                    int location = GetUniformLocation(name);
                    Parameters.Add(new EffectPassParameter(this, name, location));
                }
                GL.GetProgram(program, GetProgramParameterName.ActiveUniformBlocks, out total);
                for (int i = 0; i < total; ++i)
                {
                    int size;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(512);
                    GL.GetActiveUniformBlockName(program, i, 512, out size, sb);
                    string name = sb.ToString();
                    int location = i; //TODO: is index really the correct location?
                    location = GL.GetUniformBlockIndex(program, name);
                    Parameters.Add(new EffectPassParameter(this, name, location));
                }
                //TODO: ssbos?

            }
        }

        internal int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(program, name);
        }

        internal List<Shader> attached = new List<Shader>();

        internal void AttachShaders(IEnumerable<Shader> shaders)
        {
            using (Execute.OnUiContext)
            {
                foreach (Shader shader in shaders)
                {
                    AttachShader(shader);
                }
            }
        }

        internal void AttachShader(Shader shader)
        {
            using (Execute.OnUiContext)
            {
                GL.AttachShader(program, shader.shader);
            }
        }

        internal void Link()
        {
            if (attached == null)
                throw new Exception("Already linked");
            using (Execute.OnUiContext)
            {
                GL.LinkProgram(program);
                int linked;
                GL.GetProgram(program, GetProgramParameterName.LinkStatus, out linked);
                if (linked != 1)
                {
                    string error = GL.GetProgramInfoLog(program);
                    if (string.IsNullOrEmpty(error))
                        throw new Exception("Unknown error occured");
                    throw new Exception(error);
                }
                foreach (Shader shader in attached)
                {
                    GL.DetachShader(program, shader.shader);
                }
            }
            attached.Clear();
            attached = null;
            using (Execute.OnUiContext)
            {
                Parameters = new EffectPassParameterCollection(this);
            }
        }

        internal EffectPassParameterCollection Parameters{ get; private set; }

        public string Name
        {
            get;
            private set;
        }

        public void Apply()
        {
            using (Execute.OnUiContext)
            {
                GL.UseProgram(program);
            }
        }


        public void Compute(int x,int y=1,int z=1)
        {
            using (Execute.OnUiContext)
            {
                GL.UseProgram(program);
                GL.DispatchCompute(x, y, z);
            }
        }

        public void WaitForImageCompletion()
        {
            using (Execute.OnUiContext)
            {
                GL.MemoryBarrier(MemoryBarrierFlags.ShaderImageAccessBarrierBit);
            }
        }

        public void Dispose()
        {
            using (Execute.OnUiContext)
            {
                GL.DeleteProgram(program);
            }
        }

        public BlendState BlendState{ get; internal set; }
        //TODO: apply states

        public DepthStencilState DepthStencilState{ get; internal set; }

        public RasterizerState RasterizerState{ get; internal set; }
    }
}

