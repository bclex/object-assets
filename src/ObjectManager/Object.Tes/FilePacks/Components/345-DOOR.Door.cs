using UnityEngine;
using System.Collections;
using OA.Tes.FilePacks.Records;
using OA.Tes.Formats;

namespace OA.Tes.FilePacks.Components
{
    public class DOORComponent : BASEComponent
    {
        public class DoorData
        {
            public string doorName;
            public string doorExitName;
            public bool leadsToAnotherCell;
            public bool leadsToInteriorCell;
            public Vector3 doorExitPos;
            public Quaternion doorExitOrientation;

            public bool isOpen;
            public Quaternion closedRotation;
            public Quaternion openRotation;
            public bool moving = false;
        }

        public DoorData doorData = null;

        void Start()
        {
            usable = true;
            pickable = false;
            doorData = new DoorData { closedRotation = transform.rotation };
            doorData.openRotation = doorData.closedRotation * Quaternion.Euler(Vector3.up * 90f);
            doorData.moving = false;
            var DOOR = record as DOORRecord;
            if (DOOR.FULL.Value != null)
                doorData.doorName = DOOR.FULL.Value;
            doorData.leadsToAnotherCell = refObjDataGroup.DNAM.Value != null || refObjDataGroup.DODT != null;
            doorData.leadsToInteriorCell = refObjDataGroup.DNAM.Value != null;
            if (doorData.leadsToInteriorCell)
                doorData.doorExitName = refObjDataGroup.DNAM.Value;
            if (doorData.leadsToAnotherCell && !doorData.leadsToInteriorCell)
            {
                var doorExitCell = BaseEngine.Instance.Data.FindCellRecord(BaseEngine.Instance.CellManager.GetCellId(doorData.doorExitPos, 0));
                doorData.doorExitName = doorExitCell != null ? (((CELLRecord)doorExitCell).FULL.Value ?? "Unknown Region") : doorData.doorName;
            }
            if (refObjDataGroup.DODT != null)
            {
                doorData.doorExitPos = NifUtils.NifPointToUnityPoint(refObjDataGroup.DODT.Value.Position.ToVector3());
                doorData.doorExitOrientation = NifUtils.NifEulerAnglesToUnityQuaternion(refObjDataGroup.DODT.Value.EulerAngles.ToVector3());
            }
            objData.name = doorData.leadsToAnotherCell ? doorData.doorExitName : "Use " + doorData.doorName;
        }

        public override void Interact()
        {
            if (doorData != null)
            {
                if (doorData.isOpen) Close();
                else Open();
            }
        }

        private void Open()
        {
            if (!doorData.moving)
                StartCoroutine(c_Open());
        }

        private void Close()
        {
            if (!doorData.moving)
                StartCoroutine(c_Close());
        }

        private IEnumerator c_Open()
        {
            doorData.moving = true;
            while (Quaternion.Angle(transform.rotation, doorData.openRotation) > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, doorData.openRotation, Time.deltaTime * 5f);
                yield return new WaitForEndOfFrame();
            }
            doorData.isOpen = true;
            doorData.moving = false;
        }

        private IEnumerator c_Close()
        {
            doorData.moving = true;
            while (Quaternion.Angle(transform.rotation, doorData.closedRotation) > 1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, doorData.closedRotation, Time.deltaTime * 5f);
                yield return new WaitForEndOfFrame();
            }
            doorData.isOpen = false;
            doorData.moving = false;
        }
    }
}