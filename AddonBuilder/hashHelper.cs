using System;
using System.Security.Cryptography;
using System.IO;

namespace AddonBuilder
{
    class HashHelper
    {
        /// <summary>
        ///  Recursively computes a hash of the specified directory.
        /// </summary>
        /// <param name="algorithmName">
        ///  The name of the hash algorithm.
        /// </param>
        /// <param name="directoryInfo">
        ///  The directory to hash.
        /// </param>
        /// <returns>
        ///  A hash of the specified directory and all of its subdirectories and files.
        /// </returns>
        /// <remarks>
        ///  The hash is based on the names of all subdirectories and files,
        ///  and the contents of all files.  It treats names as case sensitive.
        ///  It ignores the order of directory entries.
        ///  The way this method computes the hash from these things is not
        ///  specified in any standard, so the resulting hash will probably
        ///  not match hashes computed by any other program.
        /// </remarks>
        public static string HashDirectory(string algorithmName, DirectoryInfo directoryInfo)
        {
            using (var hashAlgorithm = HashAlgorithm.Create(algorithmName))
            {
                // CryptoStream and BinaryWriter seem the easiest way
                // to hash strings together with other data.
                using (var cryptoStream = new CryptoStream(Stream.Null, hashAlgorithm, CryptoStreamMode.Write))
                using (var binaryWriter = new BinaryWriter(cryptoStream))
                {
                    // Get the names of files and subdirectories at this level.
                    // Don't bother with EnumerateFileSystemInfos because we
                    // need all the names so that we can sort them.
                    FileSystemInfo[] infos = directoryInfo.GetFileSystemInfos();

                    // Because the order in which the file system returns
                    // directory entries is unspecified, it should not affect
                    // the hash.  Sort the entries by name, to ensure this.
                    // It might be nicer to treat the file names as case
                    // insensitive, but StringComparison.OrdinalIgnoreCase is
                    // not guaranteed to exactly match the case tables used
                    // by the file system, so let's not even try.
                    Array.Sort(infos, (a, b) => string.Compare(a.Name, b.Name, StringComparison.Ordinal));

                    // The following loop could be split in two parts:
                    // first compute a hash of each subdirectory and file,
                    // and then write all the hashes to cryptoStream.
                    // That way, the first part could be run in parallel.
                    // However, most of the time will probably be spent
                    // waiting for disk I/O, and parallelism is unlikely
                    // to help with that; it might just slow things down.
                    foreach (FileSystemInfo info in infos)
                    {
                        // Hash the name of the file or subdirectory.
                        // BinaryWriter.Write(string) includes a length prefix.
                        // Alternatively, we could write the characters and
                        // a null character.
                        binaryWriter.Write(info.Name);

                        if ((info.Attributes & FileAttributes.Directory) == 0)
                        {
                            // Write a flag that indicates this is a file,
                            // not a directory.
                            binaryWriter.Write((byte)'F');

                            // Instead of writing the contents of the file
                            // directly to cryptoStream, hash it and write
                            // only the hash.  This way, we don't have to
                            // hash the length of the file separately and
                            // worry that the length might change while
                            // we're reading.
                            binaryWriter.Write(HashFile(algorithmName, (FileInfo)info));
                        }
                        else
                        {
                            // Write a flag that indicates this is a directory,
                            // not a file.
                            binaryWriter.Write((byte)'D');

                            // Recursively hash the subdirectory.
                            binaryWriter.Write(HashDirectory(algorithmName, (DirectoryInfo)info));
                        }
                    }
                }

                // CryptoStream.Dispose has called
                // HashAlgorithm.TransformFinalBlock for us.

                return BitConverter.ToString(hashAlgorithm.Hash).Replace("-", String.Empty);
            }
        }

        private static byte[] HashFile(string algorithmName, FileInfo fileInfo)
        {
            using (var hashAlgorithm = HashAlgorithm.Create(algorithmName))
            using (var inputStream = fileInfo.OpenRead())
            {
                return hashAlgorithm.ComputeHash(inputStream);
            }
        }
    }
}
