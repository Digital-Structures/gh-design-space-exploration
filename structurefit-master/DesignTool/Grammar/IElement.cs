namespace StructureEngine.Grammar
{
    public interface IElement
    {
        IElement Clone();
        bool IsSame(IElement elem);
    }
}
