using OA.Tes.FilePacks.Records;
using UnityEngine;

namespace OA.Tes.FilePacks.Components
{
    public interface IUsableComponent
    {
        void Use();
    }

    public interface IPickableComponent
    {
        void Pick();
    }

    public class BASEComponent : MonoBehaviour
    {
        public class ObjectData
        {
            public Texture2D icon;
            public string interactionPrefix;
            public string name;
            public string weight;
            public string value;
        }

        protected Transform _transform = null;

        public CELLRecord.RefObj refObjDataGroup = null;
        public Record record;
        public ObjectData objData = new ObjectData();
        public bool usable = false;
        public bool pickable = true;

        protected virtual void Awake()
        {
            _transform = GetComponent<Transform>();
        }

        public virtual void Interact()
        {
        }

        public static BASEComponent Create(GameObject gameObject, Record record, string tag)
        {
            gameObject.tag = tag;
            var transform = gameObject.GetComponent<Transform>();
            for (int i = 0, l = transform.childCount; i < l; i++)
                transform.GetChild(i).tag = tag;
            BASEComponent component = null;
            if (record is DOORRecord) component = gameObject.AddComponent<DOORComponent>();
            else if (record is LIGHRecord) component = gameObject.AddComponent<LIGHComponent>();
            else if (record is BOOKRecord) component = gameObject.AddComponent<BOOKComponent>();
            else if (record is CONTRecord) component = gameObject.AddComponent<CONTComponent>();
            else if (record is MISCRecord) component = gameObject.AddComponent<MISCComponent>();
            else if (record is WEAPRecord) component = gameObject.AddComponent<WEAPComponent>();
            else if (record is ARMORecord) component = gameObject.AddComponent<ARMOComponent>();
            else if (record is INGRRecord) component = gameObject.AddComponent<INGRComponent>();
            else if (record is ACTIRecord) component = gameObject.AddComponent<ACTIComponent>();
            else if (record is LOCKRecord) component = gameObject.AddComponent<LOCKComponent>();
            else if (record is PROBRecord) component = gameObject.AddComponent<PROBComponent>();
            else if (record is REPARecord) component = gameObject.AddComponent<REPAComponent>();
            else if (record is CLOTRecord) component = gameObject.AddComponent<CLOTComponent>();
            else if (record is ALCHRecord) component = gameObject.AddComponent<ALCHComponent>();
            else if (record is APPARecord) component = gameObject.AddComponent<APPAComponent>();
            else if (record is CREARecord) component = gameObject.AddComponent<CREAComponent>();
            else if (record is NPC_Record) component = gameObject.AddComponent<NPC_Component>();
            else component = gameObject.AddComponent<BASEComponent>();
            component.record = record;
            return component;
        }
    }
}