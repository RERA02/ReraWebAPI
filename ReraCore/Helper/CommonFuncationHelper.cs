using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QRCoder;
using ReraModel.CommonFunction;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml;
using static QRCoder.PayloadGenerator;


namespace ReraCore.Helper
{
    public static class CommonFuncationHelper
    {
        private static IHttpContextAccessor _httpContextAccessor;
        private static HttpContext _httpContext;
        private static readonly byte[] key = new byte[24] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 0, 1, 2, 3, 4 };
        public static void Configure(HttpContext httpContext , IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpContext = httpContext;
        }
        public static T ConvertDataTable<T>(DataTable dt)
        {
            var json = JsonConvert.SerializeObject(dt);
            if (typeof(T).Name != "List`1")
            {
                json = json.TrimStart('[').TrimEnd(']');

            }
            var obj = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Ignore,
            });
            return obj;
        }

        // for Get data from Excel Convert in int 
        public static T ConvertExcelData<T>(DataTable dt)
        {

            var json = JsonConvert.SerializeObject(dt);

            // Check if T is a List and remove the array brackets if not
            if (typeof(T).Name != "List`1")
            {
                json = json.TrimStart('[').TrimEnd(']');
            }

            // Deserialize with custom settings
            var obj = JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Include,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = new List<JsonConverter> { new IntegerJsonConverter() } // Use custom converter
            });

            return obj;
        }


        public class IntegerJsonConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(int) || objectType == typeof(int?);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                // If the value is a float or double, convert it to an integer
                if (reader.TokenType == JsonToken.Float || reader.TokenType == JsonToken.Integer)
                {
                    return Convert.ToInt32(reader.Value);
                }

                return null;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value);
            }
        }

        public static PagigantionList GetPaginationData<T>(List<T> items, int? totalRecord, int pageNumber, int pageSize)
        {
            var _totalRecord = totalRecord ?? 0;
            var totalPagination = (int)Math.Ceiling(_totalRecord / (double)pageSize);
            var paginationData = new PagigantionList
            {
                PageRecord = items.Count,// per page result records count
                TotalRecord = _totalRecord,// total records count
                PageNumber = pageNumber,// page index
                PageSize = pageSize,// record to display
                TotalPagination = totalPagination,// total pages to display
                HasNext = pageNumber < totalPagination,// next option
                HasPrevious = pageNumber > 1,// prev. option
            };
            return paginationData;
        }
        public static string AvoidSQLInjection_Char(string str)
        {
            str = str?.Trim();
            str = str?.Replace("'", "");
            str = str?.Replace("==", "=");
            return str;
        }
        public static string MakeError(ErrorDescription errorDesc)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Error = {errorDesc.Message}");
            sb.AppendLine($"Page = {errorDesc.PageName}.{errorDesc.ActionName}");
            sb.AppendLine($"@SqlExecutableQuery = {errorDesc.SqlExecutableQuery}");
            return sb.ToString();
        }
        public static string CreateMD5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    //sb.Append(hashBytes[i].ToString("X2"));
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string EmitraEncrypt(string Text)
        {
            string Key = "681D392DB4FD7B8C6562712DFEEF1";
            string initVector = "tu89geji340t89u2";
            int keysize = 256;

            byte[] initVectorBytes = Encoding.UTF8.GetBytes(initVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(Text);
            PasswordDeriveBytes password = new PasswordDeriveBytes(Key, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] Encrypted = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return Convert.ToBase64String(Encrypted);
        }
        public static string EmitraDecrypt(string EncryptedText)
        {
            string Key = "681D392DB4FD7B8C6562712DFEEF1";
            string initVector = "tu89geji340t89u2";
            int keysize = 256;
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] DeEncryptedText = Convert.FromBase64String(EncryptedText);
            PasswordDeriveBytes password = new PasswordDeriveBytes(Key, null);
            byte[] keyBytes = password.GetBytes(keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream(DeEncryptedText);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[DeEncryptedText.Length];
            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
        }

        public static string EmitraEncryptNew(string textToEncrypt, string EncryptionPassword)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 256;
            rijndaelCipher.BlockSize = 128;
            byte[] pwdBytes = Encoding.UTF8.GetBytes(EncryptionPassword);
            pwdBytes = SHA256.Create().ComputeHash(pwdBytes);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0,
            plainText.Length));
        }
        public static string EmitraDecryptNew(string textToDecrypt, string EncryptionPassword)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 256;
            rijndaelCipher.BlockSize = 128;
            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] pwdBytes = Encoding.UTF8.GetBytes(EncryptionPassword);
            pwdBytes = SHA256.Create().ComputeHash(pwdBytes);
            byte[] keyBytes = new byte[16];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
            }
            len = keyBytes.Length;
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0,
            encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }
        public static string WebRequestinJson(string url, string postData, string contenttype)
        {
            string ret = string.Empty;

            StreamWriter requestWriter;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072;
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            if (webRequest != null)
            {
                webRequest.Method = "POST";
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = 40000;

                webRequest.ContentType = contenttype;
                //POST the data.
                using (requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter.Write(postData);
                }
            }

            HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
            Stream resStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(resStream);
            ret = reader.ReadToEnd();


            return ret;
        }
     
        public static string MD5HASHING(string input)
        {
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
        public static string AESEncrypt(string textToEncrypt, string encryptionKey)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            aes.IV = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            ICryptoTransform AESEncrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] buffer = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(AESEncrypt.TransformFinalBlock(buffer, 0, buffer.Length));
        }
        public static string AESDecrypt(string textToDecrypt, string encryptionKey)
        {
            RijndaelManaged aes = new RijndaelManaged();
            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Padding = PaddingMode.PKCS7;
            aes.Mode = CipherMode.CBC;
            aes.Key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            aes.IV = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(encryptionKey));
            ICryptoTransform AESDecrypt = aes.CreateDecryptor(aes.Key, aes.IV);
            byte[] buffer = Convert.FromBase64String(textToDecrypt);
            return Encoding.UTF8.GetString(AESDecrypt.TransformFinalBlock(buffer, 0, buffer.Length));

        }
        public static string GenerateTransactionNumber()
        {
            // Generate a new GUID.
            Guid guid = Guid.NewGuid();

            // Convert the first 6 bytes of the GUID to a byte array.
            byte[] bytes = guid.ToByteArray();

            // Take the first 6 bytes and convert them to a hexadecimal string.
            string hexString = BitConverter.ToString(bytes, 0, 6).Replace("-", "");

            Random rnd = new Random();
            //Create a 12-character transaction number from the hexadecimal string.
            string transactionNumber = rnd.Next(10000, 99999) + hexString;
            return transactionNumber;
        }
        public static string EgrassEncrypt(string textToEncrypt, string FilePath)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 0x80;
            rijndaelCipher.BlockSize = 0x80;
            byte[] pwdBytes = GetFileBytes(FilePath);
            byte[] keyBytes = new byte[0x10];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            ICryptoTransform transform = rijndaelCipher.CreateEncryptor();
            byte[] plainText = Encoding.UTF8.GetBytes(textToEncrypt);
            return Convert.ToBase64String(transform.TransformFinalBlock(plainText, 0, plainText.Length));
        }
        public static string EgrassDecrypt(string textToDecrypt, string FilePath)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 0x80;
            rijndaelCipher.BlockSize = 0x80;
            textToDecrypt = textToDecrypt.Replace(" ", "+");
            byte[] encryptedData = Convert.FromBase64String(textToDecrypt);
            byte[] pwdBytes = GetFileBytes(FilePath);
            byte[] keyBytes = new byte[0x10];
            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
            {
                len = keyBytes.Length;
            }
            Array.Copy(pwdBytes, keyBytes, len);
            rijndaelCipher.Key = keyBytes;
            rijndaelCipher.IV = keyBytes;
            byte[] plainText = rijndaelCipher.CreateDecryptor().TransformFinalBlock(encryptedData, 0, encryptedData.Length);
            return Encoding.UTF8.GetString(plainText);
        }
        public static byte[] GetFileBytes(String filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;
                buffer = new byte[length];
                int count;
                int sum = 0;
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }
        public static string SMS_GenerateNewRandom()
        {
            Random generator = new Random();
            String r = generator.Next(0, 1000000).ToString("D6");
            if (r.Distinct().Count() == 1)
            {
                r = SMS_GenerateNewRandom();
            }
            return r;
        }
        public static void WriteTextLog(string log)
        {
            string fileName = $"{ConfigurationHelper.StaticFileRootPath}Log/log_{DateTime.Now.ToString("yyyyMMdd")}.txt";
            System.IO.File.AppendAllTextAsync(fileName, $"{Environment.NewLine}------{DateTime.Now}------ : - {Environment.NewLine}{log}{Environment.NewLine}");
        }
        public static void ResizeImage(Stream sourcePath, string targetPath, int width, int height)
        {
            try
            {
                var image = System.Drawing.Image.FromStream(sourcePath);
                // Dim width = CInt((image.Width))
                // Dim height = CInt((image.Height))
                var destRect = new Rectangle(0, 0, width, height);
                var destImage = new Bitmap(width, height);
                destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                var thumbGraph = Graphics.FromImage(destImage);
                thumbGraph.CompositingMode = CompositingMode.SourceCopy;
                thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                thumbGraph.PixelOffsetMode = PixelOffsetMode.HighQuality;
                var wrapMode = new ImageAttributes();
                wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                thumbGraph.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                // Dim ms As MemoryStream = New MemoryStream()
                destImage.Save(targetPath, ImageFormat.Jpeg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static string Encrypt(string textToEncrypt, string EncryptionPassword = "DevITNOT")
        {
            var sb = new StringBuilder();
            var bytes = Encoding.Unicode.GetBytes(textToEncrypt);
            foreach (var t in bytes)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString(); // returns: "48656C6C6F20776F726C64" for "Hello world"           
        }
        public static string Decrypt(string textToDecrypt, string EncryptionPassword = "DevITNOT")
        {
            var bytes = new byte[textToDecrypt.Length / 2];
            for (var i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(textToDecrypt.Substring(i * 2, 2), 16);
            }
            return Encoding.Unicode.GetString(bytes); // returns: "Hello world" for "48656C6C6F20776F726C64"            
        }
        public static byte[] GenerateQrCode(string val)
        {
            string code = val;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
            QRCoder.BitmapByteQRCode qrCode = new QRCoder.BitmapByteQRCode(qrCodeData);
            byte[] bytes = qrCode.GetGraphic(20);
            return bytes;
        }

        public static byte[] GenerateQrCodeLowQuality(string val)
        {
            string code = val;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.L);
            QRCoder.BitmapByteQRCode qrCode = new QRCoder.BitmapByteQRCode(qrCodeData);
            byte[] bytes = qrCode.GetGraphic(5);
            return bytes;
        }

        #region Configure function

        public static string NEWGetIpAddress()  // Get IP Address
        {
            try
            {
                // Check for IP behind proxy/load balancer first
                var forwardedFor = _httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    // X-Forwarded-For can contain multiple IPs, take the first one
                    var ip = forwardedFor.Split(',')[0].Trim();
                    if (IsValidIpAddress(ip))
                        return ip;
                }
                // Check X-Real-IP header (used by some proxies)
                var realIp = _httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp) && IsValidIpAddress(realIp))
                    return realIp;
                // Check CF-Connecting-IP (Cloudflare)
                var cfIp = _httpContext.Request.Headers["CF-Connecting-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(cfIp) && IsValidIpAddress(cfIp))
                    return cfIp;
                // Fall back to direct connection IP
                var remoteIp = _httpContext.Connection?.RemoteIpAddress?.ToString();
                if (!string.IsNullOrEmpty(remoteIp))
                {
                    // Handle IPv6 mapped IPv4 addresses
                    if (remoteIp.StartsWith("::ffff:"))
                        remoteIp = remoteIp.Substring(7);
                    return remoteIp;
                }
            }
            catch (Exception ex)
            {
                // Log the exception
            }
            return string.Empty;
        }
        private static bool IsValidIpAddress(string ip)
        {
            return System.Net.IPAddress.TryParse(ip, out _);
        }
        //public static IPAddressModel GetIpAddress()
        //{
        //    IPAddressModel iPAddressModel = new IPAddressModel();
        //    var context = _httpContextAccessor?.HttpContext;

        //    if (context == null)
        //        return iPAddressModel;

        //    string userIP = null, userIPv6 = null;

        //    // 1. Cloudflare header
        //    if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
        //    {
        //        userIP = cfIp.ToString();
        //    }

        //    // 2. Fallback to Remote IP
        //    if (string.IsNullOrEmpty(userIP))
        //    {
        //        userIP = context.Connection.RemoteIpAddress?.ToString();
        //    }

        //    // 3. Handle localhost (::1)
        //    if (userIP == "::1" || userIP == "127.0.0.1")
        //    {
        //        var host = Dns.GetHostEntry(Dns.GetHostName());

        //        var ipAddress = host.AddressList
        //            .FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

        //        if (ipAddress != null)
        //        {
        //            userIP = ipAddress.ToString();
        //        }

        //        userIPv6 = host.AddressList.FirstOrDefault().ToString();
        //    }

        //    //// 4. Normalize IPv6 mapped IPv4
        //    //if (!string.IsNullOrEmpty(userIP) && userIP.StartsWith("::ffff:"))
        //    //{
        //    //    userIP = userIP.Substring(7);
        //    //}
        //    iPAddressModel.IPv4_Address = userIP;
        //    iPAddressModel.IPv6_Address = userIPv6;

        //    return iPAddressModel;
        //}

        public static IPAddressModel GetIpAddress()  // Get IP Address
        {
            IPAddressModel iPAddressModel = new IPAddressModel();
            try
            {
                string hostName = Dns.GetHostName(); // Retrive the Name of HOST

                iPAddressModel.IPv6_Address = Dns.GetHostByName(hostName).AddressList[0].ToString();
                iPAddressModel.IPv4_Address = Dns.GetHostByName(hostName).AddressList[1].ToString();

                //return _httpContext?.Connection?.RemoteIpAddress?.ToString() ?? string.Empty;
            }
            catch (Exception ex)
            {
            }
            //return string.Empty;
            return iPAddressModel;
        }
        #endregion

        #region nested class
        public class FinancialYear
        {
            private int yearNumber;
            private int firstMonthInYear = 4;

            public static FinancialYear Current
            {
                get { return new FinancialYear(DateTime.Today); }
            }

            private FinancialYear(DateTime forDate)
            {
                if (forDate.Month < firstMonthInYear)
                {
                    yearNumber = forDate.Year + 1;
                }
                else
                {
                    yearNumber = forDate.Year;
                }
            }

            public override string ToString()
            {
                return yearNumber.ToString();
            }
        }
        #endregion

        #region "Janadhar Services"


        //public static DataSet JanAdhSendOTP(string janId, string MemberID)
        //{
        //    string JanAdhaarClientID = "b10d20ae-367c-4cab-8b14-49330d4a392d";
        //    DataSet objdsotp = new DataSet();
        //    try
        //    {
        //        string Scheme = "CEA";
        //        var request = (HttpWebRequest)WebRequest.Create("https://api.sewadwaar.rajasthan.gov.in/app/live/Janaadhaar/Prod/Service/Gen/Otp/For?client_id=" + JanAdhaarClientID);
        //        request.Method = "POST";
        //        string postData = "<root><genOtpFor><janaadharId>" + janId+1 + "</janaadharId><mid>" + MemberID + "</mid><schemeId>" + Scheme + "</schemeId></genOtpFor></root>";
        //        //string postData = "<root><genOtpFor><janaadharId>5142413419</janaadharId><mid>55506934591</mid><schemeId>CEA</schemeId></genOtpFor></root>";
        //        byte[] byteArray = Encoding.ASCII.GetBytes(postData);
        //        request.ContentType = "text/xml;charset=utf-8";
        //        request.ContentLength = byteArray.Length;
        //        Stream requestStream = request.GetRequestStream();
        //        requestStream.Write(byteArray, 0, byteArray.Length);
        //        requestStream.Close();

        //        var response = (HttpWebResponse)request.GetResponse();
        //        var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

        //        int foundS1 = responseString.IndexOf("<root>");
        //        responseString = responseString.Substring(foundS1);

        //        objdsotp = ConvertXMLResponceToDataSet(responseString);

        //        StreamReader respStream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
        //        string receivedResponse = respStream.ReadToEnd();
        //        respStream.Close();
        //        response.Close();

        //    }
        //    catch (Exception ex) { }

        //    return objdsotp;
        //}

        public static DataSet JanAdhSendOTP(string janId, string MemberID, string requestUrlHost)
        {
            string JanAdhaarClientID = ConfigurationHelper.JanAdhaar_SendValidateOTP_ClientID;
            DataSet objdsotp = new DataSet();
            try
            {
                string Scheme = "CEA";
               
                var request = (HttpWebRequest)WebRequest.Create(ConfigurationHelper.JanAdhaar_SendOTP_URL + JanAdhaarClientID);
                request.Method = "POST";

                string postData = "";
                if (requestUrlHost == "localhost:4200" || ConfigurationHelper.EnvironmentName == "Staging")
                {
                    postData = "<root><genOtpFor><janaadharId>5142413419</janaadharId><mid>51098343296</mid><schemeId>CEA</schemeId></genOtpFor></root>";

                    var responseString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<root><status>true</status><tid>218829605</tid><mobile>******5100</mobile></root>";
                    int foundS1 = responseString.IndexOf("<root>");
                    responseString = "<root><status>true</status><tid>218829605</tid><mobile>******5100</mobile></root>";
                    objdsotp = ConvertXMLResponceToDataSet(responseString);

                }
                else
                {
                    postData = "<root><genOtpFor><janaadharId>" + janId + "</janaadharId><mid>" + MemberID + "</mid><schemeId>" + Scheme + "</schemeId></genOtpFor></root>";

                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    request.ContentType = "text/xml;charset=utf-8";
                    request.ContentLength = byteArray.Length;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(byteArray, 0, byteArray.Length);
                    requestStream.Close();

                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    int foundS1 = responseString.IndexOf("<root>");
                    responseString = responseString.Substring(foundS1);

                    objdsotp = ConvertXMLResponceToDataSet(responseString);

                    StreamReader respStream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
                    string receivedResponse = respStream.ReadToEnd();
                    respStream.Close();
                    response.Close();
                }
            }
            catch (Exception ex) { }

            return objdsotp;
        }

        public static DataSet ConvertXMLResponceToDataSet(string xmlData)
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                DataSet xmlDS = new DataSet();
                stream = new StringReader(xmlData);
                // Load the XmlTextReader from the stream
                reader = new XmlTextReader(stream);
                xmlDS.ReadXml(reader);
                return xmlDS;
            }
            catch
            {
                return null;
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        public static string GetAadharByVID(string _VID)
        {
            string _AadhaarNo = string.Empty;
            string subaua = "PDA8046000";//System.Configuration.ConfigurationManager.AppSettings["subauaRajkisan"].ToString();
            // string subaua = "PNSCL22866";
            try
            {
                if (!string.IsNullOrEmpty(_VID) && _VID.Length == 15)
                {
                    string ip = "";
                    string url = "https://aadhaarauthapi.rajasthan.gov.in/doit-aadhaar-enc-dec/demo/hsm/auth/detokenizeV2";
                    string ModifiedData = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?><AuthRequest UUID=\"" + _VID + "\" subaua=\"" + subaua + "\" flagType=\"" + "A" + "\" ver=\"2.5\"></AuthRequest>";
                    System.Net.WebRequest req = null;
                    WebResponse rsp = null;
                    try
                    {
                        req = WebRequest.Create(url);
                        req.Method = "POST";
                        req.ContentType = "application/xml";
                        req.Headers["appname"] = "Agriculture";
                        StreamWriter writer = new StreamWriter(req.GetRequestStream());
                        writer.Write(ModifiedData);
                        writer.Close();
                        rsp = req.GetResponse();
                        StreamReader sr = new StreamReader(rsp.GetResponseStream());
                        string results = sr.ReadToEnd();
                        sr.Close();
                        XmlDocument xml = new XmlDocument();
                        xml.LoadXml(results);
                        XmlNodeList xnList = xml.SelectNodes("/DSMTokanize");
                        string sa = "";
                        foreach (XmlNode xn in xnList)
                        {
                            sa = xn["status"].InnerText;
                            if (sa.ToUpper() == "Y")
                                _AadhaarNo = xn["AadhaarNo"].InnerText;
                            else
                                _AadhaarNo = "NO";
                        }
                        if (sa.ToUpper() == "N")
                        {
                            _AadhaarNo = "NO";
                        }
                    }
                    catch (Exception ex)
                    {
                        _AadhaarNo = "NO#" + ex.Message;
                    }
                }
                else
                {
                    _AadhaarNo = "NO" + "#" + "ReferenceId Not Found.";
                }
            }
            catch (Exception ex)
            {
                _AadhaarNo = "NO" + "#" + "ReferenceId Not Found##" + ex.Message;
            }
            return _AadhaarNo;
        }


        public static string SendEsignOTP(string AadharRefNumber, string requestUrlHost)
        {
            // string JanAdhaarClientID = "b10d20ae-367c-4cab-8b14-49330d4a392d";
            string errormsg = string.Empty;
            string succmsg = string.Empty;
            string _txnid = string.Empty;
            string json = "";
            string ss = "";
            string status = "";
            string responseCode = "";

            if (AadharRefNumber.ToString().Trim().ToString() != "")
            {
                // JSON payload
                var payload = new
                {
                    aadharid = AadharRefNumber,
                    departmentname = "EEMS"
                };

                json = System.Text.Json.JsonSerializer.Serialize(payload);

                //json = "{\"aadharid\":\"" + AadharRefNumber.ToString() + "\", \"departmentname\" : \"eems\"}";
                try
                {
                    string client_id = ConfigurationHelper.ClientId_EEMS2;
                    string URL = $"{ConfigurationHelper.SendOTPForAadharAuthEsignURL}?client_id={client_id}";

                    ss = WebRequestinJson(URL, json, "application/json");
                    JObject root = (JObject)JObject.Parse(ss);
                    foreach (var item in root)
                    {
                        if (item.Key == "Status")
                            status = item.Value.ToString().Replace("\"", ""); ;
                        if (item.Key == "TransactionId")
                            _txnid = item.Value.ToString().Replace("\"", ""); ;
                        if (item.Key == "ErrorMessage")
                            errormsg = item.Value.ToString().Replace("\"", ""); ;
                    }
                    if (!status.Equals("1"))
                    {
                        if (errormsg != "")
                        {
                            //errormsg = "status - " + status;
                            _txnid = "NO" + "#" + errormsg;
                        }
                        else
                        {
                            errormsg = "status - " + status;
                            _txnid = "NO" + "#" + _txnid;
                        }

                    }
                    else if (status.Equals("1"))
                    {
                        _txnid.Replace("\"", "");

                    }
                }
                catch (Exception ex)
                {
                    _txnid = "NO" + "#" + ex.Message;
                }
            }
            else
            {
                _txnid = "NO#";
                errormsg = "Fill Aadhar ID!";
            }
            return _txnid;
        }

       

        public static DataSet JanAdhGenerateOTPValidation(string tid, string otp, string requestUrlHost)
        {
            string JanAdhaarClientID = ConfigurationHelper.JanAdhaar_SendValidateOTP_ClientID;
            DataSet objdsotp = new DataSet();
            try
            {
                if (requestUrlHost == "localhost:4200" || ConfigurationHelper.EnvironmentName == "Staging")
                {
                    var responseString = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\r\n<root><tid>218829605</tid><isvalidate>false</isvalidate></root>";

                    int foundS1 = responseString.IndexOf("<root>");
                    responseString = responseString.Substring(foundS1);

                    objdsotp = ConvertXMLResponceToDataSet(responseString);
                }
                else
                {


                    string Scheme = "CEA";
                    var request = (HttpWebRequest)WebRequest.Create(ConfigurationHelper.JanAdhaar_ValidateOTP_URL + JanAdhaarClientID);
                    request.Method = "POST";
                    string postData = "<root><validateOtp><tid>" + tid + "</tid><otp>" + otp + "</otp><schemeId>" + Scheme + "</schemeId></validateOtp></root>";
                    byte[] byteArray = Encoding.ASCII.GetBytes(postData);
                    request.ContentType = "text/xml;charset=utf-8";
                    request.ContentLength = byteArray.Length;
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(byteArray, 0, byteArray.Length);
                    requestStream.Close();

                    var response = (HttpWebResponse)request.GetResponse();
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    int foundS1 = responseString.IndexOf("<root>");
                    responseString = responseString.Substring(foundS1);

                    objdsotp = ConvertXMLResponceToDataSet(responseString);

                    StreamReader respStream = new StreamReader(response.GetResponseStream(), System.Text.Encoding.Default);
                    string receivedResponse = respStream.ReadToEnd();
                    respStream.Close();
                    response.Close();
                }
            }
            catch (Exception ex) { }

            return objdsotp;
        }

        //public static JanAadharDetailsEntity GetDetailFromJanAadhar(string JanAadharid, string enrId, string aadharid, string janmemidselected)
        //{
        //    JanAadharDetailsEntity data = new JanAadharDetailsEntity();
        //    data.Status = "";
        //    JanAadharMemberDetails JanMemdt = new JanAadharMemberDetails();
        //    string scheme = string.Empty;
        //    string client_id = ConfigurationHelper.JanAadharAuthClientId;
        //    string Url = ConfigurationHelper.JaAdharMemberDetailsURL + "?client_id=" + client_id;

        //    scheme = "HEADM";

        //    string infoFlag = "PFE";
        //    string authoMode = "AOTP";
        //    string dateTime = string.Empty;
        //    try
        //    {
        //        dateTime = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
        //    }
        //    catch { dateTime = DateTime.Now.ToString(); }
        //    StringBuilder requestXml = new StringBuilder();
        //    requestXml.Append("<root>");
        //    if (JanAadharid.Length == 10)
        //    {
        //        requestXml.Append("<Info><janaadhaarId>" + JanAadharid + "</janaadhaarId>");
        //        requestXml.Append("<enrId></enrId>");
        //    }
        //    else
        //    {
        //        requestXml.Append("<Info><janaadhaarId></janaadhaarId>");
        //        requestXml.Append("<enrId>" + enrId + "</enrId>");

        //    }
        //    //requestXml.Append("<aadharId>" + aadharid + "</aadharId>");
        //    requestXml.Append("<aadharId></aadharId>");
        //    requestXml.Append("<scheme>" + scheme.TrimEnd() + "</scheme>");
        //    requestXml.Append("<infoFlg>" + infoFlag + "</infoFlg>");
        //    requestXml.Append("<authMode>" + authoMode + "</authMode>");
        //    requestXml.Append("<dateTime>" + dateTime + "</dateTime>");
        //    requestXml.Append("</Info>");
        //    requestXml.Append("</root>");

        //    //"<root><Info><janaadhaarId>5142413419</janaadhaarId><enrId></enrId><aadharId></aadharId><scheme>HEADM</scheme><infoFlg>PFE</infoFlg><authMode>AOTP</authMode><dateTime>09-05-2025 18:46:25</dateTime></Info></root>";

        //    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(requestXml.ToString());
        //    string pathStringRequest = "~/JanaadhaarnLog/Request";
        //    string pathStringResponse = "~/JanaadhaarnLog/Response";
        //    //new csCommon().CreateTextFile(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(pathStringRequest), aadharid) + ".txt", pathStringRequest, requestXml.ToString());
        //    var req = (HttpWebRequest)WebRequest.Create(Url);



        //    req.ContentType = "application/xml";
        //    req.Method = "POST";
        //    req.ContentLength = bytes.Length;



        //    using (Stream os = req.GetRequestStream())
        //    {
        //        os.Write(bytes, 0, bytes.Length);
        //    }



        //    string response = "";



        //    using (WebResponse resp = req.GetResponse())
        //    {
        //        using (StreamReader sr = new StreamReader(resp.GetResponseStream()))
        //        {
        //            response = sr.ReadToEnd().Trim();
        //            //"<root><requestId>headm$618056276</requestId><cmsg></cmsg><personalInfo><member><enrId>9999-I3KI-00164</enrId><janaadhaarId>5142413419</janaadhaarId><nameEng>Tajreen Bi</nameEng><nameHnd>तजरीन बी</nameHnd><gender>Female</gender><fnameEng>Abdul Rahim</fnameEng><fnameHnd>अब्दुल रहीम</fnameHnd><mnameEng>Madina</mnameEng><mnameHnd>मदीना</mnameHnd><snameEng>Mohammad Farid</snameEng><snameHnd>मोहम्मद  फरीद</snameHnd><dob>11/12/2000</dob><age>24</age><maritalStatus>Married</maritalStatus><jan_mid>0</jan_mid><aadhar></aadhar><AADHAR_REF_NO>444075376115708</AADHAR_REF_NO><eid></eid><mobile>7014437155</mobile><acc>10260100025349</acc><bankName>BANK OF BARODA</bankName><ifsc>BARB0STAJAI</ifsc><bankBranch>STATION ROAD BRANCH,STATION ROAD JAIPUR,,30204?</bankBranch><micr></micr><voterId></voterId><panNo></panNo><passport></passport><dlNo></dlNo><email></email><qualification>Literate</qualification><relationTyp>Self</relationTyp><disability></disability><income>Less than 5000</income><occupation>House wife</occupation><isorphan>N</isorphan><isStateGovtEmp>N</isStateGovtEmp><rghs_no></rghs_no><ekyc_status>Y</ekyc_status><chiranjeevi_flag>Y</chiranjeevi_flag><chiranjeevi_scheme>NFSA</chiranjeevi_scheme><disabilityPercentage></disabilityPercentage><disabilityType></disabilityType><caste></caste><hof_jan_m_id>55506934591</hof_jan_m_id></member><member><enrId>9999-I3KI-00164</enrId><janaadhaarId>5142413419</janaadhaarId><nameEng>Mohammad Farid</nameEng><nameHnd>मोहम्मद फरीद</nameHnd><gender>Male</gender><fnameEng>Liyakat Ali</fnameEng><fnameHnd>लियाकत अली</fnameHnd><mnameEng>Aamna Bano</mnameEng><mnameHnd>आमना बानो</mnameHnd><snameEng>Tajreen Bi</snameEng><snameHnd>तजरीन बी</snameHnd><dob>02/02/1991</dob><age>34</age><maritalStatus>Married</maritalStatus><jan_mid>51098343296</jan_mid><aadhar></aadhar><AADHAR_REF_NO>444240328936336</AADHAR_REF_NO><eid></eid><mobile>7014437155</mobile><acc></acc><bankName></bankName><ifsc></ifsc><bankBranch></bankBranch><micr></micr><voterId>XNJ/0406025</voterId><panNo></panNo><passport></passport><dlNo></dlNo><email></email><qualification>Post Graduate</qualification><relationTyp>Husband/wife</relationTyp><disability>#NA</disability><income>1 lakh - 2 lakh</income><occupation>Pvt. Co.</occupation><isorphan>N</isorphan><isStateGovtEmp>N</isStateGovtEmp><rghs_no></rghs_no><ekyc_status>Y</ekyc_status><disabilityPercentage></disabilityPercentage><disabilityType></disabilityType><categoryDescEng>OBC</categoryDescEng><caste>43 MANIHAR</caste></member></personalInfo><family><familydetail><addressHnd>155, वार्ड न: - 26, जयपुर ग्रेटर, जयपुर</addressHnd><addressEng>155, Krishna Colony Naya Kheda, Ward No- 26, Jaipur Greater, Jaipur</addressEng><pin>302039</pin><caste>43 MANIHAR</caste><category>OBC</category><categoryCd>13</categoryCd><isRural>N</isRural><districtName>Jaipur</districtName><districtNameHin>जयपुर</districtNameHin><block_city>Jaipur Greater</block_city><block_city_hin>जयपुर ग्रेटर</block_city_hin><gp></gp><gpHin></gpHin><ward>Ward No- 26</ward><wardHin>वार्ड न: - 26</wardHin><village></village><villageHin></villageHin><districtCd>110</districtCd><block_cityCd>C194</block_cityCd><gpCd>C0194W026</gpCd><villageCd></villageCd><landType></landType><landCat></landCat><livSince></livSince><gasConn></gasConn><gasAgency></gasAgency><isMinority>N</isMinority></familydetail></family><entitlementInfo><entDetail><enrId>9999-I3KI-00164</enrId><jan_mid>0</jan_mid><mnregaNo></mnregaNo><bplNo></bplNo><ppoNo></ppoNo><rsby></rsby><rationCardNo></rationCardNo><rationCardCat>APL</rationCardCat><isNfsa></isNfsa></entDetail><entDetail><enrId>9999-I3KI-00164</enrId><jan_mid>51098343296</jan_mid><ppoNo></ppoNo></entDetail></entitlementInfo></root>";


        //            XmlDocument xml = new XmlDocument();
        //            xml.LoadXml(response);
        //            //new csCommon().CreateTextFile(Path.Combine(System.Web.HttpContext.Current.Server.MapPath(pathStringResponse), aadharid) + ".txt", pathStringResponse, response);
        //            XmlElement Dtlist1 = xml.DocumentElement;
        //            XmlElement Dtlist2 = xml.DocumentElement;
        //            List<string[]> listx = new List<string[]>();
        //            //XmlNodeList xDataList = xml.SelectNodes("//personalInfo/member");



        //            XmlNodeList nodeList = Dtlist1.SelectNodes("//personalInfo/member");
        //            string enrid = "", janaadhaarId = "", disability = "", disabilityType = "", disabilityPercentage = "", nameEng = "", nameHnd = "", fnameEng = "", fnameHnd = "", dob = "", aadhar = "", mobile = "", email = "", acc = "", bankName = "", ifsc = "", gender = "", bankBranch = "";
        //            string districtCd = "", block_cityCd = "", gpCd = "", villageCd = "", ward = "", isMinority = "", addressEng = "", addressHnd = "", pin = "", caste = "", category = "", districtName = "", block_city = "", gp = "", village = "", age = "", snameHnd = "", snameEng = "", maritalStatus = "", relationTyp = "";
        //            string micr = "", voterId = "", mnameEng = "", mnameHnd = "", income = "", occupation = "", qualification = "", panNo = "", passport = "", dlNo = "", eid = "", janmemid = "", isStateGovtEmp = "";


        //            if (nodeList.Count > 0)
        //            {
        //                foreach (XmlNode node in nodeList)
        //                {
        //                    janmemid = node.SelectSingleNode("jan_mid").InnerXml;
        //                    if (janmemid == "0")
        //                    {
        //                        janmemid = node.SelectSingleNode("hof_jan_m_id").InnerXml;
        //                    }

        //                    if (janmemidselected == janmemid)
        //                    {
        //                        data.Status = "OK";
        //                        JanMemdt.janmemid = janmemid;
        //                        JanMemdt.enrid = node.SelectSingleNode("enrId").InnerXml;
        //                        JanMemdt.janaadhaarId = node.SelectSingleNode("janaadhaarId").InnerXml;
        //                        JanMemdt.aadhar = node.SelectSingleNode("AADHAR_REF_NO").InnerXml;
        //                        JanMemdt.nameEng = node.SelectSingleNode("nameEng").InnerXml;
        //                        JanMemdt.nameHnd = node.SelectSingleNode("nameHnd").InnerXml;
        //                        JanMemdt.fnameEng = node.SelectSingleNode("fnameEng").InnerXml;
        //                        JanMemdt.fnameHnd = node.SelectSingleNode("fnameHnd").InnerXml;
        //                        JanMemdt.dob = node.SelectSingleNode("dob").InnerXml;
        //                        JanMemdt.gender = node.SelectSingleNode("gender").InnerXml;
        //                        JanMemdt.mobile = node.SelectSingleNode("mobile").InnerXml;
        //                        JanMemdt.email = node.SelectSingleNode("email").InnerXml;

        //                        JanMemdt.acc = node.SelectSingleNode("acc").InnerXml;
        //                        JanMemdt.bankName = node.SelectSingleNode("bankName").InnerXml;
        //                        JanMemdt.ifsc = node.SelectSingleNode("ifsc").InnerXml;
        //                        JanMemdt.bankBranch = node.SelectSingleNode("bankBranch").InnerXml;
        //                        JanMemdt.age = node.SelectSingleNode("age").InnerXml;
        //                        JanMemdt.snameEng = node.SelectSingleNode("snameEng").InnerXml;
        //                        JanMemdt.snameHnd = node.SelectSingleNode("snameHnd").InnerXml;
        //                        JanMemdt.maritalStatus = node.SelectSingleNode("maritalStatus").InnerXml;
        //                        JanMemdt.relationTyp = node.SelectSingleNode("relationTyp").InnerXml;

        //                        JanMemdt.mnameEng = node.SelectSingleNode("mnameEng").InnerXml;
        //                        JanMemdt.mnameHnd = node.SelectSingleNode("mnameHnd").InnerXml;
        //                        JanMemdt.voterId = node.SelectSingleNode("voterId").InnerXml;
        //                        JanMemdt.micr = node.SelectSingleNode("micr").InnerXml;
        //                        JanMemdt.income = node.SelectSingleNode("income").InnerXml;
        //                        JanMemdt.occupation = node.SelectSingleNode("occupation").InnerXml;
        //                        JanMemdt.qualification = node.SelectSingleNode("qualification").InnerXml;
        //                        JanMemdt.panNo = node.SelectSingleNode("panNo").InnerXml;
        //                        JanMemdt.passport = node.SelectSingleNode("passport").InnerXml;
        //                        JanMemdt.dlNo = node.SelectSingleNode("dlNo").InnerXml;
        //                        JanMemdt.eid = node.SelectSingleNode("eid").InnerXml;

        //                        JanMemdt.disability = node.SelectSingleNode("disability").InnerXml;
        //                        JanMemdt.disabilityType = node.SelectSingleNode("disabilityType").InnerXml;
        //                        JanMemdt.disabilityPercentage = node.SelectSingleNode("disabilityPercentage").InnerXml;
        //                        JanMemdt.isStateGovtEmp = node.SelectSingleNode("isStateGovtEmp").InnerXml;
        //                        JanMemdt.passportphotoBase64 = JanAadhaarMemberPhoto(JanMemdt.janaadhaarId, janmemid);
        //                    }
        //                }
        //                XmlNodeList nodeList1 = Dtlist2.SelectNodes("//family/familydetail");
        //                JanMemdt.Address = new JanAadharMemberAddressDetails();
        //                foreach (XmlNode node1 in nodeList1)
        //                {

        //                    JanMemdt.Address.addressEng = node1.SelectSingleNode("addressEng").InnerXml;
        //                    JanMemdt.Address.addressHnd = node1.SelectSingleNode("addressHnd").InnerXml;
        //                    JanMemdt.Address.pin = node1.SelectSingleNode("pin").InnerXml;
        //                    JanMemdt.caste = node1.SelectSingleNode("caste").InnerXml;
        //                    JanMemdt.category = node1.SelectSingleNode("category").InnerXml;
        //                    JanMemdt.Address.districtName = node1.SelectSingleNode("districtName").InnerXml;
        //                    JanMemdt.Address.block_city = node1.SelectSingleNode("block_city").InnerXml;
        //                    JanMemdt.Address.gp = node1.SelectSingleNode("gp").InnerXml;
        //                    JanMemdt.Address.village = node1.SelectSingleNode("village").InnerXml;
        //                    JanMemdt.Address.ward = node1.SelectSingleNode("ward").InnerXml;
        //                    JanMemdt.Address.isRural = node1.SelectSingleNode("isRural").InnerXml;
        //                    JanMemdt.Address.isMinority = node1.SelectSingleNode("isMinority").InnerXml;
        //                    JanMemdt.Address.districtCd = node1.SelectSingleNode("districtCd").InnerXml;
        //                    JanMemdt.Address.block_cityCd = node1.SelectSingleNode("block_cityCd").InnerXml;
        //                    JanMemdt.Address.gpCd = node1.SelectSingleNode("gpCd").InnerXml;
        //                    JanMemdt.Address.villageCd = node1.SelectSingleNode("villageCd").InnerXml;
        //                }
        //            }
        //            else
        //            {
        //                data.Status = "";
        //                data.Message = "Member details not found";
        //            }
        //        }

        //    }
        //    if (data.Status == "")
        //    {
        //        data.Message = "Member details not found";
        //    }
        //    data.UserDetails = new JanAadharMemberDetails();
        //    data.UserDetails = JanMemdt;
        //    return data;
        //}

        #endregion

        public static string EncodeUTF8ToBase64(string text)
        {
            // Convert the string to UTF-8 bytes
            byte[] encodedBytes = System.Text.Encoding.UTF8.GetBytes(text);
            // Convert the bytes to a Base64 string
            return Convert.ToBase64String(encodedBytes);
        }

        // Decode Base64 string back to original string
        public static string DecodeBase64ToString(string base64EncodedText)
        {
            // Convert Base64 string back to byte array
            byte[] decodedBytes = Convert.FromBase64String(base64EncodedText);
            // Convert the byte array back to the original UTF-8 string
            return System.Text.Encoding.UTF8.GetString(decodedBytes);
        }


        // Class properties (you can adjust the access modifiers as needed)

        public static string yearlyEng = "https://dteapp.rajasthan.gov.in/bter_engexam/files/documents/";
        public static string yearlyNonEng = "https://dteapp.hte.rajasthan.gov.in/bter_exam/files/documents/";
        public static string semEng = "https://dteapp.hte.rajasthan.gov.in/bter_eng_sem_exam/files/documents/";
        public static string semNonEng = "https://dteapp.hte.rajasthan.gov.in/bter_sem_exam/files/documents/";



       

        public static string EncryptJanAddhar(string plainText, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);

            if (keyBytes.Length != 16)
                throw new ArgumentException("Key must be 16 bytes for AES-128.");

            byte[] iv = new byte[16]; // Initialization Vector: all zeros
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var encryptor = aes.CreateEncryptor())
                using (var ms = new MemoryStream())
                using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    cs.Write(plainBytes, 0, plainBytes.Length);
                    cs.FlushFinalBlock();
                    return Convert.ToBase64String(ms.ToArray());
                }
            }
        }
        public static string DecryptJanAddhar(string cipherText, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] iv = new byte[16];
            byte[] cipherBytes = Convert.FromBase64String(cipherText);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var decryptor = aes.CreateDecryptor())
                using (var ms = new MemoryStream(cipherBytes))
                using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                using (var sr = new StreamReader(cs))
                {
                    return sr.ReadToEnd();
                }
            }
        }


        public static string GenerateOTP(string AadharNo)
        {
            string errormsg = string.Empty;
            string succmsg = string.Empty;
            string _txnid = string.Empty;
            string json = "";
            string ss = "";
            string status = "";
            string responseCode = "";
            if (AadharNo.ToString().Trim().ToString() != "")
            {
                json = "{\"aadharid\":\"" + AadharNo.ToString() + "\", \"departmentname\" : \"AgriCulture\"}";
                try
                {
                   string KeyValue = ConfigurationHelper.SendEsignOTP;
                    //KeyValue = GetURLByKey("GenericSendAadharOTP");

                    ss = WebRequestinJson(KeyValue, json, "application/json");
                    JObject root = (JObject)JObject.Parse(ss);
                    foreach (var item in root)
                    {
                        if (item.Key == "Status")
                            status = item.Value.ToString().Replace("\"", ""); ;
                        if (item.Key == "TransactionId")
                            _txnid = item.Value.ToString().Replace("\"", ""); ;
                        if (item.Key == "ErrorMessage")
                            errormsg = item.Value.ToString().Replace("\"", ""); ;
                    }
                    if (!status.Equals("1"))
                    {
                        if (errormsg != "")
                        {
                            //errormsg = "status - " + status;
                            _txnid = "NO" + "#" + errormsg;
                        }
                        else
                        {
                            errormsg = "status - " + status;
                            _txnid = "NO" + "#" + _txnid;
                        }

                    }
                    else if (status.Equals("1"))
                    {
                        _txnid.Replace("\"", "");

                    }
                }
                catch (Exception ex)
                {

                    _txnid = "NO" + "#" + ex.Message;
                }
            }
            else
            {
                _txnid = "NO#";
                errormsg = "Fill Aadhar ID!";
            }
            return _txnid;

        }

        public static string VerifyRecheckFSOTP(string otpnumber, string txn, string PvStage = "")
        {
            string str = string.Empty;
            string result = CommonFuncationHelper.VerifyAadhaarOTPWithoutEncypt(otpnumber, txn, "VerifyAadhaarOTP");
            if (result == "Success" || otpnumber == "000000")
            {
                if (result == "Success" || otpnumber == "000000")
                {
                    string fileName = "Jun162025114610972475.pdf"; // or dynamically get this from DB
                    string basePath = AppDomain.CurrentDomain.BaseDirectory;
                    string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\.."));
                    string folderPath = Path.Combine(projectRoot, "StaticFiles", "Employer", "OTRDocument");
                    //string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StaticFiles", "Employer", "OTRDocument");
                    string pdfPath = Path.Combine(folderPath, fileName);

                    if (System.IO.File.Exists(pdfPath))
                    {
                        str = CommonFuncationHelper.GenerateEsign(pdfPath, txn);
                    }
                    else
                    {
                        str = "PDF file not found.";
                    }
                }
                else
                {
                    str = "Invalid OTP";
                }
            }
            else
            {
                str = "Invalid OTP";
            }
            

            return str;
        }
        public static string VerifyAadhaarOTPWithoutEncypt(string OTP, string TXN, string KeyName)
        {
            string message = string.Empty;
            string status = "";
            string json = "";

            if (!string.IsNullOrEmpty(TXN))
            {
                try
                {
                    string url = "https://api.sewadwaar.rajasthan.gov.in/app/live/RajeSign/Prod/all/webresources/generic/authOTP/Aadhar?client_id=f6de7747-60d3-4cf0-a0ae-71488abd6e95";
                    json = $"{{\"otp\":\"{OTP}\",\"transactionid\":\"{TXN}\"}}";

                    string response = CommonFuncationHelper.WebRequestinJson(url, json, "application/json");
                    JObject root = JObject.Parse(response);

                    if (root.TryGetValue("status", StringComparison.OrdinalIgnoreCase, out JToken value))
                    {
                        status = value.ToString();
                        message = status == "1" ? "Success" : "Invalid OTP!";
                    }
                }
                catch (Exception ex)
                {
                    message = "Error: " + ex.Message;
                }
            }

            return message;
        }
        public static string GenerateEsign(string pdfPath, string txn)
        {
            string finalStatus = string.Empty;
            string finalDocument = string.Empty;
            string resultMessage = string.Empty;

            try
            {
                byte[] pdfBytes = File.ReadAllBytes(pdfPath);
                string pdfBase64 = Convert.ToBase64String(pdfBytes);
                string url = ConfigurationHelper.ESignSignURL;

                string json = $@"
        {{
            ""inputJson"": {{
                ""File"": ""{pdfBase64}""
            }},
            ""transactionid"": ""{txn}"",
            ""docname"": ""{Path.GetFileName(pdfPath)}"",
            ""designation"": """",
            ""status"": ""SelfAttested"",
            ""llx"": ""350"",
            ""lly"": ""100"",
            ""positionX"": ""50"",
            ""positionY"": ""100"",
            ""mode"": ""1""
        }}";

                string response = CommonFuncationHelper.WebRequestinJson(url, json, "application/json");
                JObject root = JObject.Parse(response);

                if (root.TryGetValue("status", StringComparison.OrdinalIgnoreCase, out JToken statusToken))
                {
                    finalStatus = statusToken.ToString();

                    if (finalStatus.ToLower() == "success" && root.TryGetValue("document", out JToken documentToken))
                    {
                        finalDocument = documentToken.ToString();
                        byte[] signedPdf = Convert.FromBase64String(finalDocument);

                        string signedFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "StaticFiles", "Employer", "SignedDocs");
                        Directory.CreateDirectory(signedFolder);
                        string outputPath = Path.Combine(signedFolder, Path.GetFileName(pdfPath));

                        File.WriteAllBytes(outputPath, signedPdf);
                        resultMessage = "Success";
                    }
                    else
                    {
                        resultMessage = "eSign failed: Document not returned.";
                    }
                }
            }
            catch (Exception ex)
            {
                resultMessage = "Error: " + ex.Message;
            }

            return resultMessage;
        }
        public static string SendSMS_byListNewLatest(string language, string Message, string Moblist, string templateid, string entityid )
        {
            try
            {
                string status = "";
               
                    //string username = new csCommon().GetURLByKey("esanchaarusername");
                    //string password = new csCommon().GetURLByKey("esanchaarpassword");
                    //string esanchaaruniqueid = new csCommon().GetURLByKey("esanchaaruniqueid");
                    //string esanchaarurl = new csCommon().GetURLByKey("esanchaarurl");
                    //string esanchaarserviceName = new csCommon().GetURLByKey("esanchaarserviceName");
                    string username = "RajKisanSMS";
                    string password = "Ksn@rJ_09Ms$";
                    string esanchaaruniqueid = "RAJKISAN_SMS";
                    string esanchaarurl = "https://api.sewadwaar.rajasthan.gov.in/app/live/eSanchar/Prod/api/OBD/CreateSMS/Request?client_id=f6de7747-60d3-4cf0-a0ae-71488abd6e95";
                    string esanchaarserviceName = "RAJKISANSMS";

                    if ((templateid == "1407167394910467557") || (templateid == "1407167574731150389") || (templateid == "1407168110431694962") || (templateid == "1407170359858006642"))
                    {
                        //esanchaarurl = new csCommon().GetURLByKey("esanchaarurlotp");
                        //username = new csCommon().GetURLByKey("esanchaarusernameotp");
                        //password = new csCommon().GetURLByKey("esanchaarpasswordotp");
                        //esanchaaruniqueid = new csCommon().GetURLByKey("esanchaaruniqueidotp");
                        //esanchaarserviceName = new csCommon().GetURLByKey("esanchaarserviceNameotp");

                          esanchaarurl = "https://api.sewadwaar.rajasthan.gov.in/app/live/eSanchar/Prod/api/OBD/CreateSMS/Request?client_id=f6de7747-60d3-4cf0-a0ae-71488abd6e95";
                          username = "RajKisanSMS";
                          password = "Ksn@rJ_09Ms$";
                          esanchaaruniqueid = "RAJKISAN_SMS";
                          esanchaarserviceName = "RAJKISANOTP";
                    }
                    else
                    {
                        username = "RajKisanSMS";
                         password = "Ksn@rJ_09Ms$";
                         esanchaaruniqueid = "RAJKISAN_SMS";
                        esanchaarurl = "https://api.sewadwaar.rajasthan.gov.in/app/live/eSanchar/Prod/api/OBD/CreateSMS/Request?client_id=f6de7747-60d3-4cf0-a0ae-71488abd6e95";
                         esanchaarserviceName = "RAJKISANOTP";
                     }

                    status = "";
                    System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate (
                    Object obj, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain,
                    System.Net.Security.SslPolicyErrors errors)
                    { return (true); };

                    if (Message == "" || Message == null)
                    {
                        status = "200";
                    }
                    else
                    {
                        string inputparams = "{\"UniqueID\":\"" + esanchaaruniqueid + "\",\"serviceName\":\"" + esanchaarserviceName + "\",\"language\":\"" + language + "\",\"message\":\"" + Message + "\",\"mobileNo\": " + Moblist + ",\"entityID\":\"" + entityid + "\",\"templateID\":\"" + templateid + "\"}";
                        //Service Unavailable Changes Govind Dated 26-12-2023 Start
                        //if (username == "RajKisanOtp")
                        //{
                        status = CommonFuncationHelper.WebRequestinJson_1(esanchaarurl, inputparams, "application/json", username, password);

                        JObject root = (JObject)JObject.Parse(status);
                        foreach (var item in root)
                        {
                            if (item.Key == "responseCode")
                                status = item.Value.ToString().Replace("\"", "");
                        }
                        //}
                        //else
                        //{
                        //    status = "200";
                        //}
                        //Service Unavailable Changes Govind Dated 26-12-2023 end
                    }

                return status;
            }
            catch (Exception ex)
            { 
                return ex.ToString();
            }

        }
        public static string WebRequestinJson_1(string url, string postData, string contenttype, string username, string password)
        {
            try
            {
                string ret = string.Empty;

                StreamWriter requestWriter;

                var webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
                if (webRequest != null)
                {
                    webRequest.Method = "POST";
                    webRequest.ServicePoint.Expect100Continue = false;
                    webRequest.Timeout = 40000;
                    webRequest.Headers.Add("username", username);
                    webRequest.Headers.Add("password", password);
                    webRequest.ContentType = contenttype;
                    //POST the data.
                    using (requestWriter = new StreamWriter(webRequest.GetRequestStream()))
                    {
                        requestWriter.Write(postData);
                    }
                }

                HttpWebResponse resp = (HttpWebResponse)webRequest.GetResponse();
                Stream resStream = resp.GetResponseStream();
                StreamReader reader = new StreamReader(resStream);
                ret = reader.ReadToEnd();
                return ret;
            }
            catch (WebException wex)
            {
                throw wex;
            }
        }
        public static async Task DownloadAndSaveFileInFolderAsync(string fileUrl, string savePath)
        {
            using (HttpClient client = new HttpClient())
            {
                // Get the file as a byte array
                byte[] fileBytes = await client.GetByteArrayAsync(fileUrl);

                // Write the byte array to the specified file path
                await File.WriteAllBytesAsync(savePath, fileBytes);
            }
        }

        public static string PostServiceCallingMethod(string url, string encryptedRequestJson)
        {
            return PostServiceCallingMethodAsync(url, encryptedRequestJson).GetAwaiter().GetResult();
        }

        private static async Task<string> PostServiceCallingMethodAsync(string url, string encryptedRequestJson)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.ExpectContinue = true;

                    var content = new StringContent(encryptedRequestJson, Encoding.UTF8, "application/json");

                    using (HttpResponseMessage response = await client.PostAsync(url, content))
                    {
                        response.EnsureSuccessStatusCode();
                        return await response.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.Message}";
            }
        }
        public static async Task WriteAllBytesAsync(string filePath, byte[] data)
        {
            // Ensure the directory exists
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Write bytes asynchronously
            await File.WriteAllBytesAsync(filePath, data);
        }

        public static string EncryptRera(string clearText)
        {
            byte[] inputArray = UTF8Encoding.UTF8.GetBytes(clearText);
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.GenerateKey();
            tripleDES.Key = key;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        public static string DecryptRera(string cipherText)
        {
            byte[] inputArray = Convert.FromBase64String(cipherText.Replace(" ", "+"));
            TripleDESCryptoServiceProvider tripleDES = new TripleDESCryptoServiceProvider();
            tripleDES.Key = key;
            tripleDES.Mode = CipherMode.ECB;
            tripleDES.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = tripleDES.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(inputArray, 0, inputArray.Length);
            tripleDES.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }


    }
}
