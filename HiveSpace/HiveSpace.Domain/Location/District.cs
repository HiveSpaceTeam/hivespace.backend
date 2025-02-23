namespace HiveSpace.Domain.Location;
public class District(string code, string name, string nameEn, string fullName, string fullNameEn, string codeName, string provinceCode, int administrativeUnitId)
{
    public string Code { get; private set; } = code;
    public string Name { get; private set; } = name;
    public string NameEn { get; private set; } = nameEn;
    public string FullName { get; private set; } = fullName;
    public string FullNameEn { get; private set; } = fullNameEn;
    public string CodeName { get; private set; } = codeName;
    public string ProvinceCode { get; private set; } = provinceCode;
    public int AdministrativeUnitId { get; private set; } = administrativeUnitId;
}
