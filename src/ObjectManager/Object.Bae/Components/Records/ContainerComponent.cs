using OA.Bae.Esm;

namespace OA.Bae.Components.Records
{
    public class ContainerComponent : GenericObjectComponent
    {
        void Start()
        {
            pickable = false;
            objData.name = ((CONTRecord)record).FNAM.value;
            objData.interactionPrefix = "Open ";
        }
    }
}