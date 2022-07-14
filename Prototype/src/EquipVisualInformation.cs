namespace OutwardModTemplate
{
    [System.Serializable]
    public class EquipVisualInformation
    {
        public string SLPackName;
        public string AssetBundleName;
        public string PrefabName;

        public EquipmentSlot.EquipmentSlotIDs EquipmentSlotID;

        public EquipVisualInformation(string sLPackName, string assetBundleName, string prefabName, EquipmentSlot.EquipmentSlotIDs equipmentSlotID)
        {
            SLPackName = sLPackName;
            AssetBundleName = assetBundleName;
            PrefabName = prefabName;
            EquipmentSlotID = equipmentSlotID;
        }
    }
}
