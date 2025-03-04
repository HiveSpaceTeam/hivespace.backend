namespace HiveSpace.Domain.Location;
public class AdministrativeRegion(
    int id, string name, string nameEn, string codeName, string codeNameEn)
{
    public int Id { get; private set; } = id;
    public string Name { get; private set; } = name;
    public string NameEn { get; private set; } = nameEn;
    public string CodeName { get; private set; } = codeName;
    public string CodeNameEn { get; private set; } = codeNameEn;
}
