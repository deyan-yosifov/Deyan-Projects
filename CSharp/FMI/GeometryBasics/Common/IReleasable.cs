using System;

namespace GeometryBasics.Common
{
    public interface IReleasable
    {
        void Initialize();
        void Release();
    }
}
