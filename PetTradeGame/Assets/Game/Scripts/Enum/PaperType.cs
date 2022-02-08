namespace Assets.Game.Scripts.Enum
{
    public enum PaperType
    {
        None = 0,
        PossessionLicense = 1 << 0,
        HealthCertificate = 1 << 1,
        SpecialPermit = 1 << 2,
        DealerLicence = 1 << 3,
    }
}
