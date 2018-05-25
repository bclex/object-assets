using OA.Tes.FilePacks.Records;

namespace OA.Tes.FilePacks.Components
{
    public class CONTComponent : BASEComponent
    {
        void Start()
        {
            pickable = false;
            objData.name = ((CONTRecord)record).FULL.Value;
            objData.interactionPrefix = "Open ";
        }
    }
}