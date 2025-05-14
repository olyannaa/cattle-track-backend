using CAT.Controllers.DTO;

namespace CAT.Services.Interfaces
{
    public interface ICSVService
    {
        public IEnumerable<T> ReadCSV<T>(Stream file);
        public byte[] WriteCSV<T>(IEnumerable<T> items);
        public IEnumerable<AnimalCSVInfoDTO> ReadAnimalCSV(Stream file);
        public string GetFileName(string input);
    }
}
