using System;
using System.Collections.Generic;
using System.Text;
using engenious.Helper;
using OpenTK.Graphics.OpenGL4;

namespace engenious.Graphics
{
    public class EffectPass :IDisposable
    {
        internal int Program;

        protected internal EffectPass(string name)//TODO: content loading
        {
            Name = name;
            using (Execute.OnUiContext)
            {
                Program = GL.CreateProgram();
            }

        }

        internal void BindAttribute(VertexElementUsage usage, string name)
        {
            using (Execute.OnUiContext)
            {
                GL.BindAttribLocation(Program, (int) usage, name);
            }
        }

        protected internal virtual void CacheParameters()
        {
            var total = -1;
            using (Execute.OnUiContext)
            {
                GL.GetProgram(Program, GetProgramParameterName.ActiveUniforms, out total);
                for (var i = 0; i < total; ++i)
                {
                    int size;
                    ActiveUniformType type;
                    var name = GL.GetActiveUniform(Program, i, out size, out type);
                    var location = GetUniformLocation(name);
                    Parameters.Add(new EffectPassParameter(this, name, location,type));
                }
                GL.GetProgram(Program, GetProgramParameterName.ActiveUniformBlocks, out total);
                for (var i = 0; i < total; ++i)
                {
                    int size;
                    var sb = new StringBuilder(512);
                    GL.GetActiveUniformBlockName(Program, i, 512, out size, sb);
                    var name = sb.ToString();
                    var location = GL.GetUniformBlockIndex(Program, name);
                    Parameters.Add(new EffectPassParameter(this, name, location,(ActiveUniformType)0x7FFFFFFF));//TODO: 
                }
                //TODO: ssbos?

            }
        }

        internal int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(Program, name);
        }

        internal List<Shader> Attached = new List<Shader>();

        internal void AttachShaders(IEnumerable<Shader> shaders)
        {
            using (Execute.OnUiContext)
            {
                foreach (var shader in shaders)
                {
                    AttachShader(shader);
                }
            }
        }

        internal void AttachShader(Shader shader)
        {
            using (Execute.OnUiContext)
            {
                GL.AttachShader(Program, shader.BaseShader);
            }
        }

        internal void Link()
        {
            if (Attached == null)
                throw new Exception("Already linked");
            using (Execute.OnUiContext)
            {
                GL.LinkProgram(Program);
                int linked;
                GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out linked);
                if (linked != 1)
                {
                    var error = GL.GetProgramInfoLog(Program);
                    if (string.IsNullOrEmpty(error))
                        throw new Exception("Unknown error occured");
                    throw new Exception(error);
                }
                foreach (var shader in Attached)
                {
                    GL.DetachShader(Program, shader.BaseShader);
                }
            }
            Attached.Clear();
            Attached = null;
            using (Execute.OnUiContext)
            {
                Parameters = new EffectPassParameterCollection(this);
            }
        }

        protected internal EffectPassParameterCollection Parameters{ get; private set; }

        public string Name
        {
            get;
            private set;
        }

        public void Apply()
        {
            using (Execute.OnUiContext)
            {
                GL.UseProgram(Program);
            }
        }


        public void Compute(int x,int y=1,int z=1)
        {
            using (Execute.OnUiContext)
            {
                GL.UseProgram(Program);
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
                GL.DeleteProgram(Program);
            }
        }

        public BlendState BlendState{ get; internal set; }
        //TODO: apply states

        public DepthStencilState DepthStencilState{ get; internal set; }

        public RasterizerState RasterizerState{ get; internal set; }
    }
}

