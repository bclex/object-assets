namespace OA.Ultima.Resources
{
    public interface IResource<T>
    {
        T GetResource(int resourceIndex);
    }
}
