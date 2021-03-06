// Pcx - Point cloud importer & renderer for Unity
// https://github.com/keijiro/Pcx

using UnityEngine;
using System.Collections.Generic;

namespace Pcx
{
    /// A container class optimized for compute buffer.
    public sealed class PointCloudData : ScriptableObject
    {
        #region Public properties and methods

        /// Number of points.
        public int pointCount {
            get { return _pointData.Length; }
        }

        /// Create a compute buffer.
        /// The returned buffer must be released by the caller.
        public ComputeBuffer CreateComputeBuffer()
        {
            var buffer = new ComputeBuffer(pointCount, sizeof(float) * 4);
            buffer.SetData(_pointData);
            return buffer;
        }

        #endregion

        #region Serialized data members

        [System.Serializable]
        struct Point
        {
            public Vector3 position;
            public uint color;
        }

        [SerializeField] Point[] _pointData;

        #endregion

        #region Editor functions

        #if UNITY_EDITOR

        static uint EncodeColor(Color c)
        {
            const float kMaxBrightness = 16;

            var y = Mathf.Max(Mathf.Max(c.r, c.g), c.b);
            y = Mathf.Clamp(Mathf.Ceil(y * 255 / kMaxBrightness), 1, 255);

            var rgb = new Vector3(c.r, c.g, c.b);
            rgb *= 255 * 255 / (y * kMaxBrightness);

            return ((uint)rgb.x      ) |
                   ((uint)rgb.y <<  8) |
                   ((uint)rgb.z << 16) |
                   ((uint)y     << 24);
        }

        public void Initialize(List<Vector3> positions, List<Color32> colors)
        {
            _pointData = new Point[positions.Count];
            for (var i = 0; i < _pointData.Length; i++)
            {
                _pointData[i] = new Point {
                    position = positions[i],
                    color = EncodeColor(colors[i])
                };
            }
        }

        #endif

        #endregion
    }
}
