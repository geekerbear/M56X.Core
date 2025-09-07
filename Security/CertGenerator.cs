using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

namespace M56X.Core.Security
{
    /// <summary>
    /// 自签名证书生成
    /// </summary>
    public class CertGenerator
    {
        /// <summary>
        /// 自签名证书生成
        /// </summary>
        /// <param name="subjectName">主题</param>
        /// <param name="sanEntries">域名</param>
        /// <param name="outputPath">输出路径</param>
        /// <param name="password">密码</param>
        /// <param name="keySize">密钥长度</param>
        /// <param name="validityYears">有效时长</param>
        public static void CreateCACertificate(
        string subjectName,
        string[]? sanEntries = null,
        string outputPath = ".",
        string password = "",
        int keySize = 2048,
        int validityYears = 10)
        {
            // 创建RSA密钥对
            using var rsa = RSA.Create(keySize);
            var request = new CertificateRequest(
                new X500DistinguishedName($"CN={subjectName}"),
                rsa,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1);

            // 设置CA扩展
            request.CertificateExtensions.Add(
                new X509BasicConstraintsExtension(
                    certificateAuthority: true,
                    hasPathLengthConstraint: false,
                    pathLengthConstraint: 0,
                    critical: true));

            request.CertificateExtensions.Add(
                new X509KeyUsageExtension(
                    X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign,
                    critical: true));

            // 添加主题备用名称(SAN)
            if (sanEntries != null && sanEntries.Length > 0)
            {
                var sanBuilder = new SubjectAlternativeNameBuilder();
                foreach (var entry in sanEntries)
                {
                    if (System.Net.IPAddress.TryParse(entry, out _))
                        sanBuilder.AddIpAddress(System.Net.IPAddress.Parse(entry));
                    else
                        sanBuilder.AddDnsName(entry);
                }
                request.CertificateExtensions.Add(sanBuilder.Build());
            }

            // 生成自签名证书
            var certificate = request.CreateSelfSigned(
                DateTimeOffset.UtcNow.AddDays(-1),
                DateTimeOffset.UtcNow.AddYears(validityYears));

            // 导出为PFX和CER格式
            var pfxBytes = certificate.Export(
                X509ContentType.Pfx,
                password);
            var cerBytes = certificate.Export(
                X509ContentType.Cert);

            // 确保输出目录存在
            Directory.CreateDirectory(outputPath);

            // 保存文件
            File.WriteAllBytes(
                Path.Combine(outputPath, "ca.pfx"),
                pfxBytes);
            File.WriteAllBytes(
                Path.Combine(outputPath, "ca.cer"),
                cerBytes);
        }
    }
}
