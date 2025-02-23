namespace HiveSpace.Domain.Location;
public class Province(string code, string name, string nameEn, string fullName, string fullNameEn, string codeName, int administrativeUnitId, int administrativeRegionId)
{
    public string Code { get; private set; } = code;
    public string Name { get; private set; } = name;
    public string NameEn { get; private set; } = nameEn;
    public string FullName { get; private set; } = fullName;
    public string FullNameEn { get; private set; } = fullNameEn;
    public string CodeName { get; private set; } = codeName;
    public int AdministrativeUnitId { get; private set; } = administrativeUnitId;
    public int AdministrativeRegionId { get; private set; } = administrativeRegionId;
}



