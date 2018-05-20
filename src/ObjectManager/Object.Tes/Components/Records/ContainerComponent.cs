using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class ContainerComponent : GenericObjectComponent
    {
        void Start()
        {
            pickable = false;
            objData.name = ((CONTRecord)record).FULL.Value;
            objData.interactionPrefix = "Open ";
        }
    }
}