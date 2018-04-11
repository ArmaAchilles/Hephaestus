namespace AddonBuilder.Models
{
    public class Hash
    {
        public string ChecksumName { get; set; }
        public string FileHash { get; set; }

        public Hash(string checksumName, string hash)
        {
            ChecksumName = checksumName;
            FileHash = hash;
        }
    }
}
