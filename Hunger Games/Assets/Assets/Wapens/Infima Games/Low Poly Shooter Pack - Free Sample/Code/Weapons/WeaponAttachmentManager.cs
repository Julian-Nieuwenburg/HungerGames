using UnityEngine;

namespace InfimaGames.LowPolyShooterPack
{
    public class WeaponAttachmentManager : WeaponAttachmentManagerBehaviour
    {
        #region FIELDS SERIALIZED

        [Header("Scope")]
        [SerializeField] private bool scopeDefaultShow = true;
        [SerializeField] private ScopeBehaviour scopeDefaultBehaviour;

        #endregion

        #region FIELDS

        private ScopeBehaviour scopeBehaviour;

        #endregion

        #region UNITY FUNCTIONS

        protected override void Awake()
        {
            if (scopeBehaviour == null)
            {
                scopeBehaviour = scopeDefaultBehaviour;
                scopeBehaviour.gameObject.SetActive(scopeDefaultShow);
            }
        }        

        #endregion

        #region GETTERS

        public override ScopeBehaviour GetEquippedScope() => scopeBehaviour;
        public override ScopeBehaviour GetEquippedScopeDefault() => scopeDefaultBehaviour;

        // ✅ **Fix: Implement missing method (bows don’t have muzzles, so return null)**
        public override MuzzleBehaviour GetEquippedMuzzle() => null;

        // ✅ **Fix: Implement missing method (bows don’t have magazines, so return null)**
        public override MagazineBehaviour GetEquippedMagazine() => null;

        #endregion
    }
}
