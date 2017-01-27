using System;
using System.Collections.Generic;
using OpenTK;

namespace engenious.Graphics
{
    public sealed class EffectParameter
    {
        private List<EffectPassParameter> parameters;

        public EffectParameter(string name)
        {
            this.parameters = new List<EffectPassParameter>();
            this.Name = name;
        }

        internal void Add(EffectPassParameter param)
        {
            parameters.Add(param);
        }

        public string Name{ get; private set; }

        public void SetValue(bool value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(bool[] values)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(values);
                }
            }
        }

        public void SetValue(int value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(int[] values)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(values);
                }
            }
        }

        public void SetValue(uint value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(uint[] values)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(values);
                }
            }
        }

        public void SetValue(float value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(float[] values)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(values);
                }
            }
        }

        public void SetValue(string value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(Texture value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(Vector2 value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(Vector2[] values)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(values);
                }
            }
        }

        public void SetValue(Vector3 value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(Vector3[] values)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(values);
                }
            }
        }

        public void SetValue(Vector4 value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(Vector4[] values)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(values);
                }
            }
        }

        public void SetValue(Matrix value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(Matrix[] values)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(values);
                }
            }
        }

        public void SetValue(Quaternion value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }

        public void SetValue(Quaternion[] values)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(values);
                }
            }
        }

        public void SetValue(ConstantBuffer value)
        {
            using (Execute.OnUiContext)
            {
                foreach (var param in parameters)
                {
                    param.pass.Apply();
                    param.SetValue(value);
                }
            }
        }
    }
}

