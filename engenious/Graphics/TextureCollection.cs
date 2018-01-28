using System;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace engenious.Graphics
{
    public sealed class TextureCollection : ICollection<Texture>
    {
        private readonly Texture[] _textures;

        public TextureCollection()
        {
            var maxTextures = GL.GetInteger(GetPName.MaxTextureImageUnits);
            _textures = new Texture[maxTextures];
        }

        public Texture this [int index]
        { 
            get
            {
                return _textures[index];
            }
            set
            {
                if (_textures[index] != value)
                {
                    _textures[index] = value;
                    GL.ActiveTexture(TextureUnit.Texture0 + index);
                    if (value == null)
                        GL.BindTexture(TextureTarget.Texture2D, 0);
                    else
                        value.Bind();

                }

            }

        }

        #region ICollection implementation

        public int InsertFree(Texture item)
        {
            var ind = IndexOf(item);
            if (ind != -1)
                return ind;
            ind = IndexOf(null);
            if (ind == -1)
                return -1;
            this[ind] = item;
            return ind;
        }

        public void Add(Texture item)
        {
            var free = IndexOf(null);
            if (free == -1)
                return;
            this[free] = item;
        }

        public void Clear()
        {
            for (var i = 0; i < _textures.Length; i++)
                this[i] = null;
        }

        public bool Contains(Texture item)
        {
            return IndexOf(item) != -1;
        }

        public int IndexOf(Texture item)
        {
            for (var i = 0; i < _textures.Length; i++)
                if (_textures[i] == item)
                    return i;
            return -1;
        }

        public void CopyTo(Texture[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Texture item)
        {
            var ind = IndexOf(item);
            if (ind == -1)
                return false;
            this[ind] = null;
            return true;
        }

        public int Count
        {
            get
            {
                return _textures.Length;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }


        #endregion

        #region IEnumerable implementation

        public IEnumerator<Texture> GetEnumerator()
        {
            return (IEnumerator<Texture>)_textures.GetEnumerator();
        }

        #endregion

        #region IEnumerable implementation

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _textures.GetEnumerator();
        }

        #endregion
    }
}

