using System;

namespace StructureEngine.Grammar
{
    public interface IShapeState : IEquatable<IShapeState>
    {
        string Name { get; }
        bool IsEnd();
    }
}
