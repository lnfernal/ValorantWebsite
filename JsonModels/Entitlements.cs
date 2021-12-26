namespace ValorantManager.JsonModels
{
    public class Entitlements
    {
        public class Rootobject
        {
            public Entitlementsbytype[] EntitlementsByTypes { get; set; }
        }

        public class Entitlementsbytype
        {
            public string ItemTypeID { get; set; }
            public Entitlement[] Entitlements { get; set; }
        }

        public class Entitlement
        {
            // public string TypeID { get; set; }
            public string ItemID { get; set; }
            public string InstanceID { get; set; }
        }
    }
}
