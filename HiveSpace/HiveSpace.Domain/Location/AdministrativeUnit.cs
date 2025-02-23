namespace HiveSpace.Domain.Location;

public class AdministrativeUnit(int id, string fullName, string fullNameEn, string shortName, string shortNameEn, string codeName, string codeNameEn)
{
    public int Id { get; private set; } = id;
    public string FullName { get; private set; } = fullName;
    public string FullNameEn { get; private set; } = fullNameEn;
    public string ShortName { get; private set; } = shortName;
    public string ShortNameEn { get; private set; } = shortNameEn;
    public string CodeName { get; private set; } = codeName;
    public string CodeNameEn { get; private set; } = codeNameEn;
}
