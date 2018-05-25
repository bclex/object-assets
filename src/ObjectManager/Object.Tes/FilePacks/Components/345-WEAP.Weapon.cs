using OA.Core;
using OA.Tes.FilePacks.Records;
using System.Collections;
using UnityEngine;

namespace OA.Tes.FilePacks.Components
{
    public class WEAPComponent : BASEComponent
    {
        bool _isEquiped = false;
        bool _isVisible = true;
        bool _animating = false;
        Transform _hand = null;
        Renderer[] _renderers = null;

        void Start()
        {
            var WEAP = (WEAPRecord)record;
            //objData.icon = TESUnity.instance.Engine.textureManager.LoadTexture(WPDT.ITEX.value, "icons"); 
            objData.name = WEAP.FULL.Value;
            objData.weight = WEAP.DATA.Weight.ToString();
            objData.value = WEAP.DATA.Value.ToString();
            objData.interactionPrefix = "Take ";
            _renderers = GetComponentsInChildren<Renderer>();
            var colliders = GetComponents<MeshCollider>();
            for (var i = 0; i < colliders.Length; i++)
                Destroy(colliders[i]);
        }

        void Update()
        {
            if (_isEquiped)
                if (InputManager.GetButtonDown("Attack"))
                {
                    if (_isVisible) PlayAttackAnimation();
                    else SetVisible(true);
                }
        }

        public void SetVisible(bool visible)
        {
            if (visible == _isVisible)
                return;
            for (var i = 0; i < _renderers.Length; i++)
                _renderers[i].enabled = visible;
            _isVisible = visible;
        }

        public void Equip(Transform hand)
        {
            _transform.parent = hand;
            _transform.localPosition = Vector3.zero;
            _transform.localRotation = Quaternion.identity;
            _hand = hand;
            _isEquiped = true;
        }

        public void Unequip(Transform disabledObjects)
        {
            _transform.parent = disabledObjects;
            _isEquiped = false;
            _hand = null;
        }

        public void PlayAttackAnimation()
        {
            if (!_animating)
                StartCoroutine(PlayAttackAnimationCoroutine());
        }

        private IEnumerator PlayAttackAnimationCoroutine()
        {
            _animating = true;
            var originalRotation = _hand.localRotation;
            var target = Quaternion.Euler(0.0f, 0.0f, 90.0f);
            var time = 0.25f;
            var elapsed = 0.0f;
            var endOfFrame = new WaitForEndOfFrame();
            while (elapsed < time)
            {
                _hand.localRotation = Quaternion.Slerp(_hand.localRotation, target, elapsed / time);
                elapsed += Time.deltaTime;
                yield return endOfFrame;
            }
            time = 0.4f;
            elapsed = 0.0f;
            while (elapsed < time)
            {
                _hand.localRotation = Quaternion.Slerp(_hand.localRotation, originalRotation, elapsed / time);
                elapsed += Time.deltaTime;
                yield return endOfFrame;
            }
            _animating = false;
        }
    }
}
