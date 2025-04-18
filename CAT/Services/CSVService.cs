using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.IO;
using CAT.Services.Interfaces;
using CAT.Controllers.DTO;
using CsvHelper.Configuration.Attributes;
using System.Reflection;

namespace CAT.Services
{
    public class CSVService : ICSVService
    {
        CsvConfiguration Config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            MissingFieldFound = null, // Игнорировать отсутствующие поля
            HeaderValidated = null,   // Отключить проверку заголовков
            PrepareHeaderForMatch = args => args.Header.Trim(), // Убрать лишние пробелы
            Delimiter = ";"
        };
        public IEnumerable<T> ReadCSV<T>(Stream file)
        {
            var reader = new StreamReader(file);
            var csv = new CsvReader(reader, Config);
            csv.Read();
            csv.ReadHeader();
            var records = csv.GetRecords<T>();

            return records;
        }

        public IEnumerable<AnimalCSVInfoDTO> ReadAnimalCSV(Stream file)
        {
            var reader = new StreamReader(file);
            var csv = new CsvReader(reader, Config);
            var records = new List<AnimalCSVInfoDTO>();

            csv.Read();
            csv.ReadHeader();

            // список известных столбцов
            var knownHeaders = typeof(AnimalCSVInfoDTO)
                .GetProperties()
                .SelectMany(prop => prop.GetCustomAttributes<NameAttribute>())
                .Select(attr => attr.Names)
                .SelectMany(names => names)
                .ToHashSet();

            while (csv.Read())
            {
                var animal = csv.GetRecord<AnimalCSVInfoDTO>();
                animal.AdditionalFields = new Dictionary<string, string>();

                foreach (var header in csv.HeaderRecord)
                {
                    if (!knownHeaders.Contains(header))
                    {
                        animal.AdditionalFields[header] = csv.GetField(header);
                    }
                }

                records.Add(animal);
            }

            return records;
        }

        public byte[] WriteCSV<T>(IEnumerable<T> items)
        {
            using (var stream = new MemoryStream())
            {
                using (var writeFile = new StreamWriter(stream, leaveOpen: true))
                {
                    var config = new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ",", HasHeaderRecord = false };
                    var csv = new CsvWriter(writeFile, config);
                    csv.WriteRecords(items);
                }
                stream.Position = 0;
                return stream.ToArray();
            }
        }
    }
}
