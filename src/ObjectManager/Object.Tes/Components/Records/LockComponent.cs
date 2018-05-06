using OA.Tes.FilePacks.Records;

namespace OA.Tes.Components.Records
{
    public class LockComponent : GenericObjectComponent
    {
        void Start()
        {
            usable = true;
            pickable = false;
            var LOCK = (LOCKRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = LOCK.FNAM.Value;
            objData.weight = LOCK.LKDT.Weight.ToString();
            objData.value = LOCK.LKDT.Value.ToString();
        }
    }
}