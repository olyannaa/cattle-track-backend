using Amazon.Runtime;
using Amazon.S3.Model;
using Amazon.S3;
using Amazon;

namespace CAT.Services
{
    public class YandexS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public YandexS3Service(IConfiguration configuration)
        {
            _bucketName = configuration["YandexS3:BucketName"];

            var credentials = new BasicAWSCredentials(
                configuration["YandexS3:AccessKey"],
                configuration["YandexS3:SecretKey"]
            );

            var config = new AmazonS3Config
            {
                ServiceURL = "https://storage.yandexcloud.net", 
                ForcePathStyle = true, 
                SignatureVersion = "4" 
            };

            _s3Client = new AmazonS3Client(credentials, config);
        }

        public async Task<string> UploadFileAsync(Stream fileStream, string fileName, string contentType)
        {
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = fileStream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await _s3Client.PutObjectAsync(request);
            return $"https://storage.yandexcloud.net/{_bucketName}/{fileName}";
        }
        public async Task<bool> CheckS3AccessAsync()
        {
            try
            {
                var response = await _s3Client.ListBucketsAsync();
                Console.WriteLine("Buckets: " + string.Join(", ", response.Buckets.Select(b => b.BucketName)));

                return true;
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"S3 Error: {ex.Message}");
                Console.WriteLine($"Error Code: {ex.ErrorCode}");
                Console.WriteLine($"Request ID: {ex.RequestId}");
                Console.WriteLine($"Status Code: {ex.StatusCode}");
                Console.WriteLine($"AWS Region: {_s3Client.Config.RegionEndpoint?.SystemName}");
                Console.WriteLine($"Service URL: {_s3Client.Config.ServiceURL}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error: {ex.Message}");
            }
            return false;
        }
    }
}
